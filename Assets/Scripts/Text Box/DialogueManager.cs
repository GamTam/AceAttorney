using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject _textBoxPrefab;
    [SerializeField] private GameObject _courtRecordPrefab;
    private GameObject _tempCourtRecord;
    [HideInInspector] public GameObject _tempBox;
    private Animator _advanceButton;
    private TMP_Text textBox;
    private TMP_Text _nameBox;
    private List<TBLine> lines;
    [HideInInspector] public int _currentLine;
    private PlayerInput _playerInput;
    [HideInInspector] public string _prevActionMap;
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
    private bool _autoEnd;
    
    [SerializeField] private GameObject _interjectionObj;

    public bool _doneTalking;

    void Awake() {
        lines = new List<TBLine>();
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _advanceText = _playerInput.actions["Advance"];
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
    }

    public void StartText(DialogueSO linesIn, bool quickEnd = false, int startingLine = 0, string prevActionMap = null)
    {
        _doneTalking = false;
        _crossEx = linesIn.isCrossExamination;
        _shownResponses = false;
        _currentLine = startingLine;

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
        dialogueVertexAnimator = new DialogueVertexAnimator(textBox);
        dialogueVertexAnimator._parent = this;

        if (prevActionMap == null)
        {
            try
            {
                _prevActionMap = _playerInput.currentActionMap.name;
            }
            catch
            {
                _prevActionMap = "Menu";
            }
        }
        else
        {
            _prevActionMap = prevActionMap;
        }
        
        _playerInput.SwitchCurrentActionMap("TextBox");
        lines.Clear();

        lines = linesIn.dialogueText.ToList();
        
        NextLine(true, quickEnd);
    }

    private void Update()
    {
        if (!(dialogueVertexAnimator == null) && !(_dialogue == null) && _startedText)
        {
            if (_advanceText.triggered)
            {
                NextLine();
            }
            else if (!dialogueVertexAnimator.textAnimating &&
                     (_currentLine == lines.Count && _dialogue.HasResponses && !_shownResponses) || _autoEnd)
            {
                if (!dialogueVertexAnimator.textAnimating)
                {
                    NextLine();   
                }
            }
            
            if (!dialogueVertexAnimator.textAnimating && !_advanceButton.gameObject.activeSelf)
            {
                if (_char != null) _char.Play($"{_currentAnim}_idle");
                if (_currentLine == lines.Count && _dialogue.HasResponses) return;
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
    public void NextLine(bool firstTime = false, bool quickEnd = false)
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
        
        if (_currentLine == lines.Count)
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
                StartCoroutine(EndDialogue());
                _doneTalking = true;
            }
            return;
        }

        if (movingText)
        {
            return;
        }

        _skipFade = false;
        TBLine line = lines[_currentLine];
        _currentLine += 1;
        this.EnsureCoroutineStopped(ref typeRoutine);
        dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands =
            DialogueUtility.ProcessInputString(line.Dialogue, out string totalTextMessage);
        TextAlignOptions[] textAlignInfo = SeparateOutTextAlignInfo(commands);
        String nameInfo = line.KnownName ? line.Name : "???";
        String soundInfo = line.BlipSound;
        String faceInfo = line.Char;
        String emotionInfo = line.Anim;
        Interjection interjection = line.Interjection;
        _skipFade = line.SkipFade;

        _autoEnd = line.AutoEnd;

        StateChange state = line.StateChange;
        if (state.StoryFlag != null) if (!Globals.StoryFlags.Contains(state.StoryFlag)) Globals.StoryFlags.Add(state.StoryFlag);
        if (state.EvidenceToAdd != null) if (!Globals.Evidence.Contains(state.EvidenceToAdd)) Globals.Evidence.Add(state.EvidenceToAdd);
        if (state.EvidenceToRemove != null) if (Globals.Evidence.Contains(state.EvidenceToRemove)) Globals.Evidence.Remove(state.EvidenceToRemove);
        if (state.PersonToAdd != null) if (!Globals.Profiles.Contains(state.PersonToAdd)) Globals.Profiles.Add(state.PersonToAdd);

        bool addToCourtRecord = line.AddToCourtRecord;

        TextAlignOptions options = line.Align;
        
        if (options == TextAlignOptions.topCenter)
        {
            textBox.alignment = TextAlignmentOptions.Top;
        }
        else if (options == TextAlignOptions.midCenter)
        {
            textBox.alignment = TextAlignmentOptions.Center;
        }
        else if (options == TextAlignOptions.left)
        {
            textBox.alignment = TextAlignmentOptions.TopLeft;
        }
        else if (options == TextAlignOptions.right)
        {
            textBox.alignment = TextAlignmentOptions.TopRight;
        }

        if (line.StopMusic) _musicManager.fadeOut();
        
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
        if (_char != null) _char.Play($"{_currentAnim}_idle");
        
        _nameBox.text = nameInfo;

        if (quickEnd)
        {
            typeRoutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, null, null));
            _swap.StartSwap(faceInfo, skipFade:true);
            dialogueVertexAnimator.QuickEnd();
            return;
        }
        StartCoroutine(StartText(commands, totalTextMessage, line.Name, faceInfo, interjection, addToCourtRecord, line.Thinking, line.ForceFade));
    }

    private IEnumerator StartText(List<DialogueCommand> commands, string totalTextMessage, string name, string faceInfo, Interjection interjection, bool addToCourtRecord, bool Thinking, bool forceFade)
    {
        if (_tempCourtRecord != null) _tempCourtRecord.GetComponent<Animator>().Play("Fade Out");
        while (_tempCourtRecord != null)
        {
            yield return null;
        }
        
        _startedText = false;
        GameObject obj = null;
        RawImage img;
        int c = GameObject.FindGameObjectsWithTag("UI").Length;

        bool skip = false;
        switch (interjection)
        {
            case Interjection.Objection:
                obj = Instantiate(_interjectionObj);
                obj.transform.SetParent(GameObject.FindGameObjectsWithTag("UI")[c - 1].transform, false);
                img = obj.GetComponent<RawImage>();
                img.texture = Resources.Load<Texture>("Sprites/Interjections/Objection");
                img.SetNativeSize();
                if (_soundManager.Play($"objection{_nameBox.text}") == null)
                {
                    _soundManager.Play("interjection");
                }

                break;
            case Interjection.HoldIt:
                obj = Instantiate(_interjectionObj);
                obj.transform.SetParent(GameObject.FindGameObjectsWithTag("UI")[c - 1].transform, false);
                img = obj.GetComponent<RawImage>();
                img.texture = Resources.Load<Texture>("Sprites/Interjections/Hold It");
                img.SetNativeSize();
                if (_soundManager.Play($"holdIt{_nameBox.text}") == null)
                {
                    _soundManager.Play("interjection");
                }
                
                break;
            case Interjection.TakeThat:
                obj = Instantiate(_interjectionObj);
                obj.transform.SetParent(GameObject.FindGameObjectsWithTag("UI")[c - 1].transform, false);
                img = obj.GetComponent<RawImage>();
                img.texture = Resources.Load<Texture>("Sprites/Interjections/Take That");
                img.SetNativeSize();
                if (_soundManager.Play($"takeThat{_nameBox.text}") == null)
                {
                    _soundManager.Play("interjection");
                }
                
                break;
            case Interjection.NA:
                skip = true;
                break;
        }

        if (!skip)
        {
            _tempBox.SetActive(false);
            yield return new WaitForSeconds(1);
            Destroy(obj);
        }
        
        if (_prevChar != _char && (_char != null || faceInfo == "NaN") || forceFade)
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
            }
        }
        
        _tempBox.SetActive(true);

        _advanceButton.gameObject.SetActive(false);
        if (_char != null && name == faceInfo && !Thinking) _char.Play($"{_currentAnim}_talk");
        
        if (addToCourtRecord)
        {
            _soundManager.Play("court record");
            _tempCourtRecord = Instantiate(_courtRecordPrefab, GameObject.FindWithTag("UI").transform, false);

            _tempCourtRecord.GetComponentsInChildren<Image>()[3].sprite = Globals.Evidence[Globals.Evidence.Count - 1].Icon;
            _tempCourtRecord.GetComponentsInChildren<Image>()[3].SetNativeSize();
            
            _tempCourtRecord.gameObject.GetComponentsInChildren<TMP_Text>()[0].SetText(Globals.Evidence[Globals.Evidence.Count - 1].Name);
            _tempCourtRecord.gameObject.GetComponentsInChildren<TMP_Text>()[1].SetText(Globals.Evidence[Globals.Evidence.Count - 1].Description);
            
            _tempBox.transform.SetParent(_tempCourtRecord.transform, true);
            yield return null;
            _tempBox.transform.SetParent(GameObject.FindWithTag("UI").transform, true); 
        }
        
        if (name != null)
        {
            if (name == "")
            {
                _nameBox.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _nameBox.transform.parent.gameObject.SetActive(true);
            }
        }
        
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
    

    IEnumerator EndDialogue()
    {
        if (_tempCourtRecord != null) _tempCourtRecord.GetComponent<Animator>().Play("Fade Out");
        while (_tempCourtRecord != null)
        {
            yield return null;
        }
        StartCoroutine(dialogueVertexAnimator.AnimateTextIn(new List<DialogueCommand>(), "", _typingClip, null));
        Destroy(_tempBox);
        _playerInput.SwitchCurrentActionMap(_prevActionMap);
    }
}
