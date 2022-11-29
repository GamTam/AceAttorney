using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorMovement : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] _buttons;
    [SerializeField] private GameObject[] _talkingButtons;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private GameObject _cursorTalking;
    [SerializeField] private GameObject _transparent;
    [SerializeField] private GameObject _corner;

    [Header("Explore Elements")] 
    [SerializeField] private GameObject _investigationCursor;
    [SerializeField] private GameObject[] _examiningColliders;
    [SerializeField] private GameObject[] _darkenBackground;
    [SerializeField] private GameObject[] _presentObjects;
    
    [Header("Transition")]
    [SerializeField] private FadingIn _fadeIn;
    [SerializeField] private FadingOut _fadeOut;
    
    [Header("Misc.")]
    [SerializeField] string _song = "SteelSamurai";
    [SerializeField] SwapCharacters _swap;

    int _selection = 0;

    bool _turnedOff;
    bool _examine;
    bool _move;
    bool _talk;
    bool _present;
    bool _talking;
    
    private DialogueManager _doneTalking;
    
    private PlayerInput _playerInput;
    private InputAction _left;
    private InputAction _right;
    private InputAction _select;
    private InputAction _back;
    
    private MusicManager _musicManager;
    private SoundManager _soundManager;

    public Sprite _completedTalkingImage;

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
            //Attempted input code, but I dont think I can do it this way.

            if(_back.triggered)
            {
                _turnedOff = false;
                _soundManager.Play("back");
                StartCoroutine(TurnOn(_transparent));
                AppearArrays(_buttons);
                _selection = 0;
                _cursor.transform.position = _buttons[_selection].transform.position;
                _fadeIn.startFading();
                if(_examine)
                {
                    _examine = false;
                }
                else if(_talk)
                {
                    _selection = 2;
                    _cursor.transform.position = _buttons[_selection].transform.position;
                    _cursorTalking.SetActive(false);
                    DissappearArrays(_talkingButtons);
                    DissappearArrays(_darkenBackground);
                    _talk = false;
                }
                else if(_present)
                {
                    _selection = 3;
                    _cursor.transform.position = _buttons[_selection].transform.position;
                    DissappearArrays(_presentObjects);
                    DissappearArrays(_darkenBackground);
                    _present = false;
                }
            }

            if(_doneTalking._doneTalking && !_examine)
            {
                TalkReturn();
                _doneTalking._doneTalking = false;
            }
        }

        if(_talk)
        {
            if(_left.triggered)
            {
                if(_selection == 0)
                {
                
                }
                else
                {
                    _soundManager.Play("select");
                    _selection--;
                    _cursorTalking.transform.position = _talkingButtons[_selection].transform.position;
                }
            }
            //Right or D
            else if(_right.triggered)
            {
                if(_selection == 2)
                {
                
                }
                else
                {
                    _soundManager.Play("select");
                    _selection++;
                    _cursorTalking.transform.position = _talkingButtons[_selection].transform.position;
                }
            }

            if(_select.triggered)
            {
                Talk(_selection);
            }
        }
        else if(_present)
        {
            if(_left.triggered)
            {
                if(_selection == 0)
                {
                
                }
                else
                {
                    _soundManager.Play("select");
                    _selection--;
                    _cursorTalking.transform.position = _talkingButtons[_selection].transform.position;
                }
            }
            //Right or D
            else if(_right.triggered)
            {
                if(_selection == 2)
                {
                
                }
                else
                {
                    _soundManager.Play("select");
                    _selection++;
                    _cursorTalking.transform.position = _talkingButtons[_selection].transform.position;
                }
            }

            if(_select.triggered)
            {
                Talk(_selection);
            }
        }
    }

    public void ExBtn()
    {
        _examine = true;
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
        _selection = 1;
        _cursor.transform.position = _buttons[1].transform.position;
        _soundManager.Play("confirm");
        StartCoroutine(TurnOn(_transparent));
        //StartCoroutine(TurnOn(_corner));
        _fadeIn.startFading();
    }

    public void TalkBtn()
    {
        _talk = true;
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
        _selection = 3;
        _cursor.transform.position = _buttons[3].transform.position;
        _soundManager.Play("confirm");
        StartCoroutine(TurnOff(_transparent));
        _fadeOut.startFading();
        _turnedOff = true;
        DissappearArrays(_buttons);
        AppearArrays(_darkenBackground);
        AppearArrays(_presentObjects);
    }

    public void Talk(int button)
    {
        _selection = button;
        _cursorTalking.transform.position = _talkingButtons[button].transform.position;
        _soundManager.Play("confirm");
        _cursorTalking.SetActive(false);
        DissappearArrays(_talkingButtons);
        DissappearArrays(_darkenBackground);
        _talking = true;
    }

    public void TalkReturn()
    {
        _cursorTalking.SetActive(true);
        _cursorTalking.transform.position = _talkingButtons[_selection].transform.position;
        _talkingButtons[_selection].GetComponent<Image>().sprite = _completedTalkingImage;
        AppearArrays(_talkingButtons);
        AppearArrays(_darkenBackground);
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
}
