using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] private Renderer _cursorRenderer;
    [SerializeField] private GameObject _cursoring;
    [SerializeField] private Color _newColor;
    [SerializeField] private float _moveSpeed = 3;

    private Camera _cam;

    bool _soTrue = false;

    private PlayerInput _playerInput;
    private InputAction _mousePos;
    private InputAction _vCursor;
    
    void Start()
    {
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        _cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        _mousePos = _playerInput.actions["MousePos"];
        _vCursor = _playerInput.actions["MoveVector"];
        
        _cursorRenderer = _cursoring.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_soTrue)
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
        }
    }

    public void TrueMachine()
    {
        transform.position = new Vector3(_cam.transform.position.x, _cam.transform.position.y, transform.position.z);
        _playerInput.SwitchCurrentActionMap("Investigation");
        _soTrue = true;
    }
}
