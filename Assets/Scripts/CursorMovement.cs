using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CursorMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] _buttons;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private GameObject _transparent;
    [SerializeField] private GameObject _corner;

    [SerializeField] private Examine _examine;
    
    [SerializeField] private FadingIn _fadeIn;
    [SerializeField] private FadingOut _fadeOut;

    int _selection = 0;

    bool _turnedOff;
    bool _firstTime = true;
    
    private MusicManager _musicManager;
    private SoundManager _soundManager;
    private bool _continueSong = false;
    [SerializeField] string _song = "SteelSamurai";

    void Start()
    {
        _musicManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<MusicManager>();
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        _musicManager.Play(_song);
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
            if(Input.GetKeyDown(KeyCode.A))
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
            else if(Input.GetKeyDown(KeyCode.D))
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

            if(Input.GetKeyDown(KeyCode.Return))
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
        StartCoroutine(TurnOff(_corner));
        _fadeOut.startFading();
        _turnedOff = true;
        _buttons[0].SetActive(false);
        _buttons[1].SetActive(false);
        _buttons[2].SetActive(false);
        _buttons[3].SetActive(false);
        _examine.Examining();
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
            yield return new WaitForSeconds(0.005f);
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
