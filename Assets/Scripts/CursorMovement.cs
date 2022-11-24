using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
    
    [Header("Transition")]
    [SerializeField] private FadingIn _fadeIn;
    [SerializeField] private FadingOut _fadeOut;
    
    [Header("Misc.")]
    [SerializeField] string _song = "SteelSamurai";
    [SerializeField] SwapCharacters _swap;

    int _selection = 0;

    bool _turnedOff;
    bool _firstTime = true;
    
    private PlayerInput _playerInput;
    private InputAction _left;
    private InputAction _right;
    private InputAction _select;
    
    private MusicManager _musicManager;
    private SoundManager _soundManager;
    private bool _continueSong = false;

    void Start()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Menu");
        
        _left = _playerInput.actions["Left"];
        _right = _playerInput.actions["Right"];
        _select = _playerInput.actions["Select"];
        
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        _musicManager.Play(_song);
        
        for(int i = 0; i < _examiningColliders.Length; i++)
        {
            _examiningColliders[i].SetActive(false);
        }
    }
    
    void Update()
    {
        if(_firstTime == true)
        {
            _fadeIn.startFading();
            _firstTime = false;
        }
        if(_turnedOff == false)
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
                    default:
                        break;
                }
            }
        }
    }

    public void ExBtn()
    {
        _selection = 0;
        _cursor.transform.position = _buttons[0].transform.position;
        _soundManager.Play("confirm");
        StartCoroutine(TurnOff(_transparent));
        // StartCoroutine(TurnOff(_corner));
        _swap.StartSwap(fadeIn:false);
        _fadeOut.startFading();
        _turnedOff = true;
        for(int i = 0; i < _examiningColliders.Length; i++)
        {
            _examiningColliders[i].SetActive(true);
        }
        for(int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].SetActive(false);
        }
        
        Instantiate(_investigationCursor);
    }

    public void MovBtn()
    {
        _selection = 1;
        _cursor.transform.position = _buttons[1].transform.position;
        _soundManager.Play("confirm");
        StartCoroutine(TurnOn(_transparent));
        StartCoroutine(TurnOn(_corner));
        _fadeIn.startFading();
    }

    public void TalkBtn()
    {
        _selection = 2;
        _cursor.transform.position = _buttons[2].transform.position;
        _soundManager.Play("confirm");
    }

    public void PresBtn()
    {
        _selection = 3;
        _cursor.transform.position = _buttons[3].transform.position;
        _soundManager.Play("confirm");
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
}
