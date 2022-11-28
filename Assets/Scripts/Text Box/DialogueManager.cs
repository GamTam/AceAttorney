using System;
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
    [HideInInspector] public GameObject _tempBox;
    private Animator _advanceButton;
    private TMP_Text textBox;
    private TMP_Text _nameBox;
    private Queue<TBLine> lines;
    private PlayerInput _playerInput;
    private string _prevActionMap;
    private string _typingClip = "blipmale";

    [HideInInspector] public DialogueVertexAnimator dialogueVertexAnimator;
    private bool movingText;
    private InputAction _advanceText;
    private MusicManager _musicManager;
    private SoundManager _soundManager;

    public ResponseHandler _responseHandler;
    private DialogueSO _dialogue;
    private bool _shownResponses;

    private Animator _char;
    private Animator _prevChar;
    private string _currentAnim;

    [SerializeField] private SwapCharacters _swap;
    private bool _startedText = true;
    private bool _skipFade;

    private bool _crossEx;

    [SerializeField] private Texture[] _interjections;
    [SerializeField] private GameObject _interjectionObj;

    // Izzy
    private CrossExamination crossExamination;

    void Awake() {
        lines = new Queue<TBLine>();
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _advanceText = _playerInput.actions["Advance"];
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();

        crossExamination = FindObjectOfType<CrossExamination>();
    }

    public void StartText(DialogueSO linesIn)
    {
        _crossEx = linesIn.isCrossExamination;
        _shownResponses = false;

        if (_tempBox != null)
        {
            Destroy(_tempBox.gameObject);
        }

        try
        {
            _char = GameObject.Find(_swap._charName).GetComponent<Animator>();
            _prevChar = _char;
        } catch {}

        _tempBox = Instantiate(_textBoxPrefab);
        _tempBox.transform.SetParent(GameObject.FindWithTag("UI").transform, false);

        _dialogue = linesIn;
        
        TMP_Text[] texts = _tempBox.GetComponentsInChildren<TMP_Text>();
        _advanceButton = _tempBox.GetComponentsInChildren<Animator>()[1];

        textBox = texts[1];
        _nameBox = texts[0];
        _advanceButton.gameObject.SetActive(false);
        Debug.Log(_advanceButton.name);
        dialogueVertexAnimator = new DialogueVertexAnimator(textBox);
        dialogueVertexAnimator._parent = this;

        _prevActionMap = _playerInput.currentActionMap.name;
        _playerInput.SwitchCurrentActionMap("TextBox");
        lines.Clear();
        
        foreach (TBLine line in linesIn.dialogueText)
        {
            lines.Enqueue(line);
        }
        
        NextLine(true);
    }

    private void Update()
    {
        if (!(dialogueVertexAnimator == null) && !(_dialogue == null) && _startedText)
        {
            if (_advanceText.triggered)
            {
                NextLine();
            }
            else if (!dialogueVertexAnimator.textAnimating && lines.Count == 0 && _dialogue.HasResponses && !_shownResponses)
            {
                NextLine();
            }
            
            if (!dialogueVertexAnimator.textAnimating && !_advanceButton.gameObject.activeSelf)
            {
                if (_char != null) _char.Play($"{_currentAnim}_idle");
                if (lines.Count == 0 && _dialogue.HasResponses) return;
                _advanceButton.gameObject.SetActive(true);
                if (_crossEx)
                {
                    _advanceButton.Play("Idle_Cross");
                    if (_dialogue.nextLine.name == "Loop")
                    {
                        _advanceButton.transform.Find("Forwards").gameObject.GetComponent<Image>().enabled = false;
                    }
                    else
                    {
                        _advanceButton.transform.Find("Forwards").gameObject.GetComponent<Image>().enabled = true;
                    }

                    if (_dialogue.prevLine == null)
                    {
                        _advanceButton.transform.Find("Backwards").gameObject.GetComponent<Image>().enabled = false;
                    }
                    else
                    {
                        _advanceButton.transform.Find("Backwards").gameObject.GetComponent<Image>().enabled = true;
                    }
                }
                else
                {
                    _advanceButton.transform.Find("Backwards").gameObject.GetComponent<Image>().enabled = false;
                    _advanceButton.transform.Find("Forwards").gameObject.GetComponent<Image>().enabled = true;
                    
                    _advanceButton.Play("Idle");
                }
            }
        }
    }

    public DialogueSO ReturnCurrentDialogue() {
        return _dialogue;
    }

    private Coroutine typeRoutine = null;
    public void NextLine(bool firstTime = false)
    {
        if (_shownResponses) return;

        if (dialogueVertexAnimator.textAnimating)
        {
            dialogueVertexAnimator.QuickEnd();
            return;
        }

        if (!firstTime)
        {
            if (_crossEx)
            {
                _soundManager.Play("confirm");   
            }
            else
            {
                _soundManager.Play("textboxAdvance");
            }
        }
        
        if (lines.Count == 0)
        {
            if (_dialogue.HasNextLine) {
                StartText(_dialogue.nextLine);
            }
            else if (_dialogue.HasResponses)
            {
                _shownResponses = true;
                _responseHandler.ShowResponses(_dialogue.responses);
            }
            else
            {
                EndDialogue();
            }
            return;
        }

        if (movingText)
        {
            return;
        }

        _skipFade = false;
        TBLine line = lines.Dequeue();
        this.EnsureCoroutineStopped(ref typeRoutine);
        dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands =
            DialogueUtility.ProcessInputString(line.Dialogue, out string totalTextMessage);
        TextAlignOptions[] textAlignInfo = SeparateOutTextAlignInfo(commands);
        String nameInfo = line.KnownName ? line.Name : "???";
        String soundInfo = line.BlipSound;
        String faceInfo = SeparateOutFaceInfo(commands);
        String emotionInfo = SeparateOutEmotionInfo(commands);
        Interjection interjection = line.Interjection;
        _skipFade = line.SkipFade;

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

        if (line.StopMusic) _musicManager.Stop();
        
        if (nameInfo != null) _nameBox.text = nameInfo;
        if (soundInfo != null) _typingClip = soundInfo;
        
        if (faceInfo != null)
        {
            try
            {
                _char = GameObject.Find(faceInfo).GetComponent<Animator>();
            }
            catch
            {
                _char = null;
            }
        }
        
        if (emotionInfo != null) _currentAnim = emotionInfo;
        
        StartCoroutine(StartText(commands, totalTextMessage, faceInfo, interjection));
    }

    private IEnumerator StartText(List<DialogueCommand> commands, string totalTextMessage, string faceInfo, Interjection interjection)
    {
        _startedText = false;
        GameObject obj = null;
        RawImage img;

        bool skip = false;
        switch (interjection)
        {
            case Interjection.Objection:
                obj = Instantiate(_interjectionObj);
                obj.transform.SetParent(GameObject.FindWithTag("UI").transform, false);
                img = obj.GetComponent<RawImage>();
                img.texture = _interjections[0];
                img.SetNativeSize();
                _soundManager.Play($"objection{_nameBox.text}");
                break;
            case Interjection.HoldIt:
                obj = Instantiate(_interjectionObj);
                obj.transform.SetParent(GameObject.FindWithTag("UI").transform, false);
                img = obj.GetComponent<RawImage>();
                img.texture = _interjections[1];
                img.SetNativeSize();
                _soundManager.Play($"holdIt{_nameBox.text}");
                break;
            case Interjection.TakeThat:
                obj = Instantiate(_interjectionObj);
                obj.transform.SetParent(GameObject.FindWithTag("UI").transform, false);
                img = obj.GetComponent<RawImage>();
                img.texture = _interjections[2];
                img.SetNativeSize();
                _soundManager.Play($"takeThat{_nameBox.text}");
                break;
            case Interjection.NA:
                skip = true;
                break;
        }

        if (!skip)
        {
            _tempBox.SetActive(false);
            yield return new WaitForSeconds(1);
            _tempBox.SetActive(true);
            Destroy(obj);
        }
        
        if (_prevChar != _char && (_char != null || faceInfo == "NaN"))
        {
            _prevChar = _char;
            _swap.StartSwap(faceInfo, fadeIn:faceInfo != "NaN", skipFade:_skipFade);
            if (!_skipFade) _tempBox.SetActive(false);
            
            while (!_swap._done)
            {
                yield return null;
            }

            if (!_skipFade)
            {
                yield return new WaitForSeconds(0.25f);
                _tempBox.SetActive(true);
            }
        }
        
        _advanceButton.gameObject.SetActive(false);
        if (_char != null) _char.Play($"{_currentAnim}_talk");
        typeRoutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, _typingClip, null));
        _startedText = true;
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
    
    private String SeparateOutFaceInfo(List<DialogueCommand> commands) {
        for (int i = 0; i < commands.Count; i++) {
            DialogueCommand command = commands[i];
            if (command.type == DialogueCommandType.Speaker) {
                return command.stringValue;
            }
        }
        return null;
    }
    
    private String SeparateOutEmotionInfo(List<DialogueCommand> commands) {
        for (int i = 0; i < commands.Count; i++) {
            DialogueCommand command = commands[i];
            if (command.type == DialogueCommandType.Emotion) {
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
