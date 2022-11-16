using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCursor : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private float _moveSpeed = 3;

    private Camera _cam;
    private DialogueTrigger _selectedObj;

    bool _soTrue = false;

    private PlayerInput _playerInput;
    private InputAction _mousePos;
    private InputAction _vCursor;
    private InputAction _select;
    
    void Start()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        
        transform.position = new Vector3(_cam.transform.position.x, _cam.transform.position.y, transform.position.z);
        _playerInput.SwitchCurrentActionMap("Investigation");

        _mousePos = _playerInput.actions["Investigation/MousePos"];
        _vCursor = _playerInput.actions["Investigation/MoveVector"];
        _select = _playerInput.actions["Investigation/Select"];
        
        Debug.Log(_select);
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        Vector2 cursorPos;

        if (_playerInput.currentControlScheme == "Mouse")
        {
            cursorPos = Camera.main.ScreenToWorldPoint(_mousePos.ReadValue<Vector2>());
            transform.position = cursorPos;
        }
        else
        {
            cursorPos = _vCursor.ReadValue<Vector2>();
            transform.position = transform.position + (Vector3) (cursorPos * Time.deltaTime * _moveSpeed);
        }

        if (_select.triggered)
        {
            Debug.Log("Eminem");
            _selectedObj.TriggerDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Examinable")
        {
            _spriteRenderer.color = _selectedColor;
            _selectedObj = other.gameObject.GetComponent<DialogueTrigger>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Examinable")
        {
            _spriteRenderer.color = _baseColor;
            _selectedObj = null;
        }
    }
}
