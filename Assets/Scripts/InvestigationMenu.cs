using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InvestigationMenu : MonoBehaviour
{
    [SerializeField] private GameObject _investigateCursor;
    
    [Header("Misc.")] [SerializeField] string _song;
    [SerializeField] SwapCharacters _swap;
    [SerializeField] private GameObject _background;

    private DialogueManager _dialogueManager;

    private PlayerInput _playerInput;

    private MusicManager _musicManager;
    private SoundManager _soundManager;
    private GameObject _selectedButton;
    private Animator _anim;

    void Awake()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _anim = GetComponent<Animator>();

        GameObject obj = GameObject.FindGameObjectWithTag("Dialogue Manager");
        _dialogueManager = obj.GetComponent<DialogueManager>();

        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
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
    
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(_selectedButton);
        transform.position = EventSystem.current.currentSelectedGameObject.transform.position;

        if (_selectedButton != EventSystem.current.currentSelectedGameObject)
        {
            if (_selectedButton != null) _soundManager.Play("select");
            _selectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

    public void Click(Button obj)
    {
        _playerInput.SwitchCurrentActionMap("Null");
        StartCoroutine(BackgroundAnimOut());
        
        foreach (Button but in GameObject.FindWithTag("UI").GetComponentsInChildren<Button>())
        {
            if (but != obj) but.GetComponent<Animator>().Play("Fade Out", 1);
        }
        
        _soundManager.Play("confirm");
        _swap.StartSwap(fadeIn:false);
        _anim.Play("Select Fade Out");

        switch (obj.name)
        {
            case "Examine":
                StartCoroutine(Examine());
                break;
        }
    }

    private IEnumerator Examine()
    {
        yield return new WaitForSeconds(0.25f);
        _playerInput.SwitchCurrentActionMap("Investigation");
        Instantiate(_investigateCursor, (Vector2) Camera.main.transform.position, Quaternion.identity);
        GameObject.FindWithTag("Investigation").SetActive(false);
    }
}