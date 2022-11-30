using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CursorMovement : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] _buttons;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private GameObject _transparent;
    [SerializeField] private GameObject _corner;

    [Header("Explore Elements")] 
    [SerializeField] private GameObject _investigationCursor;
    [SerializeField] private GameObject[] _examiningColliders;

    [Header("Move Elements")]
    [SerializeField] private GameObject[] _moveObjects;
    [SerializeField] private GameObject _cursorMove;
    [SerializeField] private GameObject[] _moveButtons;
    [SerializeField] private TextMeshProUGUI[] _AreaText;
    [SerializeField] private string[] _AreaNames;
    [SerializeField] private Texture[] _AreaTextures;

    [Header("Talking Elements")]
    [SerializeField] private GameObject _cursorTalking;
    [SerializeField] private GameObject[] _talkingButtons;
    [SerializeField] private TextMeshProUGUI[] _talkText;
    [SerializeField] private string[] _talkNames;

    [Header("Present Elements")]
    [SerializeField] private GameObject[] _presentObjects;
    [SerializeField] private GameObject _evidenceIcon;
    [SerializeField] private GameObject _evidenceProfile;
    [SerializeField] private GameObject _cursorPresent;
    [SerializeField] private GameObject[] _presentButtons;
    [SerializeField] private TextMeshProUGUI _titleEvidence;
    [SerializeField] private TextMeshProUGUI _descriptionEvidence;
    [SerializeField] private DialogueTrigger[] _presentEvidence;
    [SerializeField] private Texture[] _evidenceTextures;
    
    [Header("Transition")]
    [SerializeField] private FadingIn _fadeIn;
    [SerializeField] private FadingOut _fadeOut;
    [SerializeField] private GameObject _offScreen;
    [SerializeField] private GameObject[] _darkenBackground;
    
    [Header("Misc.")]
    [SerializeField] string _song;
    [SerializeField] SwapCharacters _swap;
    [SerializeField] public Sprite _completedTalkingImage;

    int _selection = 0;
    int _maxEvidence = 0;

    bool _turnedOff;
    bool _examine;
    bool _move;
    bool _talk;
    bool _present;
    bool _pageSwap;
    
    private DialogueManager _doneTalking;
    
    private PlayerInput _playerInput;
    private InputAction _left;
    private InputAction _right;
    private InputAction _select;
    private InputAction _back;

    //Move
    private RawImage[] _rawImagesMove;

    //Present
    private RawImage[] _rawImagesIconPresent;
    private RawImage[] _rawImagesProfilePresent;
    
    private MusicManager _musicManager;
    private SoundManager _soundManager;

    void Start()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Menu");
        
        _left = _playerInput.actions["Left"];
        _right = _playerInput.actions["Right"];
        _select = _playerInput.actions["Select"];
        _back = _playerInput.actions["Back"];
        
        GameObject obj = GameObject.FindGameObjectWithTag("Dialogue Manager");
        _doneTalking = obj.GetComponent<DialogueManager>();

        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        _musicManager.Play(_song);

        if(_maxEvidence > 9)
        {
            _pageSwap = true;
        }

        _rawImagesMove = new RawImage[_AreaTextures.Length];
        _rawImagesIconPresent = new RawImage[_evidenceTextures.Length];
        _rawImagesProfilePresent = new RawImage[_evidenceTextures.Length];
        Setup();

        StartCoroutine(WaitThenFade());
    }
    
    void Update()
    {
        if(!_turnedOff)
        {
            //Left or A
            if(_left.triggered)
            {
                if(_selection == 0)
                {
                
                }
                else
                {
                    _soundManager.Play("select");
                    _selection--;
                    _cursor.transform.position = _buttons[_selection].transform.position;
                }
            }
            //Right or D
            else if(_right.triggered)
            {
                if(_selection == 3)
                {
                
                }
                else
                {
                    _soundManager.Play("select");
                    _selection++;
                    _cursor.transform.position = _buttons[_selection].transform.position;
                }
            }

            if(_select.triggered)
            {
                //_buttons[_selection].onClick.Invoke();
                switch(_selection)
                {
                    case 0:
                        ExBtn();
                        break;
                    case 1:
                        MovBtn();
                        break;
                    case 2:
                        TalkBtn();
                        break;
                    case 3:
                        PresBtn();
                        break;
                }
            }
        }
        else
        {
            if(_back.triggered)
            {
                Back();
            }

            if(_doneTalking._doneTalking && !_examine && !_present && !_move && _talk)
            {
                TalkReturn();
                _doneTalking._doneTalking = false;
            }

            if(_doneTalking._doneTalking && !_examine && !_talk && !_move && _present)
            {
                PresentReturn();
                _doneTalking._doneTalking = false;
            }
        }

        if(_talk)
        {
            Selection(_cursorTalking, _talkingButtons, 2);
        }
        else if(_present)
        {
            Selection(_cursorPresent, _presentButtons, 9);
        }
        else if(_move)
        {
            Selection(_cursorMove, _moveButtons, 1);
        }
    }

    public void Back()
    {
        _doneTalking._doneTalking = false;
        _turnedOff = false;
        _soundManager.Play("back");
        StartCoroutine(TurnOn(_transparent));
        AppearArrays(_buttons);
        _selection = 0;
        _cursor.transform.position = _buttons[_selection].transform.position;
        _fadeIn.startFading();
        _cursorTalking.SetActive(false);
        _cursorPresent.SetActive(false);
        _cursorMove.SetActive(false);
        if(_examine)
        {
            _examine = false;
        }
        else if(_move)
        {
            _selection = 1;
            _cursor.transform.position = _buttons[_selection].transform.position;
            DissappearArrays(_moveButtons);
            DissappearArrays(_moveObjects);
            DissappearArrays(_darkenBackground);
            _move = false;
        }
        else if(_talk)
        {
            _selection = 2;
            _cursor.transform.position = _buttons[_selection].transform.position;
            DissappearArrays(_talkingButtons);
            DissappearArrays(_darkenBackground);
            _talk = false;
        }
        else if(_present)
        {
            _selection = 3;
            _cursor.transform.position = _buttons[_selection].transform.position;
            DissappearArrays(_presentObjects);
            DissappearArrays(_presentButtons);
            DissappearArrays(_darkenBackground);
            _present = false;
        }
    }

    public void ExBtn()
    {
        _move = false;
        _talk = false;
        _examine = true;
        _present = false;
        _selection = 0;
        _cursor.transform.position = _buttons[0].transform.position;
        _soundManager.Play("confirm");
        StartCoroutine(TurnOff(_transparent));
        // StartCoroutine(TurnOff(_corner));
        _swap.StartSwap(fadeIn:false);
        _fadeOut.startFading();
        _turnedOff = true;
        AppearArrays(_examiningColliders);
        DissappearArrays(_buttons);
        
        Instantiate(_investigationCursor);
    }

    public void MovBtn()
    {
        _move = true;
        _talk = false;
        _examine = false;
        _present = false;
        _selection = 0;
        _cursor.transform.position = _offScreen.transform.position;
        _soundManager.Play("confirm");
        StartCoroutine(TurnOff(_transparent));
        //_fadeOut.startFading();
        _turnedOff = true;
        DissappearArrays(_buttons);
        AppearArrays(_darkenBackground);
        AppearArrays(_moveButtons);
        AppearArrays(_moveObjects);
        _moveObjects[1].SetActive(false);
        _cursorMove.SetActive(true);
        _cursorMove.transform.position = _moveButtons[0].transform.position;
    }

    public void TalkBtn()
    {
        _talk = true;
        _examine = false;
        _present = false;
        _move = false;
        _selection = 0;
        _cursor.transform.position = _buttons[2].transform.position;
        _soundManager.Play("confirm");
        StartCoroutine(TurnOff(_transparent));
        _fadeOut.startFading();
        _turnedOff = true;
        DissappearArrays(_buttons);
        AppearArrays(_darkenBackground);
        AppearArrays(_talkingButtons);
        _cursorTalking.SetActive(true);
        _cursorTalking.transform.position = _talkingButtons[0].transform.position;
    }

    public void PresBtn()
    {
        _present = true;
        _examine = false;
        _talk = false;
        _move = false;
        _selection = 0;
        _cursor.transform.position = _buttons[3].transform.position;
        _soundManager.Play("confirm");
        StartCoroutine(TurnOff(_transparent));
        _fadeOut.startFading();
        _turnedOff = true;
        DissappearArrays(_buttons);
        AppearArrays(_darkenBackground);
        AppearArrays(_presentObjects);
        AppearArrays(_presentButtons);
        _cursorPresent.SetActive(true);
        _cursorPresent.transform.position = _presentButtons[0].transform.position;
        _presentButtons[0].SetActive(false);
        PresentingSelection();
    }

    public void Talk(int button)
    {
        _selection = button;
        _cursorTalking.transform.position = _talkingButtons[button].transform.position;
        _soundManager.Play("confirm");
        _cursorTalking.SetActive(false);
        DissappearArrays(_talkingButtons);
        DissappearArrays(_darkenBackground);
    }

    public void TalkReturn()
    {
        _cursorTalking.SetActive(true);
        _cursorTalking.transform.position = _talkingButtons[_selection].transform.position;
        _talkingButtons[_selection].GetComponent<Image>().sprite = _completedTalkingImage;
        AppearArrays(_talkingButtons);
        AppearArrays(_darkenBackground);
    }

    public void Present(int button)
    {
        _selection = button;
        _cursorPresent.transform.position = _presentButtons[button].transform.position;
        _soundManager.Play("confirm");
        DissappearArrays(_presentObjects);
        DissappearArrays(_presentButtons);
        DissappearArrays(_darkenBackground);
        _cursorPresent.SetActive(false);
    }

    public void PresentReturn()
    {
        _cursorPresent.SetActive(true);
        _cursorPresent.transform.position = _presentButtons[_selection].transform.position;
        AppearArrays(_presentButtons);
        AppearArrays(_presentObjects);
        AppearArrays(_darkenBackground);
        _presentButtons[_selection].SetActive(false);
    }

    public void PresentingSelection()
    {
        switch(_selection)
        {
            case 0:
                _titleEvidence.text = "Attorney's Badge";
                _descriptionEvidence.text = "No one would believe I was a defence attorney if I didn't carry this.";
                break;
            case 1:
                _titleEvidence.text = "Autopsy Reports";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 2:
                _titleEvidence.text = "Barbershop Financial Records";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 3:
                _titleEvidence.text = "Bloody Gloves";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 4:
                _titleEvidence.text = "Bottle of Pills";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 5:
                _titleEvidence.text = "Cafe Front Sign";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 6:
                _titleEvidence.text = "Cafe Receipt";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 7:
                _titleEvidence.text = "Decisive Evidence";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 8:
                _titleEvidence.text = "Doctor's Report";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 9:
                _titleEvidence.text = "Empty Kitchen";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 10:
                _titleEvidence.text = "Gun";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 11:
                _titleEvidence.text = "Phone Line Outage Report";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 12:
                _titleEvidence.text = "Security Camera Photo";
                _descriptionEvidence.text = "No description at the moment.";
                break;
            case 13:
                _titleEvidence.text = "Singing Barbershop Floor Plan";
                _descriptionEvidence.text = "No description at the moment.";
                break;
        }
    }

    public void Move(int button)
    {
        _selection = button;
        _cursorMove.transform.position = _moveButtons[button].transform.position;
        _soundManager.Play("confirm");
        DissappearArrays(_moveObjects);
        DissappearArrays(_moveButtons);
        DissappearArrays(_darkenBackground);
        _cursorMove.SetActive(false);
        _musicManager.Stop();
        SceneManager.LoadScene(sceneName:_AreaNames[_selection]);
    }

    IEnumerator TurnOff(GameObject _item)
    {
        for(float i = 1f; i >= -0.05f; i -= 0.05f)
        {
            _item.transform.localScale = new Vector2(1, i);
            yield return null;
        }
        yield return new WaitForSeconds(0.15f);
    }

    IEnumerator TurnOn(GameObject _item)
    {
        for(float i = 0f; i < 1.01f; i += 0.05f)
        {
            _item.transform.localScale = new Vector2(1, i);
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.15f);
    }

    private IEnumerator WaitThenFade()
    {
        yield return new WaitForSeconds(0.1f);
        _fadeIn.startFading();
    }

    public void DissappearArrays(GameObject[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(false);
        }
    }

    public void AppearArrays(GameObject[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(true);
        }
    }

    public void Selection(GameObject cursor, GameObject[] Buttons, int counter)
    {
        if(_left.triggered)
        {
            if(_selection == 0 || _selection > 9 && _present)
            {
                if(_selection > 9 && _present)
                {
                    _soundManager.Play("select");
                    if(_selection == 10)
                    {
                        cursor.transform.position = Buttons[_selection - 1].transform.position;
                        _presentButtons[0].SetActive(true);
                        _presentButtons[9].SetActive(false);
                        _pageSwap = true;
                        _selection--;
                    }
                    else
                    {
                        _selection--;
                        cursor.transform.position = Buttons[_selection - 10].transform.position;
                        _presentButtons[_selection - 10].SetActive(false);
                        _presentButtons[_selection - 9].SetActive(true);
                    }
                    _rawImagesIconPresent[_selection] = (RawImage)_evidenceIcon.GetComponent<RawImage>(); 
                    _rawImagesIconPresent[_selection].texture = (Texture)_evidenceTextures[_selection];
                    _rawImagesProfilePresent[_selection] = (RawImage)_evidenceProfile.GetComponent<RawImage>(); 
                    _rawImagesProfilePresent[_selection].texture = (Texture)_evidenceTextures[_selection];
                    PresentingSelection();
                }
                else
                {

                }
            }
            else
            {
                _soundManager.Play("select");
                _selection--;
                cursor.transform.position = Buttons[_selection].transform.position;
                if(_present)
                {
                    _presentButtons[_selection].SetActive(false);
                    _presentButtons[_selection + 1].SetActive(true);

                    _rawImagesIconPresent[_selection] = (RawImage)_evidenceIcon.GetComponent<RawImage>(); 
                    _rawImagesIconPresent[_selection].texture = (Texture)_evidenceTextures[_selection];

                    _rawImagesProfilePresent[_selection] = (RawImage)_evidenceProfile.GetComponent<RawImage>(); 
                    _rawImagesProfilePresent[_selection].texture = (Texture)_evidenceTextures[_selection];

                    PresentingSelection();
                }
                else if(_move)
                {
                    _moveObjects[1].SetActive(false);
                    _moveObjects[0].SetActive(true);
                }
            }
        }
        //Right or D
        else if(_right.triggered)
        {
            if(_selection >= counter)
            {
                if(_selection == _maxEvidence && _present)
                {
                    
                }
                else if(_maxEvidence > 9 && _present)
                {
                    _soundManager.Play("select");
                    _selection++;
                    cursor.transform.position = Buttons[_selection - 10].transform.position;
                    if(_pageSwap)
                    {
                        _presentButtons[0].SetActive(false);
                        _presentButtons[9].SetActive(true);
                        _pageSwap = false;
                    }
                    else
                    {
                        _presentButtons[_selection - 10].SetActive(false);
                        _presentButtons[_selection - 11].SetActive(true);
                    }
                    _rawImagesIconPresent[_selection] = (RawImage)_evidenceIcon.GetComponent<RawImage>(); 
                    _rawImagesIconPresent[_selection].texture = (Texture)_evidenceTextures[_selection];
                    _rawImagesProfilePresent[_selection] = (RawImage)_evidenceProfile.GetComponent<RawImage>(); 
                    _rawImagesProfilePresent[_selection].texture = (Texture)_evidenceTextures[_selection];
                    PresentingSelection();
                }
            }
            else if(_selection == _maxEvidence && _present)
            {

            }
            else
            {
                _soundManager.Play("select");
                _selection++;
                cursor.transform.position = Buttons[_selection].transform.position;
                if(_present)
                {
                    _presentButtons[_selection].SetActive(false);
                    _presentButtons[_selection - 1].SetActive(true);

                    _rawImagesIconPresent[_selection] = (RawImage)_evidenceIcon.GetComponent<RawImage>(); 
                    _rawImagesIconPresent[_selection].texture = (Texture)_evidenceTextures[_selection];
                    
                    _rawImagesProfilePresent[_selection] = (RawImage)_evidenceProfile.GetComponent<RawImage>(); 
                    _rawImagesProfilePresent[_selection].texture = (Texture)_evidenceTextures[_selection];

                    PresentingSelection();
                }
                else if(_move)
                {
                    _moveObjects[1].SetActive(true);
                    _moveObjects[0].SetActive(false);
                }
            }
        }
        if(_select.triggered)
        {
            if(_move)
            {

            }
            if(_talk)
            {
                Talk(_selection);
            }
            if(_present)
            {
                Present(_selection);
                _presentEvidence[_selection].TriggerDialogue();
            }
        }
    }

    public void Setup()
    {
        //Move Setup
        for(int a = 0; a < _AreaTextures.Length; a++)
        {
            _rawImagesMove[a] = (RawImage)_moveObjects[a].GetComponent<RawImage>(); 
            _rawImagesMove[a].texture = (Texture)_AreaTextures[a];
        }
        for(int b = 0; b < _AreaText.Length; b++)
        {
            _AreaText[b].text = _AreaNames[b];
        }
        //Talking Setup
        for(int c = 0; c < _talkText.Length; c++)
        {
            _talkText[c].text = _talkNames[c];
        }
    }
}
