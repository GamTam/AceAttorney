using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagsCourt : MonoBehaviour
{
    [SerializeField] private DialogueSO _dialogue;
    [SerializeField] private DialogueSO _newDialogue;
    [SerializeField] private GameObject _fadeOut;
    
    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    private MusicManager _musicManager;
    
    private bool _openedMenu;

    private void Start()
    {
        
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _dialogueManager = GameObject.FindGameObjectWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _playerInput.SwitchCurrentActionMap("Menu");

        
        if (Globals.StoryFlags.Contains("Won Case"))
        {
            _dialogueManager.StartText(_newDialogue);
        }
        else
        {
            _dialogueManager.StartText(_dialogue);
        }
    }

    void Update()
    {
        if (Globals.StoryFlags.Contains("Start Case") && _dialogueManager._doneTalking && !_openedMenu)
        {
            _openedMenu = true;
            GameObject obj = Instantiate(_fadeOut);
            SceneTransition trans = obj.GetComponent<SceneTransition>();
            trans._destination = "Courtroom";
        }
    }
}
