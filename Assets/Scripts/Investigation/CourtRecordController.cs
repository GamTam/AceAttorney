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
    
    public event Action<EvidenceSO> HasPresented;

    public List<Image> _oddEvidence = new List<Image>();
    public List<Image> _evenEvidence = new List<Image>();
    private GameObject _openPage;
    private int _page = 1;
    
    private SoundManager _soundManager;
    private PlayerInput _playerInput;
    private GameObject _selectedButton;
    private DialogueManager _dialogueManager;

    private Vector3 _speedVector = new Vector3(0.15f, 0.15f, 1);

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _soundManager = GameObject.FindWithTag("Audio").GetComponent<SoundManager>();
        _dialogueManager = GameObject.FindWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _base.transform.localScale = new Vector3(0, 0, 0);
        
        _playerInput.SwitchCurrentActionMap("Null");
        _soundManager.Play("record flip");

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(_icon, _evidenceRow.transform, false);
            obj.gameObject.SetActive(true);
            
            if (Globals.Evidence.Count > i)
            {
                obj.GetComponentsInChildren<Image>()[1].sprite = Globals.Evidence[i].Icon;
                obj.GetComponentsInChildren<Image>()[1].SetNativeSize();
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
        
        if (_selectedButton != EventSystem.current.currentSelectedGameObject)
        {
            _soundManager.Play("select");
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
            
            if (_openPage == _backupRow)
            {
                images = _evenEvidence;
            }
            else
            {
                images = _oddEvidence;
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
        }
    }

    public void Close(bool sound=true)
    {
        if (sound) _soundManager.Play("back");
        StartCoroutine(CloseRecord());
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
        
        foreach (Image icon in images)
        {
            if (icon.gameObject.transform.parent.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                foreach (EvidenceSO evidence in Globals.Evidence)
                {
                    if (evidence.Icon == icon.sprite)
                    {
                        HasPresented?.Invoke(evidence);
                    }
                }
            }
        }
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
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
        } catch {}
        Destroy(gameObject);
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
            
            if (Globals.Evidence.Count > i + (_page - 1) * 10)
            {
                obj.GetComponentsInChildren<Image>()[1].sprite = Globals.Evidence[i + (_page - 1) * 10].Icon;
                obj.GetComponentsInChildren<Image>()[1].SetNativeSize();
                if (_openPage == _evidenceRow)
                {
                    _evenEvidence.Add(obj.GetComponentsInChildren<Image>()[1]);
                }
                else
                {
                    _oddEvidence.Add(obj.GetComponentsInChildren<Image>()[1]);
                }
            }
            else
            {
                obj.GetComponent<Button>().interactable = false;
                obj.GetComponentsInChildren<Image>()[1].enabled = false;
            }
        }
        
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
    
    public void KillChildren(Transform transform)
    {
        Debug.Log(transform.childCount);
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

        Debug.Log(transform.childCount);
    }
}
