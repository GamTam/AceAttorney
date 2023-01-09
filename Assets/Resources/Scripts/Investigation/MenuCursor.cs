using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{
    protected GameObject _selectedButton;
    protected SoundManager _soundManager;
    protected Animator _anim;
    
    protected void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(_selectedButton);
        transform.position = EventSystem.current.currentSelectedGameObject.transform.position;

        if (_selectedButton != EventSystem.current.currentSelectedGameObject)
        {
            if (_selectedButton != null) _soundManager.Play("select");
            _selectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

    public void Click()
    {
        _soundManager.Play("confirm");
        _anim.Play("Select Fade Out");
    }
}
