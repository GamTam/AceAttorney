using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private SpriteRenderer _spr;
    public float _speed = 2;
    public string _destination;
    private PlayerInput _playerInput;
    private MusicManager _musicManager;
    public SpriteRenderer _toBeContinued;
    public SpriteRenderer _theEnd;

    public bool TBC;
    public bool Ending;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Vector3 pos = Camera.main.transform.position;
        gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + 1);
        _spr = GetComponent<SpriteRenderer>();
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _musicManager = GameObject.FindWithTag("Audio").GetComponent<MusicManager>();
        _spr.color = new Color(_spr.color.r, _spr.color.g, _spr.color.b, 0);
        
        _playerInput.SwitchCurrentActionMap("Null");
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        while (_spr.color.a < 1)
        {
            _spr.color = new Color(_spr.color.r, _spr.color.g, _spr.color.b, _spr.color.a + _speed * Time.deltaTime);
            yield return null;
        }
        _spr.color = Color.black;

        if (TBC || Ending)
        {
            yield return new WaitForSeconds(1);
            
            SpriteRenderer spr;
            if (TBC)
            {
                spr = _toBeContinued;
            }
            else
            {
                spr = _theEnd;
            }
            
            while (spr.color.a < 1)
            {
                spr.color = new Color(1, 1, 1, spr.color.a + _speed * Time.deltaTime);
                yield return null;
            }
            
            spr.color = Color.white;
            yield return new WaitForSeconds(2);
            
            while (spr.color.a > 0)
            {
                spr.color = new Color(1, 1, 1, spr.color.a - _speed * Time.deltaTime);
                yield return null;
            }
            
            yield return new WaitForSeconds(2);
            _musicManager.fadeOut(2f);
            yield return new WaitForSeconds(5);
        }

        SceneManager.LoadScene(_destination);
        yield return null;
        gameObject.transform.position = Camera.main.transform.position;
        StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeOut()
    {
        while (_spr.color.a > 0)
        {
            _spr.color = new Color(_spr.color.r, _spr.color.g, _spr.color.b, _spr.color.a - _speed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
