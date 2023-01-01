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
    [SerializeField] private List<EvidenceSO> _profiles;
    [SerializeField] private TrialController _trial;
    [SerializeField] private CrossExamination _crossExamination;
    [SerializeField] private GameObject _penaltyUI;
    [SerializeField] private GameObject _fadeOut;
    
    private PlayerInput _playerInput;
    private DialogueManager _dialogueManager;
    private MusicManager _musicManager;
    
    private bool _startedCrossEx;
    private bool _endedCrossEx;
    private bool _boCrossEx;
    private bool _boCrossExEnd;
    private bool _hermanCrossEx;
    private bool _hermanCrossExEnd;
    private bool _hermanCrossEx2;
    private bool _hermanCrossExEnd2;
    private bool _openedMenu;

    private void Start()
    {
        if (Globals.Evidence.Count == 0)
        {
            Globals.Evidence = _evidence;
        }

        Globals.Profiles = _profiles;
        
        _dialogueManager = GameObject.FindGameObjectWithTag("Dialogue Manager").GetComponent<DialogueManager>();
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _dialogueManager.StartText(_dialogue);
    }

    private void Update()
    {
        if (Globals.StoryFlags.Contains("We did it, Reddit") && _dialogueManager._doneTalking && !_openedMenu)
        {
            _musicManager.fadeOut(2);
            _openedMenu = true;
            GameObject obj = Instantiate(_fadeOut);
            SceneTransition trans = obj.GetComponent<SceneTransition>();
            trans._destination = "Lobby";
            trans._speed = 1;
        }
        
        if (Globals.StoryFlags.Contains("Herman CrossEx 2 Start") && !_hermanCrossEx2)
        {
            _musicManager.Play("CrossExFast");
            _hermanCrossEx2 = true;
            _penaltyUI.SetActive(true);
            _trial.enabled = true;
            _crossExamination.enabled = true;
        }
        
        if (Globals.StoryFlags.Contains("Herman CrossEx 2 End") && !_hermanCrossExEnd2)
        {
            _hermanCrossExEnd2 = true;
            _penaltyUI.SetActive(false);
            _trial.enabled = false;
            _crossExamination.enabled = false; 
        }
    
        if (Globals.StoryFlags.Contains("Herman CrossEx Start") && !_hermanCrossEx)
        {
            _musicManager.Play("CrossExFast");
            _hermanCrossEx = true;
            _penaltyUI.SetActive(true);
            _trial.enabled = true;
            _crossExamination.enabled = true;
        }
        
        if (Globals.StoryFlags.Contains("Herman CrossEx End") && !_hermanCrossExEnd)
        {
            _hermanCrossExEnd = true;
            _penaltyUI.SetActive(false);
            _trial.enabled = false;
            _crossExamination.enabled = false;
        }
        
        if (Globals.StoryFlags.Contains("Bo CrossEx Start") && !_boCrossEx)
        {
            _musicManager.Play("CrossEx");
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
            _musicManager.Play("CrossEx");
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
