using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagsCourtRoom : MonoBehaviour
{
    [SerializeField] private DialogueSO _dialogue;
    [SerializeField] private List<EvidenceSO> _evidence;
    [SerializeField] private TrialController _trial;
    [SerializeField] private CrossExamination _crossExamination;
    [SerializeField] private GameObject _penaltyUI;
    
    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    private MusicManager _musicManager;
    
    private bool _startedCrossEx;
    private bool _endedCrossEx;
    private bool _boCrossEx;
    private bool _boCrossExEnd;

    private void Start()
    {
        if (Globals.Evidence.Count == 0)
        {
            Globals.Evidence = _evidence;
        }
        
        _dialogueManager = GameObject.FindGameObjectWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _dialogueManager.StartText(_dialogue);
    }

    private void Update()
    {
        if (Globals.StoryFlags.Contains("Bo CrossEx Start") && !_boCrossEx)
        {
            _boCrossEx = true;
            _penaltyUI.SetActive(true);
            _trial.enabled = true;
            _crossExamination.enabled = true;
        }
        
        if (Globals.StoryFlags.Contains("Bo CrossEx End") && !_boCrossExEnd)
        {
            _boCrossExEnd = true;
            _penaltyUI.SetActive(false);
            _trial.enabled = false;
            _crossExamination.enabled = false;
        }

        if (Globals.StoryFlags.Contains("Gumshoe CrossEx Start") && !_startedCrossEx)
        {
            _penaltyUI.SetActive(true);
            _startedCrossEx = true;
            _trial.enabled = true;
            _crossExamination.enabled = true;
        }
        
        if (Globals.StoryFlags.Contains("Gumshoe CrossEx End") && !_endedCrossEx)
        {
            _penaltyUI.SetActive(false);
            _endedCrossEx = true;
            _trial.enabled = false;
            _crossExamination.enabled = false;
        }
    }
}
