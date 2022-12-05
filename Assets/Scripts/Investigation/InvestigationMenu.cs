using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InvestigationMenu : MenuCursor
{
    [SerializeField] private GameObject _investigateCursor;
    [SerializeField] private GameObject _talkPrefab;
    [SerializeField] private GameObject _movePrefab;
    [SerializeField] private GameObject _presentPrefab;
    [SerializeField] private TalkSO[] _talkText;
    [SerializeField] private MoveSO[] _moveablePlaces;
    [SerializeField] public EvidenceTalkPair[] _evidenceDialogue;
    [SerializeField] public DialogueSO _wrongEvidence;
    [SerializeField] private List<EvidenceSO> _evidence;
    
    [Header("Misc.")] [SerializeField] string _song;
    [SerializeField] SwapCharacters _swap;
    [SerializeField] private GameObject _background;

    private DialogueManager _dialogueManager;

    private PlayerInput _playerInput;

    private MusicManager _musicManager;
    
    [Serializable]
    public struct EvidenceTalkPair {
        public EvidenceSO Evidence;
        public DialogueSO Dialogue;
    }

    void Awake()
    {
        Globals.Evidence = _evidence;
        base.Start();
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();

        GameObject obj = GameObject.FindGameObjectWithTag("Dialogue Manager");
        _dialogueManager = obj.GetComponent<DialogueManager>();

        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
    }

    private void Start()
    {
        _musicManager.Play(_song);
    }

    private void OnEnable()
    {
        _playerInput.SwitchCurrentActionMap("Menu");
        _background.transform.localScale = new Vector3(1, 0, 1);
        StartCoroutine(BackgroundAnimIn());

        if (_selectedButton == null)
        {
            Button[] buttons = GameObject.FindWithTag("Investigation").transform.Find("Buttons").GetComponentsInChildren<Button>();
            
            _selectedButton = buttons[0].gameObject;
        }
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_selectedButton);
    }

    private IEnumerator BackgroundAnimIn()
    {
        _background.transform.localScale = new Vector2(1, 0);
        for(float i = 0f; i < 1f; i += 0.1f)
        {
            _background.transform.localScale = new Vector2(1, i);
            yield return new WaitForSeconds(1/60f);
        }
        _background.transform.localScale = new Vector2(1, 1);
    }

    private IEnumerator BackgroundAnimOut()
    {
        _background.transform.localScale = new Vector2(1, 1);
        for(float i = 1f; i > 0f; i -= 0.1f)
        {
            _background.transform.localScale = new Vector2(1, i);
            yield return new WaitForSeconds(1/60f);
        }
        _background.transform.localScale = new Vector2(1, 0);
    }

    public void Click()
    {
        base.Click();
        _playerInput.SwitchCurrentActionMap("Null");
        StartCoroutine(BackgroundAnimOut());
        
        foreach (GameObject but in GameObject.FindGameObjectsWithTag("Button"))
        {
             if (but != EventSystem.current.currentSelectedGameObject) but.GetComponent<Animator>().Play("Fade Out");
        }

        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "Examine":
                StartCoroutine(Examine());
                break;
            case "Move":
                StartCoroutine(Move());
                break;
            case "Talk":
                StartCoroutine(Talk());
                break;
            case "Present":
                StartCoroutine(Present());
                break;
        }
    }

    private IEnumerator Examine()
    {
        _swap.StartSwap(fadeIn:false);
        yield return new WaitForSeconds(0.25f);
        _playerInput.SwitchCurrentActionMap("Investigation");
        Instantiate(_investigateCursor, (Vector2) Camera.main.transform.position, Quaternion.identity);
        GameObject.FindWithTag("Investigation").SetActive(false);
    }
    
    private IEnumerator Talk()
    {
        yield return new WaitForSeconds(0.25f);
        
        GameObject obj = Instantiate(_talkPrefab);
        obj.transform.SetParent(GameObject.FindWithTag("UI").transform, false);
        TalkManager talk = obj.GetComponent<TalkManager>();
        talk.ShowOptions(_talkText);
        
        GameObject.FindWithTag("Investigation").SetActive(false);
        _playerInput.SwitchCurrentActionMap("Menu");
    }
    
    private IEnumerator Move()
    {
        yield return new WaitForSeconds(0.25f);
        
        GameObject obj = Instantiate(_movePrefab);
        obj.transform.SetParent(GameObject.FindWithTag("UI").transform, false);
        MoveManager move = obj.GetComponent<MoveManager>();
        move.ShowOptions(_moveablePlaces);
        
        GameObject.FindWithTag("Investigation").SetActive(false);
        _playerInput.SwitchCurrentActionMap("Menu");
    }
    
    private IEnumerator Present()
    {
        yield return new WaitForSeconds(0.25f);
        
        GameObject obj = Instantiate(_presentPrefab);
        obj.transform.SetParent(GameObject.FindGameObjectsWithTag("UI")[1].transform, false);
        CRPresent pres = obj.GetComponent<CRPresent>();
        pres.enabled = true;
        
        GameObject.FindWithTag("Investigation").SetActive(false);
        _playerInput.SwitchCurrentActionMap("Menu");
    }
}