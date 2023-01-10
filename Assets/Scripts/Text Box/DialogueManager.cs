using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject _textBoxPrefab;
    [SerializeField] private GameObject _courtRecordPrefab;
    [SerializeField] private GameObject _courtRecord;
    [SerializeField] private SwapCharacters _swap;
    [SerializeField] private GameObject _interjectionObj;
    [SerializeField] private GameObject _controlFlag;
    [SerializeField] private GameObject _fade;
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private SpriteRenderer _foreground;
    
    private GameObject _tempCourtRecord;
    [HideInInspector] public TextBoxController _tempBox;
    private Animator _advanceButton;
    private TMP_Text textBox;
    private TMP_Text _nameBox;
    private List<TBLine> lines;
    [HideInInspector] public int _currentLine;
    private PlayerInput _playerInput;
    [HideInInspector] public string _prevActionMap;
    private string _typingClip = "blipmale";
    private bool _presenting;

    [HideInInspector] public DialogueVertexAnimator dialogueVertexAnimator;
    private bool _hideOptions;
    private InputAction _advanceText;
    private InputAction _cr;
    private MusicManager _musicManager;
    private SoundManager _soundManager;
    private List<GameObject> _galleryObjects = new List<GameObject>();

    [HideInInspector] public ResponseHandler _responseHandler;
    private DialogueSO _dialogue;
    private bool _shownResponses;

    [SerializeField] private Animator _char;
    private Animator _prevChar;
    private AnimationClip _currentAnim;
    private string _currentAnimName;

    private bool _startedText = true;
    private bool _skipFade;

    private bool _crossEx;
    private bool _autoEnd;
    private bool _mute;
    

    public bool _doneTalking;

    void Awake() {
        lines = new List<TBLine>();
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _advanceText = _playerInput.actions["Advance"];
        _cr = _playerInput.actions["Textbox/Court Record"];
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
    }

    public void StartText(DialogueSO linesIn, bool quickEnd = false, int startingLine = 0, string prevActionMap = null)
    {
        _controlFlag.SetActive(true);
        _doneTalking = false;
        _crossEx = linesIn.isCrossExamination;
        _shownResponses = false;
        _currentLine = startingLine;

        if (_tempBox != null)
        {
            Destroy(_tempBox.gameObject);
        }

        _tempBox = Instantiate(_textBoxPrefab).GetComponent<TextBoxController>();
        _tempBox.transform.SetParent(GameObject.FindWithTag("UI").transform, false);
        _tempBox.transform.SetSiblingIndex(_tempBox.transform.parent.childCount - 2);

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
        
        StartCoroutine(NextLine(true, quickEnd));
    }

    private void Update()
    {
        if (!(dialogueVertexAnimator == null) && !(_dialogue == null) && _startedText)
        {
            if (_advanceText.triggered)
            {
                StartCoroutine(NextLine());
            }
            else if (!dialogueVertexAnimator.textAnimating &&
                     (_currentLine == lines.Count && _dialogue.HasResponses && !_shownResponses) || _autoEnd)
            {
                if (!dialogueVertexAnimator.textAnimating)
                {
                    StartCoroutine(NextLine());   
                }
            }

            if (!dialogueVertexAnimator.textAnimating && _cr.triggered && !_hideOptions)
            {
                StartCoroutine(CourtRecord());
            }
            
            if (!dialogueVertexAnimator.textAnimating && !_advanceButton.gameObject.activeSelf)
            {
                if (_char != null) _char.Play($"{_currentAnimName}_idle");
                if ((_currentLine == lines.Count && _dialogue.HasResponses) || _tempBox.IsHidden) return;
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

    private IEnumerator CourtRecord()
    {
        if (_presenting || _crossEx) yield break;

        _presenting = true;
        
        GameObject obj = Instantiate(_courtRecord, GameObject.FindGameObjectWithTag("UI").transform, false);
        obj.GetComponent<CRNormal>().enabled = true;
        
        _playerInput.SwitchCurrentActionMap("Menu");

        while (obj != null)
        {
            yield return null;
        }

        _playerInput.SwitchCurrentActionMap("TextBox");
        _presenting = false;
    }
    
    private Coroutine typeRoutine = null;
    public IEnumerator NextLine(bool firstTime = false, bool quickEnd = false)
    {
        #region Variable Setup

        if (_autoEnd && dialogueVertexAnimator.textAnimating) yield break;
        if (_shownResponses) yield break;

        if (dialogueVertexAnimator.textAnimating)
        {
            dialogueVertexAnimator.QuickEnd();
            yield break;
        }

        if (!firstTime && !_mute)
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
        
        _mute = false;
        
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
            yield break;
        }

        _skipFade = false;
        TBLine line = lines[_currentLine];
        _currentLine += 1;
        this.EnsureCoroutineStopped(ref typeRoutine);
        dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands =
            DialogueUtility.ProcessInputString(line.Dialogue, out string totalTextMessage);
        String nameInfo = line.Name;
        String soundInfo = line.BlipSound;
        String faceInfo = line.Char;
        String emotionInfo = line.Anim;
        Interjection interjection = line.Interjection;
        BackgroundFade bg = line.FadeDetails;
        _hideOptions = line.HideOptions;
        _skipFade = line.FadeType == FadeTypes.SkipFade;

        _autoEnd = line.AutoEnd;
        if (line.AutoEnd)
        {
            _mute = true;
        }

        foreach (GameObject person in _galleryObjects)
        {
            Destroy(person);
        }

        _galleryObjects = new List<GameObject>();
        foreach (GameObject person in line.ObjectsToSpawn)
        {
            _galleryObjects.Add(Instantiate(person));
        }

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
        _fade.transform.position = new Vector3(_fade.transform.position.x, _fade.transform.position.y,
            (int) bg.BackgroundFadePos);

        if (line.StopMusic) _musicManager.fadeOut();
        
        if (soundInfo != null) _typingClip = soundInfo;
        
        try
        {
            _char = GameObject.Find($"{faceInfo}(Clone)").GetComponent<Animator>();
        }
        catch
        {
            _char = null;
        }
        
        _controlFlag.SetActive(false);
        _tempBox.HideAll();
        #endregion

        #region Reset Scene

        if (emotionInfo != null) _currentAnimName = emotionInfo;
        if (_char != null) _char.Play($"{_currentAnimName}_idle");

        _tempBox.transform.SetSiblingIndex(_tempBox.transform.parent.transform.Find("Controls").GetSiblingIndex() - 1);

        if (quickEnd)
        {
            typeRoutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, null, null));
            _swap.StartSwap(faceInfo, skipFade:true);
            dialogueVertexAnimator.QuickEnd();
            yield break;
        }
        
        if (_tempCourtRecord != null) _tempCourtRecord.GetComponent<Animator>().Play("Fade Out");
        while (_tempCourtRecord != null)
        {
            yield return null;
        }
        
        _startedText = false;
        GameObject obj = null;
        RawImage img;

        #endregion
        
        #region Interjection
        
        bool skip = false;
        switch (interjection)
        {
            case Interjection.Objection:
                obj = Instantiate(_interjectionObj);
                obj.transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform, false);
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
                obj.transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform, false);
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
                obj.transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform, false);
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
            _controlFlag.SetActive(false);
            _tempBox.HideAll();
            yield return new WaitForSeconds(1);
            Destroy(obj);
        }
        #endregion
        
        #region Screen Fade Out
        
        if (bg.BackgroundFadeType == BGFadeTypes.FadeOut || bg.BackgroundFadeType == BGFadeTypes.FadeOutThenIn)
        {
            SpriteRenderer spr = _fade.GetComponent<SpriteRenderer>();
            Color startColor = spr.color;
            Color endColor = new Color(bg.Color.r, bg.Color.g, bg.Color.b, 1);
            float time = 0;

            while (time < bg.LengthInSeconds)
            {
                time += Time.deltaTime;
                Debug.Log(spr.color.a);
                spr.color = Color.Lerp(startColor, endColor, time / bg.LengthInSeconds);
                if (Math.Abs(spr.color.a - endColor.a) < 0.0001) break;
                yield return null;
            }

            spr.color = endColor;
        }

        #endregion
        
        #region Swap Characters
        if (_char == null || line.FadeType == FadeTypes.ForceFade)
        {
            _prevChar = _char;
            _swap.StartSwap(faceInfo, fadeIn:faceInfo != "NaN", skipFade:_skipFade);
            if (!_skipFade)
            {
                _controlFlag.SetActive(false);
                _tempBox.HideAll();
            }
            
            while (!_swap._done)
            {
                yield return null;
            }
            
            try
            {
                _char = GameObject.Find($"{faceInfo}(Clone)").GetComponent<Animator>();
            }
            catch
            {
                _char = null;
            }

            if (!_skipFade)
            {
                yield return new WaitForSeconds(0.25f);
            }
        }
        
        #endregion
        
        #region Add To Court Record
        if (addToCourtRecord)
        {
            _soundManager.Play("court record");
            _tempCourtRecord = Instantiate(_courtRecordPrefab, GameObject.FindWithTag("UI").transform, false);

            _tempCourtRecord.GetComponentsInChildren<Image>()[3].sprite = Globals.Evidence[Globals.Evidence.Count - 1].Icon;
            _tempCourtRecord.GetComponentsInChildren<Image>()[3].SetNativeSize();
            
            _tempCourtRecord.gameObject.GetComponentsInChildren<TMP_Text>()[0].SetText(Globals.Evidence[Globals.Evidence.Count - 1].Name);
            _tempCourtRecord.gameObject.GetComponentsInChildren<TMP_Text>()[1].SetText(Globals.Evidence[Globals.Evidence.Count - 1].Description);
            
            _tempCourtRecord.transform.SetSiblingIndex(_tempBox.transform.GetSiblingIndex() - 1);
        }
        #endregion

        #region Set Background
        if (line.Background == "NaN")
        {
            _background.sprite = null;
            _foreground.sprite = null;
        }
        else if (line.Background != "")
        {
            _background.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/" + line.Background);
            try
            {
                _foreground.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/" + line.Background + "_fg");
            }
            catch
            {
            }
        }
        
        if (line.CustomForeground != "")
            _foreground.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/" + line.CustomForeground + "_fg");
        #endregion
        
        #region Screen Fade In
        
        if (bg.BackgroundFadeType == BGFadeTypes.FadeIn || bg.BackgroundFadeType == BGFadeTypes.FadeOutThenIn)
        {
            SpriteRenderer spr = _fade.GetComponent<SpriteRenderer>();
            Color startColor = spr.color;
            float time = 0;
            Color endColor = new Color(bg.Color.r, bg.Color.g, bg.Color.b, 0);
                
            while (time < bg.LengthInSeconds)
            {
                time += Time.deltaTime;
                spr.color = Color.Lerp(startColor, endColor, time / bg.LengthInSeconds);
                if (Math.Abs(spr.color.a - endColor.a) < 0.0001) break;
                yield return null;
            }

            spr.color = endColor;
        }

        #endregion

        #region Play Opening Animation
        if (_char != null)
        {
            if (!_char.HasState(0, Animator.StringToHash($"{_currentAnimName}_opening"))) goto SkipIf;
            
            _char.Play($"{_currentAnimName}_opening");
            yield return null;

            while (Globals.IsAnimationPlaying(_char, $"{_currentAnimName}_opening"))
            {
                yield return null;
            }
        }
        SkipIf:
        #endregion

        #region Show Textbox
        if (_hideOptions) _controlFlag.SetActive(false);
        else _controlFlag.SetActive(true);
        _tempBox.ShowAll();
        
        _advanceButton.gameObject.SetActive(false);
        #endregion
        
        #region Start Text
        if (String.Concat(totalTextMessage.Where(c => !Char.IsWhiteSpace(c))) == "" || totalTextMessage == null)
        {
            _controlFlag.SetActive(false);
            _tempBox.HideAll();
            _mute = true;
        }
        else
        {
            _nameBox.text = nameInfo;
            _nameBox.transform.parent.gameObject.SetActive(!line.HideNameTag);
        }
        
        if (_char != null && !line.Thinking) _char.Play($"{_currentAnimName}_talk");
        
        typeRoutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, _typingClip, null));
        _startedText = true;
        #endregion
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
        _controlFlag.SetActive(false);
        Destroy(_tempBox.gameObject);
        _playerInput.SwitchCurrentActionMap(_prevActionMap);
    }
}
