using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrialController : MonoBehaviour
{
    [SerializeField] private TrialSO currentTrial;
    [SerializeField] private GameObject scuffedEvidenceUI;
    [SerializeField] private GameObject scuffedWrongEvidenceUI;
    private DialogueManager dialogueManager;

    private PlayerInput playerInput;
    private InputAction presentingEvidence;
    private InputAction wrongEvidence;

    private void Start() {
        dialogueManager = FindObjectOfType<DialogueManager>();
        scuffedEvidenceUI.SetActive(false);
        scuffedWrongEvidenceUI.SetActive(false);

        playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Textbox");
        presentingEvidence = playerInput.actions["Textbox/PresentEvidence"];
        wrongEvidence = playerInput.actions["Textbox/WrongEvidence"];
    }

    private void Update() {
        if (presentingEvidence.triggered) {
            StartCoroutine(PresentEvidence());
        }
        if (wrongEvidence.triggered) {
            StartCoroutine(WrongEvidenceShown());
        }
    }

    public void StartTrial() {
        dialogueManager.StartTextSO(currentTrial.listOfDialogues);
    }

    public IEnumerator PresentEvidence() {
        scuffedEvidenceUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        scuffedEvidenceUI.SetActive(false);
    }

    public IEnumerator WrongEvidenceShown() {
        scuffedWrongEvidenceUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        scuffedWrongEvidenceUI.SetActive(false);
    }
}