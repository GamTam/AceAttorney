using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadingIn : MonoBehaviour
{
    Image _rend;
    SpriteRenderer _spriteRenderer;

    [SerializeField] private TextMeshProUGUI[] _textBoxes;
    [SerializeField] private GameObject[] _gameObjects;
    [SerializeField] private string _answer;

    void Start()
    {
        if(_answer == "GameObject")
        {
            //Image
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Color c = _spriteRenderer.material.color;
            c.a = 0f;
        }
        else if(_answer == "Image")
        {
            //Image
            _rend = GetComponent<Image>();
            Color c = _rend.material.color;
            c.a = 0f;
        }
    }

    IEnumerator FadeIn()
    {
        if(_answer == "GameObject")
        {
            
        }
        else if(_answer == "Image")
        {
            for(float f = 0f; f <= 1; f += 0.50f)
            {
                for(int i = 0; i < _textBoxes.Length; i++)
                {
                    Color Ex = _textBoxes[i].color;
                    Ex.a = f;
                    _textBoxes[i].color = Ex;
                }
                Color c = _rend.material.color;
                c.a = f;
                _rend.material.color = c;
                yield return new WaitForSeconds(0.05f);
            }    
        }
    }

    public void startFading()
    {
        StartCoroutine(FadeIn());
    }
}
