using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CourtRecordController : MonoBehaviour
{
    [SerializeField] private GameObject _base;
    [SerializeField] private GameObject _icon;
    [SerializeField] private GameObject _evidenceRow;
    [SerializeField] private GameObject _backupRow;

    [Header("Arrows")] 
    [SerializeField] private GameObject[] _arrows;
    [SerializeField] private RectTransform _targetPoint;

    [Header("Evidence Details")] 
    [SerializeField] private Image _checkSprite;
    [SerializeField] private Image _bigEvidenceIcon;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;

    [Header("Top Banner")] 
    [SerializeField] private Image _banner;
    [SerializeField] private List<Sprite> _bannerImages;

    public event Action<EvidenceSO> HasPresented;

    private List<Image> _oddEvidence = new List<Image>();
    private List<Image> _evenEvidence = new List<Image>();
    private GameObject _openPage;

    private bool _sound = true;
    private bool _evidence = true;
    private int _page = 1;
    private int _evidencePage = 1;
    private int _evidenceIndex = 0;
    private int _profilePage = 1;
    private int _profileIndex = 0;

    private SoundManager _soundManager;
    private PlayerInput _playerInput;
    private GameObject _selectedButton;
    private DialogueManager _dialogueManager;

    private CRNormal _normal;
    private CRPrompt _prompt;

    private Vector3 _speedVector = new Vector3(0.15f, 0.15f, 1);
    
    private Navigation _nav = new Navigation();
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _soundManager = GameObject.FindWithTag("Audio").GetComponent<SoundManager>();
        _dialogueManager = GameObject.FindWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _base.transform.localScale = new Vector3(0, 0, 0);
        
        _playerInput.SwitchCurrentActionMap("Null");
        _soundManager.Play("record flip");

        _normal = GetComponent<CRNormal>();
        
        _nav.wrapAround = true;
        _nav.mode = Navigation.Mode.Horizontal;

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(_icon, _evidenceRow.transform, false);
            obj.gameObject.SetActive(true);
            
            if (Globals.Evidence.Count > i)
            {
                obj.GetComponentsInChildren<Image>()[1].sprite = Globals.Evidence[i].Icon;
                obj.GetComponentsInChildren<Image>()[1].SetNativeSize();
                obj.GetComponent<Button>().navigation = _nav;
                _oddEvidence.Add(obj.GetComponentsInChildren<Image>()[1]);
            }
            else
            {
                obj.GetComponent<Button>().interactable = false;
                obj.GetComponentsInChildren<Image>()[1].enabled = false;
            }

            if (i == 0)
            {
                EventSystem.current.SetSelectedGameObject(obj.gameObject);
                _bigEvidenceIcon.sprite = Globals.Evidence[0].Icon;
                _bigEvidenceIcon.SetNativeSize();

                _title.SetText(Globals.Evidence[0].Name);
                _description.SetText(Globals.Evidence[0].Description);

                _checkSprite.enabled = true;
                if (Globals.Evidence[0].CheckImage == null)
                {
                    _checkSprite.enabled = false;
                }
            }
        }

        if (Globals.Evidence.Count > 10)
        {
            _arrows[0].SetActive(true);
            _arrows[1].SetActive(true);
        }

        _selectedButton = EventSystem.current.currentSelectedGameObject;
        
        while (_base.transform.localScale.x < 1)
        {
            yield return new WaitForSeconds(1 / 60f);
            _base.transform.localScale += _speedVector;
        }
        
        _playerInput.SwitchCurrentActionMap("Menu");
        _base.transform.localScale = new Vector3(1, 1, 1);
        _openPage = _evidenceRow;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(_selectedButton);
        
        if (_selectedButton != EventSystem.current.currentSelectedGameObject && gameObject.transform.GetSiblingIndex() == gameObject.transform.parent.childCount - 1)
        {
            if (_sound) _soundManager.Play("select");
            _selectedButton = EventSystem.current.currentSelectedGameObject;

            if (_selectedButton == _arrows[0])
            {
                StartCoroutine(PageTurn(true));
                return;
            }

            if (_selectedButton == _arrows[1])
            {
                StartCoroutine(PageTurn(false));
                return;
            }

            List<Image> images;
            List<EvidenceSO> masterList;
            
            if (_openPage == _backupRow)
            {
                images = _evenEvidence;
            }
            else
            {
                images = _oddEvidence;
            }

            masterList = _evidence ? Globals.Evidence : Globals.Profiles;
            
            int index = images.IndexOf(
                EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>()[1]) + (_page - 1) * 10;
            
            _bigEvidenceIcon.sprite = masterList[index].Icon;
            _bigEvidenceIcon.SetNativeSize();

            _title.SetText(masterList[index].Name);
            _description.SetText(masterList[index].Description);

            _checkSprite.enabled = true;
            if (masterList[index].CheckImage == null)
            {
                _checkSprite.enabled = false;
            }
        }
    }

    public void Close(bool sound=true)
    {
        if (sound) _soundManager.Play("back");
        StartCoroutine(CloseRecord());
    }

    public void ProfileEvidenceSwap()
    {
        _soundManager.Play("record flip");
        StartCoroutine(SwapProfileAndEvidence());
    }

    public void Present()
    {
        List<Image> images;
            
        if (_openPage == _backupRow)
        {
            images = _evenEvidence;
        }
        else
        {
            images = _oddEvidence;
        }
        
        int index = images.IndexOf(
            EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>()[1]) + (_page - 1) * 10;
        HasPresented?.Invoke(Globals.Evidence[index]);
    }

    public IEnumerator WaitThenLoop(EvidenceSO evidence, InvestigationMenu.EvidenceTalkPair[] pairs, DialogueSO wrongEvidence)
    {
        _playerInput.SwitchCurrentActionMap("Null");
        while (_base.transform.localScale.x > 0)
        {
            yield return new WaitForSeconds(1 / 60f);
            _base.transform.localScale -= _speedVector;
        }

        _base.transform.localScale = new Vector3(0, 0, 0);

        GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(0.1f);

        bool found = false;
        foreach (InvestigationMenu.EvidenceTalkPair pair in pairs)
        {
            if (pair.Evidence == evidence)
            {
                _dialogueManager.StartText(pair.Dialogue);
                found = true;
                break;
            }
        }
        
        if (!found) _dialogueManager.StartText(wrongEvidence);

        yield return new WaitUntil(() => _dialogueManager._doneTalking);

        yield return new WaitForSeconds(0.1f);
        GetComponent<Image>().enabled = false;
        
        while (_base.transform.localScale.x < 1)
        {
            yield return new WaitForSeconds(1 / 60f);
            _base.transform.localScale += _speedVector;
        }

        _base.transform.localScale = new Vector3(1, 1, 1);
        GetComponent<Image>().enabled = true;
        
        yield return new WaitForSeconds(0.1f);
        _playerInput.SwitchCurrentActionMap("Menu");
    }

    private IEnumerator CloseRecord()
    {
        while (_base.transform.localScale.x > 0)
        {
            yield return new WaitForSeconds(1 / 60f);
            _base.transform.localScale -= _speedVector;
        }

        _base.transform.localScale = new Vector3(0, 0, 0);

        GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        try
        {
            if (!_normal.enabled) GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
        } catch {}
        Destroy(gameObject);
    }

    public IEnumerator SwapProfileAndEvidence()
    {
        _sound = false;
        int index = 0;

        if (_openPage == _evidenceRow)
        {
            index = _oddEvidence.IndexOf(
                EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>()[1]);
            _openPage = _backupRow;
        }
        else 
        { 
            _openPage = _evidenceRow; 
            index = _evenEvidence.IndexOf(
            EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>()[1]);
        }

        if (_evidence)
        {
            _evidencePage = _page;
            _evidenceIndex = index;
        }
        else
        {
            _profilePage = _page;
            _profileIndex = index;
        }
        
        _evidence = !_evidence;
        _playerInput.SwitchCurrentActionMap("Null");
        while (_base.transform.localScale.y > 0)
        {
            yield return new WaitForSeconds(1 / 60f);
            _base.transform.localScale -= new Vector3(0, _speedVector.y);
        }

        if (_normal.enabled) _normal.SetControlLabel(_evidence);
        else if (_prompt.enabled) _prompt.SetControlLabel(_evidence);

        _base.transform.localScale = new Vector3(_base.transform.localScale.x, 0, _base.transform.localScale.y);
        EventSystem.current.SetSelectedGameObject(null);
        
        KillChildren(_evidenceRow.transform);
        KillChildren(_backupRow.transform);

        _arrows[0].SetActive(false);
        _arrows[1].SetActive(false);

        _banner.sprite = _evidence ? _bannerImages[0] : _bannerImages[1];
        _page = _evidence ? _evidencePage : _profilePage;
        index = _evidence ? _evidenceIndex : _profileIndex;
        
        _oddEvidence = new List<Image>();
        _evenEvidence = new List<Image>();
        
        AddItemsToRow();

        if (_openPage == _evidenceRow)
        {
            EventSystem.current.SetSelectedGameObject(_evenEvidence[index].transform.parent.gameObject);
            _openPage = _backupRow;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(_oddEvidence[index].transform.parent.gameObject);
            _openPage = _evidenceRow;
        }

        if (_evidence)
        {
            if (Globals.Evidence.Count > 10)
            {
                _arrows[0].SetActive(true);
                _arrows[1].SetActive(true);
            }
        }
        else
        {
            if (Globals.Profiles.Count > 10)
            {
                _arrows[0].SetActive(true);
                _arrows[1].SetActive(true);
            }
        }

        while (_base.transform.localScale.y < 1)
        {
            yield return new WaitForSeconds(1 / 60f);
            _base.transform.localScale += new Vector3(0, _speedVector.y);
        }

        _base.transform.localScale = new Vector3(_base.transform.localScale.x, 1, _base.transform.localScale.y);
        _sound = true;
        _playerInput.SwitchCurrentActionMap("Menu");
    }

    public IEnumerator PageTurn(bool left)
    {
        RectTransform currentRect;
        RectTransform hiddenRect;

        if (_openPage == _backupRow)
        {
            currentRect = _backupRow.GetComponent<RectTransform>();
            hiddenRect = _evidenceRow.GetComponent<RectTransform>();
        }
        else
        {
            currentRect = _evidenceRow.GetComponent<RectTransform>();
            hiddenRect = _backupRow.GetComponent<RectTransform>();
        }
        
        _playerInput.SwitchCurrentActionMap("Null");
        if (left)
        {
            _page -= 1;
            if (_page <= 0)
            {
                _page = (int) Math.Ceiling(Globals.Evidence.Count / 10f);
            }
        }
        else
        {
            _page += 1;
            if (_page > (int) Math.Ceiling(Globals.Evidence.Count / 10f))
            {
                _page = 1;
            }
        }

        AddItemsToRow();
        
        _arrows[0].SetActive(false);
        _arrows[1].SetActive(false);
        List<Image> images;
        
        if (left)
        {
            hiddenRect.localPosition = new Vector2(-_targetPoint.localPosition.x, hiddenRect.localPosition.y);

            while (currentRect.transform.position.x < _targetPoint.position.x)
            {
                currentRect.localPosition = Vector3.MoveTowards(currentRect.localPosition, _targetPoint.localPosition, 20 * Time.deltaTime * 200);
                hiddenRect.localPosition = Vector3.MoveTowards(hiddenRect.localPosition, new Vector2(0, hiddenRect.localPosition.y), 20 * Time.deltaTime * 200);
                yield return null;
            }
            
            if (_openPage == _evidenceRow)
            {
                KillChildren(_evidenceRow.transform);
                _oddEvidence = new List<Image>();
                images = _evenEvidence;
                EventSystem.current.SetSelectedGameObject(_evenEvidence[_evenEvidence.Count - 1].transform.parent.gameObject);
                _openPage = _backupRow;
            }
            else
            {
                KillChildren(_backupRow.transform);
                _evenEvidence = new List<Image>();
                images = _oddEvidence;
                EventSystem.current.SetSelectedGameObject(_oddEvidence[_oddEvidence.Count - 1].transform.parent.gameObject);
                _openPage = _evidenceRow;
            }
        }
        else
        {
            hiddenRect.localPosition = new Vector2(_targetPoint.localPosition.x, hiddenRect.localPosition.y);

            while (currentRect.transform.position.x > -_targetPoint.position.x)
            {
                currentRect.localPosition = Vector3.MoveTowards(currentRect.localPosition, new Vector2(-_targetPoint.localPosition.x, _targetPoint.localPosition.y), 20 * Time.deltaTime * 200);
                hiddenRect.localPosition = Vector3.MoveTowards(hiddenRect.localPosition, new Vector2(0, hiddenRect.localPosition.y), 20 * Time.deltaTime * 200);
                yield return null;
            }
            
            if (_openPage == _evidenceRow)
            {
                KillChildren(_evidenceRow.transform);
                _oddEvidence = new List<Image>();
                images = _evenEvidence;
                EventSystem.current.SetSelectedGameObject(_evenEvidence[0].transform.parent.gameObject);
                _openPage = _backupRow;
            }
            else
            {
                KillChildren(_backupRow.transform);
                _evenEvidence = new List<Image>();
                images = _oddEvidence;
                EventSystem.current.SetSelectedGameObject(_oddEvidence[0].transform.parent.gameObject);
                _openPage = _evidenceRow;
            }
        }
        
        foreach (Image icon in images)
        {
            if (icon.gameObject.transform.parent.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                foreach (EvidenceSO evidence in Globals.Evidence)
                {
                    if (evidence.Icon == icon.sprite)
                    {
                        _bigEvidenceIcon.sprite = evidence.Icon;
                        _bigEvidenceIcon.SetNativeSize();

                        _title.SetText(evidence.Name);
                        _description.SetText(evidence.Description);

                        _checkSprite.enabled = true;
                        if (evidence.CheckImage == null)
                        {
                            _checkSprite.enabled = false;
                        }

                        break;
                    }
                }
            }
        }
        _selectedButton = EventSystem.current.currentSelectedGameObject;
        _playerInput.SwitchCurrentActionMap("Menu");
        
        _arrows[0].SetActive(true);
        _arrows[1].SetActive(true);
    }

    private void AddItemsToRow()
    {
        List<EvidenceSO> items = new List<EvidenceSO>();

        if (_evidence)
        {
            items = Globals.Evidence;
        }
        else
        {
            items = Globals.Profiles;
        }
        
        for (int i = 0; i < 10; i++)
        {
            GameObject obj;
            if (_openPage == _evidenceRow)
            {
                obj = Instantiate(_icon, _backupRow.transform, false);
            }
            else
            {
                obj = Instantiate(_icon, _evidenceRow.transform, false);
            } 
            obj.gameObject.SetActive(true);
            
            if (items.Count > i + (_page - 1) * 10)
            {
                obj.GetComponentsInChildren<Image>()[1].sprite = items[i + (_page - 1) * 10].Icon;
                obj.GetComponentsInChildren<Image>()[1].SetNativeSize();
                if (_openPage == _evidenceRow)
                {
                    _evenEvidence.Add(obj.GetComponentsInChildren<Image>()[1]);
                }
                else
                {
                    _oddEvidence.Add(obj.GetComponentsInChildren<Image>()[1]);
                }
                obj.GetComponent<Button>().navigation = _nav;
            }
            else
            {
                obj.GetComponent<Button>().interactable = false;
                obj.GetComponentsInChildren<Image>()[1].enabled = false;
            }
        }
    }
    
    public void KillChildren(Transform transform)
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
