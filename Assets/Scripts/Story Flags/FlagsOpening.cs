using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagsOpening : MonoBehaviour
{
    [SerializeField] private EvidenceSO _badge;
    [SerializeField] private DialogueSO _dialogue;
    [SerializeField] private TalkSO[] _talkText;
    [SerializeField] private MoveSO[] _moveablePlaces;
    
    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    
    private bool _openedMenu;

    private void Start()
    {
        if (Globals.StoryFlags.Contains("Met Lyla"))
        {
            enabled = false;
            return;
        }
        
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _dialogueManager = GameObject.FindGameObjectWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        Globals.Evidence.Add(_badge);
        _playerInput.SwitchCurrentActionMap("Menu");
        _dialogueManager.StartText(_dialogue);
    }

    void Update()
    {
        if (Globals.StoryFlags.Contains("Opening Monalogue") && !_openedMenu && _dialogueManager._doneTalking)
        {
            _openedMenu = true;
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
            GameObject.FindWithTag("UI").transform.Find("Investigation").transform.Find("Select")
                .GetComponent<InvestigationMenu>()._talkText = _talkText;
            GameObject.FindWithTag("UI").transform.Find("Investigation").transform.Find("Select")
                .GetComponent<InvestigationMenu>()._moveablePlaces = _moveablePlaces;
        }
    }
}
