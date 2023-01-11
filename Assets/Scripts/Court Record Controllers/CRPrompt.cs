using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CRPrompt : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CourtRecordController _cr;

    private InvestigationMenu _menu;
    
    private PlayerInput _playerInput;

    private InputAction _back;
    private InputAction _profiles;
    private InputAction _present;
    
    void Start()
    {
        _text.SetText("<sprite=\"Keyboard\" name=\"E\">Present         <sprite=\"Keyboard\" name=\"R\">Profiles");
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
        
        if (_profiles.triggered)
        {
            _cr.ProfileEvidenceSwap();
        }
    }
    
    public void SetControlLabel(bool evidence)
    {
        if (evidence)
        {
            _text.SetText("<sprite=\"Keyboard\" name=\"E\">Present         <sprite=\"Keyboard\" name=\"R\">Profiles");
            return;
        }
        
        _text.SetText("<sprite=\"Keyboard\" name=\"E\">Present         <sprite=\"Keyboard\" name=\"R\">Evidence");
    }
}
