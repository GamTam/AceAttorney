using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TrialController : MonoBehaviour
{
    [SerializeField] private DialogueSO currentTrial;
    [SerializeField] private DialogueSO[] allDialogues;
    [SerializeField] private int maxPenalties = 5;

    [SerializeField] private TextMeshProUGUI penaltiesText;

    private int currentPenalties = 0;
    private DialogueManager dialogueManager;
    private CrossExamination crossExamination;

    private PlayerInput playerInput;

    private void Start() {
        dialogueManager = FindObjectOfType<DialogueManager>();
        crossExamination = FindObjectOfType<CrossExamination>();

        penaltiesText.text = $"{currentPenalties}/{maxPenalties}";

        playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Textbox");
    }

    public void StartTrial() {
        currentPenalties = 0;
        penaltiesText.text = $"{currentPenalties}/{maxPenalties}";
        
        dialogueManager.StartText(currentTrial);
    }

    public void IncreaseIncorrects() {
        currentPenalties++;
        penaltiesText.text = $"{currentPenalties}/{maxPenalties}";

        if (currentPenalties >= maxPenalties) {
            print("AGHHHHH!!!!");
        }
    }

    public DialogueSO[] ReturnAllDialogues() {
        return allDialogues;
    }
}