using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CursorMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] _buttons;
    [SerializeField] private GameObject[] _examiningColliders;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private GameObject _transparent;
    [SerializeField] private GameObject _corner;

    [SerializeField] private Examine _examine;
    
    [SerializeField] private FadingIn _fadeIn;
    [SerializeField] private FadingOut _fadeOut;

    int _selection = 0;

    bool _turnedOff;
    bool _firstTime = true;

    void Start()
    {
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
            if(Input.GetKeyDown(KeyCode.A))
            {
                if(_selection == 0)
                {
                
                }
                else
                {
                    SoundEffectManager.PlaySound("move");
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
                    SoundEffectManager.PlaySound("move");
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
        SoundEffectManager.PlaySound("select");
        StartCoroutine(TurnOff(_transparent));
        StartCoroutine(TurnOff(_corner));
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
        _examine.Examining();
    }

    public void MovBtn()
    {
        _selection = 1;
        _cursor.transform.position = _buttons[1].transform.position;
        SoundEffectManager.PlaySound("select");
        StartCoroutine(TurnOn(_transparent));
        StartCoroutine(TurnOn(_corner));
        _fadeIn.startFading();
    }

    public void TalkBtn()
    {
        _selection = 2;
        _cursor.transform.position = _buttons[2].transform.position;
        SoundEffectManager.PlaySound("select");
    }

    public void PresBtn()
    {
        _selection = 3;
        _cursor.transform.position = _buttons[3].transform.position;
        SoundEffectManager.PlaySound("select");
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
