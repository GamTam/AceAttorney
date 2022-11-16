using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(15, 20)] [SerializeField] private string[] _dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartText(_dialogue);
    }
}