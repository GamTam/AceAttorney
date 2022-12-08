using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagsStreetView : MonoBehaviour
{
    [SerializeField] private InvestigationMenu _investigation;
    [SerializeField] private DialogueSO _dialogue;
    [SerializeField] private DialogueSO _newDialogue;
    [SerializeField] private MoveSO _abandonedBuilding;
    [SerializeField] private SwapCharacters _swap;
    [SerializeField] private GameObject _talk;
    [SerializeField] private GameObject _present;
    
    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    private MusicManager _musicManager;
    
    private bool _openedMenu;
    private bool _addedDialogueOne;
    private bool _updatedMoveLocations;

    private void Start()
    {
        
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _dialogueManager = GameObject.FindGameObjectWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _playerInput.SwitchCurrentActionMap("Menu");

        
        if (Globals.StoryFlags.Contains("Got Evidence From Gumshoe") &&
                 Globals.StoryFlags.Contains("Got Evidence From Barbershop") &&
                 !Globals.StoryFlags.Contains("Can Go To Abandoned Building"))
        {
            _investigation._song = "";
            Destroy(_talk);
            Destroy(_present);
            _dialogueManager.StartText(_newDialogue);
            _investigation._moveablePlaces.Add(_abandonedBuilding);
        } else if (Globals.StoryFlags.Contains("Got Evidence From Gumshoe") &&
                   Globals.StoryFlags.Contains("Got Evidence From Barbershop") &&
                   Globals.StoryFlags.Contains("Can Go To Abandoned Building"))
        {
            _musicManager.fadeOut();
            _swap.StartSwap(fadeIn:false, skipFade:true);
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
            Destroy(_talk);
            Destroy(_present);
            _investigation._song = "";
            _investigation._moveablePlaces.Add(_abandonedBuilding);
        } 
        else if (Globals.StoryFlags.Contains("Met Gumshoe"))
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
        if (Globals.StoryFlags.Contains("Met Gumshoe") && !_openedMenu && _dialogueManager._doneTalking)
        {
            _openedMenu = true;
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
        }
    }
}
