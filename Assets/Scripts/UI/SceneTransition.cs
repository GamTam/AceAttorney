using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private SpriteRenderer _spr;
    public float _speed = 2;
    public string _destination;
    private PlayerInput _playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.transform.position = Camera.main.transform.position;
        _spr = GetComponent<SpriteRenderer>();
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _spr.color = new Color(0, 0, 0, 0);
        
        _playerInput.SwitchCurrentActionMap("Null");
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        while (_spr.color.a < 1)
        {
            _spr.color = new Color(0, 0, 0, _spr.color.a + _speed * Time.deltaTime);
            yield return null;
        }
        _spr.color = Color.black;

        SceneManager.LoadScene(_destination);
        yield return null;
        gameObject.transform.position = Camera.main.transform.position;
        StartCoroutine(FadeOut());
    }
    
    private IEnumerator FadeOut()
    {
        while (_spr.color.a > 0)
        {
            _spr.color = new Color(0, 0, 0, _spr.color.a - _speed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
