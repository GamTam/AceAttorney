﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject _textBoxPrefab;
    private GameObject _tempBox;
    private GameObject _advanceButton;
    private TMP_Text textBox;
    private TMP_Text _nameBox;
    private Queue<string> lines;
    private PlayerInput _playerInput;
    private string _prevActionMap;
    private string _typingClip = "blipmale";

    private DialogueVertexAnimator dialogueVertexAnimator;
    private bool movingText;
    private InputAction _advanceText;
    private SoundManager _soundManager;

    // Izzy
    private bool isThisDialoguePresentable;
    private Queue<bool> isPresentable;

    void Awake() {
        lines = new Queue<string>();
        isPresentable = new Queue<bool>();
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _advanceText = _playerInput.actions["Advance"];
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    public void StartText(String[] linesIn)
    {
        _tempBox = Instantiate(_textBoxPrefab);
        _tempBox.transform.SetParent(GameObject.FindWithTag("UI").transform, false);
        
        TMP_Text[] texts = _tempBox.GetComponentsInChildren<TMP_Text>();
        _advanceButton = _tempBox.GetComponentInChildren<Animator>().gameObject;

        textBox = texts[0];
        _nameBox = texts[1];
        _advanceButton.SetActive(false);
        dialogueVertexAnimator = new DialogueVertexAnimator(textBox);

        _prevActionMap = _playerInput.currentActionMap.name;
        _playerInput.SwitchCurrentActionMap("TextBox");
        lines.Clear();
        
        foreach (string line in linesIn)
        {
            lines.Enqueue(line);
        }
        
        NextLine(true);
    }

    public void StartTextSO(DialogueSO[] dialogues) {
        _tempBox = Instantiate(_textBoxPrefab);
        _tempBox.transform.SetParent(GameObject.FindWithTag("UI").transform, false);

        TMP_Text[] texts = _tempBox.GetComponentsInChildren<TMP_Text>();
        _advanceButton = _tempBox.GetComponentInChildren<Animator>().gameObject;

        textBox = texts[0];
        _nameBox = texts[1];
        _advanceButton.SetActive(false);
        dialogueVertexAnimator = new DialogueVertexAnimator(textBox);

        _prevActionMap = _playerInput.currentActionMap.name;
        _playerInput.SwitchCurrentActionMap("TextBox");
        lines.Clear();

        foreach (DialogueSO dialogue in dialogues)
        {
            lines.Enqueue(dialogue.dialogueText);
            isPresentable.Enqueue(dialogue.isPresentable);
            // currentDialogue = dialogue;
        }

        NextLine(true);
    }

    public bool GetCurrentPresentablity() {
        return isThisDialoguePresentable;
    }

    private void Update()
    {
        if (_advanceText.triggered)
        {
            NextLine();
        }

        if (!(dialogueVertexAnimator == null))
        {
            if (!dialogueVertexAnimator.textAnimating)
            {
                _advanceButton.SetActive(true);
            }
        }
    }

    private Coroutine typeRoutine = null;
    public void NextLine(bool firstTime = false) {
        if (dialogueVertexAnimator.textAnimating)
        {
            dialogueVertexAnimator.QuickEnd();
            return;
        }
        
        if (!firstTime) _soundManager.Play("textboxAdvance");
        
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (movingText)
        {
            return;
        }
        
        this.EnsureCoroutineStopped(ref typeRoutine);
        dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands =
            DialogueUtility.ProcessInputString(lines.Dequeue(), out string totalTextMessage);
        TextAlignOptions[] textAlignInfo = SeparateOutTextAlignInfo(commands);
        String nameInfo = SeparateOutNameInfo(commands);
        String soundInfo = SeparateOutSoundInfo(commands);

        for (int i = 0; i < textAlignInfo.Length; i++)
        {
            TextAlignOptions info = textAlignInfo[i];
            if (info == TextAlignOptions.topCenter)
            {
                textBox.alignment = TextAlignmentOptions.Top;
            }
            else if (info == TextAlignOptions.midCenter)
            {
                textBox.alignment = TextAlignmentOptions.Center;
            }
            else if (info == TextAlignOptions.left)
            {
                textBox.alignment = TextAlignmentOptions.TopLeft;
            }
            else if (info == TextAlignOptions.right)
            {
                textBox.alignment = TextAlignmentOptions.TopRight;
            }
        }
        
        if (nameInfo != null) _nameBox.text = nameInfo;
        if (soundInfo != null) _typingClip = soundInfo;
        
        Debug.Log(_typingClip);

        _advanceButton.SetActive(false);
        typeRoutine =
            StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, _typingClip, null));
    }
    
    private TextAlignOptions[] SeparateOutTextAlignInfo(List<DialogueCommand> commands) {
        List<TextAlignOptions> tempResult = new List<TextAlignOptions>();
        for (int i = 0; i < commands.Count; i++) {
            DialogueCommand command = commands[i];
            if (command.type == DialogueCommandType.Align) {
                tempResult.Add(command.textAlignOptions);
            }
        }
        return tempResult.ToArray();
    }
    
    private String SeparateOutNameInfo(List<DialogueCommand> commands) {
        for (int i = 0; i < commands.Count; i++) {
            DialogueCommand command = commands[i];
            if (command.type == DialogueCommandType.Name) {
                return command.stringValue;
            }
        }
        return null;
    }
    
    private String SeparateOutSoundInfo(List<DialogueCommand> commands) {
        for (int i = 0; i < commands.Count; i++) {
            DialogueCommand command = commands[i];
            if (command.type == DialogueCommandType.Sound) {
                return command.stringValue;
            }
        }
        return null;
    }

    void EndDialogue()
    {
        StartCoroutine(dialogueVertexAnimator.AnimateTextIn(new List<DialogueCommand>(), "", _typingClip, null));
        Destroy(_tempBox);
        _playerInput.SwitchCurrentActionMap(_prevActionMap);
    }
}
