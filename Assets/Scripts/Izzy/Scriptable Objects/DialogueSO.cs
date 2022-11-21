using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "AceAttorney GDW/Dialogue")]
public class DialogueSO : ScriptableObject 
{
    [Header("Mandatory")]
    [TextArea(10, 5)] public string[] dialogueText;
    public DialogueSO nextLine;
    
    [Header("Optional")]
    public Choice[] choices;
    
    [Header("Cross Examination")] 
    public DialogueSO prevLine;
    public DialogueSO pressSequence;
    public DialogueSO wrongPresentSequence;
    public DialogueSO presentSequence;
    public EvidenceSO[] evidence;
}

public class Choice
{
    public string name;
    public DialogueSO dialogue;
}