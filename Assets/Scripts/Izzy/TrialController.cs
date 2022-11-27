using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TrialController : MonoBehaviour
{
    [SerializeField] private DialogueSO currentTrial;
    [SerializeField] private int maxIncorrects = 5;

    [SerializeField] private TextMeshProUGUI currentIncorrectsText;
    [SerializeField] private TextMeshProUGUI maxIncorrectsText;

    private int currentIncorrects = 0;
    private DialogueManager dialogueManager;
    private CrossExamination crossExamination;

    private PlayerInput playerInput;
    private InputAction presentingEvidence;

    private void Start() {
        dialogueManager = FindObjectOfType<DialogueManager>();
        crossExamination = FindObjectOfType<CrossExamination>();

        currentIncorrectsText.text = currentIncorrects.ToString();
        maxIncorrectsText.text = maxIncorrects.ToString();

        playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Textbox");
        presentingEvidence = playerInput.actions["Textbox/PresentEvidence"];
    }

    private void Update() {
        if (presentingEvidence.triggered) {
            PresentEvidence();
        }
    }

    public void StartTrial() {
        currentIncorrects = 0;
        currentIncorrectsText.text = currentIncorrects.ToString();
        
        dialogueManager.StartText(currentTrial);
    }

    public void PresentEvidence() {
        crossExamination.StartCrossExamination();
    }

    public void IncreaseIncorrects() {
        currentIncorrects++;
        currentIncorrectsText.text = currentIncorrects.ToString();

        if (currentIncorrects >= maxIncorrects) {
            print("AGHHHHH!!!!");
        }
    }
}