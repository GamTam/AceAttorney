using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CRCrossEx: MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CourtRecordController _cr;

    private InvestigationMenu _menu;
    
    private PlayerInput _playerInput;

    private InputAction _back;
    private InputAction _present;
    
    void Start()
    {
        _text.SetText("<sprite=\"Keyboard\" name=\"backspace\">Back         <sprite=\"Keyboard\" name=\"E\">Present");
        _playerInput = GameObject.FindWithTag("Controller Manager").GetComponent<PlayerInput>();
        
        _back = _playerInput.actions["Menu/Cancel"];
        _present = _playerInput.actions["Menu/Present"];
    }
    
    void Update()
    {
        if (_back.triggered)
        {
            _playerInput.SwitchCurrentActionMap("Textbox");
            _cr.Close();
        }

        if (_present.triggered)
        {
            _playerInput.SwitchCurrentActionMap("Textbox");
            _cr.Close(false);
            _cr.Present();
        }
    }
}