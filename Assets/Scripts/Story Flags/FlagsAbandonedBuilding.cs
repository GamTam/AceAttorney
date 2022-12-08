using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagsAbandonedBuilding : MonoBehaviour
{
    [SerializeField] private InvestigationMenu _investigation;
    [SerializeField] private DialogueSO _dialogue;
    
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

        if (Globals.StoryFlags.Contains("Broke In"))
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
        if (Globals.StoryFlags.Contains("Broke In") && !_openedMenu && _dialogueManager._doneTalking)
        {
            _openedMenu = true;
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
        }
    }
}
