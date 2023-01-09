using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagsDetentionCenter : MonoBehaviour
{
    [SerializeField] private DialogueSO _dialogue;
    [SerializeField] private GameObject _fadeOut;
    
    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    
    private bool _openedMenu;
    private bool _addedDialogueOne;
    private bool _updatedMoveLocations;
    private bool _begunEnd;

    private void Start()
    {
        
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _dialogueManager = GameObject.FindGameObjectWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _playerInput.SwitchCurrentActionMap("Menu");

        if (Globals.StoryFlags.Contains("Met Lyla"))
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
        if (Globals.StoryFlags.Contains("Met Lyla") && !_openedMenu && _dialogueManager._doneTalking)
        {
            _openedMenu = true;
            GameObject.FindWithTag("UI").transform.Find("Investigation").gameObject.SetActive(true);
        }

        if (Globals.StoryFlags.Contains("Ready For Court") && _dialogueManager._doneTalking && !_begunEnd)
        {
            Destroy(GameObject.FindGameObjectWithTag("UI").transform.GetComponentInChildren<CourtRecordController>().gameObject);
            GameObject obj = Instantiate(_fadeOut);
            SceneTransition trans = obj.GetComponent<SceneTransition>();
            trans._speed = 0.5f;
            trans.TBC = true;
            trans._destination = "Lobby";
            _begunEnd = true;
        }
    }
}
