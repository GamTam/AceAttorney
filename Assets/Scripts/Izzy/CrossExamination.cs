using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class CrossExamination : MonoBehaviour 
{
    [SerializeField] GameObject _courtRecord;
    
    private EvidenceSO _selectedEvidence;
    private bool _presenting;

    private SoundManager _soundManager;
    
    private TrialController trialController;
    private DialogueManager dialogueManager;
    private DialogueSO currentDialogue;

    private PlayerInput playerInput;
    private InputAction present;
    private InputAction pressing;
    private InputAction previousLine;
    private InputAction nextLine;

    private void Start() {
        dialogueManager = FindObjectOfType<DialogueManager>();
        trialController = FindObjectOfType<TrialController>();

        playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Textbox");
        pressing = playerInput.actions["Textbox/Press"];
        present = playerInput.actions["Textbox/Court Record"];
        previousLine = playerInput.actions["Textbox/PreviousLine"];
        nextLine = playerInput.actions["Textbox/NextLine"];
        
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    private void Update() {
        currentDialogue = dialogueManager.ReturnCurrentDialogue();
        if (currentDialogue == null || _presenting) return;

        if (pressing.triggered) {
            Press();
        }

        if (present.triggered)
        {
            StartCoroutine(Present());
        }

        if (currentDialogue.isCrossExamination)
        {
            if (nextLine.triggered && currentDialogue.nextLine != null &&
                !dialogueManager.dialogueVertexAnimator.textAnimating)
            {
                _soundManager.Play("confirm");
                dialogueManager.StartText(currentDialogue.nextLine);
            }

            if (previousLine.triggered && currentDialogue.prevLine != null &&
                !dialogueManager.dialogueVertexAnimator.textAnimating)
            {
                _soundManager.Play("confirm");
                dialogueManager.StartText(currentDialogue.prevLine);
            }
        }
    }

    public IEnumerator Present()
    {
        _selectedEvidence = null;
        _presenting = true;
        var correctEvidenceName = currentDialogue.ReturnListOfEvidence();
        GameObject obj = Instantiate(_courtRecord, GameObject.FindGameObjectsWithTag("UI")[1].transform, false);
        CourtRecordController cr = obj.GetComponent<CourtRecordController>();
        obj.GetComponent<CRCrossEx>().enabled = true;
        
        playerInput.SwitchCurrentActionMap("Menu");
        cr.HasPresented += UpdateEvidence;

        while (_selectedEvidence == null)
        {
            if (obj == null)
            {
                _presenting = false;
                yield break;
            }

            yield return null;
        }

        if (correctEvidenceName.Contains(_selectedEvidence))
        {
            CorrectEvidenceShown();
        }
        else
        {
            IncorrectEvidenceShown();
        }
        
        _presenting = false;
    }

    void UpdateEvidence(EvidenceSO evidence)
    {
        _selectedEvidence = evidence;
    }

    private void CorrectEvidenceShown() {
        if (currentDialogue.HasPresentSequence) {
            dialogueManager.StartText(currentDialogue.presentSequence);
        }
    }

    private void IncorrectEvidenceShown() {
        if (currentDialogue.HasWrongPresentSequence)
        {
            currentDialogue.wrongPresentSequence.nextLine = currentDialogue;
            dialogueManager.StartText(currentDialogue.wrongPresentSequence);
            trialController.IncreaseIncorrects();
        }
    }

    private void Press() {
        Debug.Log("a");
        if (currentDialogue.HasPressingSequence) {
            dialogueManager.StartText(currentDialogue.pressSequence);
        }
    }
}