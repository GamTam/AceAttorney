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
    [SerializeField] private EventSystem _eventSystem;
    [Header("Misc.")] [SerializeField] string _song;
    [SerializeField] SwapCharacters _swap;
    [SerializeField] private GameObject _background;

    private DialogueManager _dialogueManager;

    private PlayerInput _playerInput;

    private MusicManager _musicManager;
    private SoundManager _soundManager;
    private GameObject _selectedButton;

    void Start()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Menu");

        GameObject obj = GameObject.FindGameObjectWithTag("Dialogue Manager");
        _dialogueManager = obj.GetComponent<DialogueManager>();

        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        _background.transform.localScale = new Vector3(1, 0, 1);
        StartCoroutine(BackgroundAnimIn());
        
        _musicManager.Play(_song);
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
        if (_eventSystem.currentSelectedGameObject == null) return;
        transform.position = _eventSystem.currentSelectedGameObject.transform.position;

        if (_selectedButton != _eventSystem.currentSelectedGameObject)
        {
            if (_selectedButton != null) _soundManager.Play("select");
            _selectedButton = _eventSystem.currentSelectedGameObject;
        }
    }

    public void Examine(GameObject obj)
    {
        
    }
}