using UnityEngine;
using UnityEngine.InputSystem;

public class Saving : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private TrialController trialController;
    private MusicManager _musicManager;

    private DialogueSO currentDialogue;
    private DialogueSO[] allDialogues;
    
    private PlayerInput playerInput;
    private InputAction savingKey;
    private InputAction loadingKey;

    private void Start() {
        dialogueManager = FindObjectOfType<DialogueManager>();
        trialController = FindObjectOfType<TrialController>();
        _musicManager = FindObjectOfType<MusicManager>();
        
        allDialogues = trialController.ReturnAllDialogues();

        playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Textbox");
        savingKey = playerInput.actions["Textbox/Saving"];
        loadingKey = playerInput.actions["Textbox/Loading"];
    }

    private void Update() {
        if (savingKey.triggered) SaveGame();
        if (loadingKey.triggered) LoadGame();
    }

    public void SaveGame() {
        currentDialogue = dialogueManager.ReturnCurrentDialogue();
        PlayerPrefs.SetInt("currentDialogueIndex", currentDialogue.dialogueIndex);
        PlayerPrefs.SetInt("currentLine", dialogueManager._currentLine);
        PlayerPrefs.SetString("musicPlaying", _musicManager.musicPlaying.name);
    }

    public void LoadGame() {
        var savedDialogueIndex = PlayerPrefs.GetInt("currentDialogueIndex");
        string s = PlayerPrefs.GetString("musicPlaying");
        if (_musicManager.GetMusicPlaying().name != s)
        {
            _musicManager.fadeOut();
            _musicManager.Play(s);
        }

        foreach (DialogueSO dialogue in allDialogues) {
            if (dialogue.dialogueIndex == savedDialogueIndex) {
                dialogueManager.StartText(dialogue, true, PlayerPrefs.GetInt("currentLine"));
            }
        }
    }
}