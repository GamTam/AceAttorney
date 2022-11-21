using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSO _dialogue;
    public bool _inspected;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartText(_dialogue);
        _inspected = true;
    }
}