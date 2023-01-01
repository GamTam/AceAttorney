using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagsBarbershop : MonoBehaviour
{
    [SerializeField] private DialogueSO _dialogue;
    [SerializeField] private DialogueSO _endDialogue;
    
    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    
    private bool _openedMenu;
    private bool _addedDialogueOne;
    private bool _updatedMoveLocations;
    private bool _addedBuilding;

    private void Start()
    {
        
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _dialogueManager = GameObject.FindGameObjectWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _playerInput.SwitchCurrentActionMap("Menu");

        if (Globals.StoryFlags.Contains("Started Investigation"))
        {
            _openedMenu = true;
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
        }
        else
        {
            _dialogueManager.StartText(_dialogue);
        }
    }

    void Update()
    {
        if (Globals.StoryFlags.Contains("Started Investigation") && !_openedMenu && _dialogueManager._doneTalking)
        {
            _openedMenu = true;
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
        }

        if (Globals.StoryFlags.Contains("Financial Added") && Globals.StoryFlags.Contains("Clock Added") &&
            !Globals.StoryFlags.Contains("Got Evidence From Barbershop") && _dialogueManager._doneTalking)
        {
            _dialogueManager.StartText(_endDialogue);
        }
    }
}
