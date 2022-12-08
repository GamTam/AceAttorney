using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CRPresent : MonoBehaviour
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
        _menu = GameObject.FindWithTag("UI").transform.Find("Investigation/Select").GetComponent<InvestigationMenu>();
        Debug.Log(_menu);
        
        _back = _playerInput.actions["Menu/Cancel"];
        _present = _playerInput.actions["Menu/Present"];
    }
    
    void Update()
    {
        if (_back.triggered)
        {
            _cr.Close();
        }

        if (_present.triggered)
        {
            _cr.Present();
            _cr.HasPresented += Present;
        }
    }

    void Present(EvidenceSO evidence)
    {
        IEnumerator coroutine = _cr.WaitThenLoop(evidence, _menu._evidenceDialogue, _menu._wrongEvidence);
        StartCoroutine(coroutine);
    }
}