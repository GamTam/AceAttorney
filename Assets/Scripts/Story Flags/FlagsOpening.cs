using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagsOpening : MonoBehaviour
{
    [SerializeField] private EvidenceSO _badge;
    [SerializeField] private DialogueSO _dialogue;
    [SerializeField] private TalkSO[] _talkText;
    [SerializeField] private TalkSO[] _talkTextAfterDC;
    [SerializeField] private MoveSO[] _moveablePlaces;
    [SerializeField] private MoveSO[] _moveablePlacesAfterDC;
    
    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    
    private bool _openedMenu;

    private void Start()
    {
        if (Globals.StoryFlags.Contains("Know Where Barbershop Is"))
        {
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
            
            GameObject.FindWithTag("UI").transform.Find("Investigation").transform.Find("Select")
                .GetComponent<InvestigationMenu>()._talkText = _talkTextAfterDC.ToList();
            GameObject.FindWithTag("UI").transform.Find("Investigation").transform.Find("Select")
                .GetComponent<InvestigationMenu>()._moveablePlaces = _moveablePlacesAfterDC.ToList();
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
            enabled = false;
            _openedMenu = true;
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
            GameObject.FindWithTag("UI").transform.Find("Investigation").transform.Find("Select")
                .GetComponent<InvestigationMenu>()._talkText = _talkText.ToList();
            GameObject.FindWithTag("UI").transform.Find("Investigation").transform.Find("Select")
                .GetComponent<InvestigationMenu>()._moveablePlaces = _moveablePlaces.ToList();
        }
    }
}
