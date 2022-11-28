using UnityEngine;
using UnityEngine.InputSystem;

public class CrossExamination : MonoBehaviour 
{
    [SerializeField] EvidenceSO presentedEvidence; // This will change when we have actual evidence presentation
    
    private SoundManager _soundManager;
    
    private TrialController trialController;
    private DialogueManager dialogueManager;
    private DialogueSO currentDialogue;

    private PlayerInput playerInput;
    private InputAction pressing;
    private InputAction previousLine;
    private InputAction nextLine;

    private void Start() {
        dialogueManager = FindObjectOfType<DialogueManager>();
        trialController = FindObjectOfType<TrialController>();

        playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Textbox");
        pressing = playerInput.actions["Textbox/Press"];
        previousLine = playerInput.actions["Textbox/PreviousLine"];
        nextLine = playerInput.actions["Textbox/NextLine"];
        
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    private void Update() {
        currentDialogue = dialogueManager.ReturnCurrentDialogue();
        if (currentDialogue == null) return;

        if (pressing.triggered) {
            Press();
        }

        if (currentDialogue.HasPressingSequence)
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

    public void StartCrossExamination() {
        var correctEvidenceName = currentDialogue.ReturnListOfEvidence();

        foreach (EvidenceSO evidence in correctEvidenceName) {
            if (presentedEvidence.evidenceName == evidence.evidenceName) {
                CorrectEvidenceShown();
            }
            else {
                IncorrectEvidenceShown();
            }
        }
    }

    private void CorrectEvidenceShown() {
        if (currentDialogue.HasPresentSequence) {
            dialogueManager.StartText(currentDialogue.presentSequence);
        }
        else return;
    }

    private void IncorrectEvidenceShown() {
        if (currentDialogue.HasWrongPresentSequence) {
            dialogueManager.StartText(currentDialogue.wrongPresentSequence);
            trialController.IncreaseIncorrects();
        }
        else return;
    }

    private void Press() {
        if (currentDialogue.HasPressingSequence) {
            dialogueManager.StartText(currentDialogue.pressSequence);
        }
        else return;

        //dialogueManager.StartText(currentDialogue.pressSequence);
        //print(currentDialogue.dialogueText[0]);
    }
}