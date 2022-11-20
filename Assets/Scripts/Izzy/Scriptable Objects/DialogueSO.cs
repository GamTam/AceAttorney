using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "AceAttorney GDW/Dialogue")]
public class DialogueSO : ScriptableObject 
{
    [Header("Mandatory")]
    [TextArea(10, 5)] public string[] dialogueText;

    [Header("Cross Examination")] 
    public DialogueSO prevLine;
    public DialogueSO nextLine;
    public DialogueSO pressSequence;
    public DialogueSO wrongPresentSequence;
    public DialogueSO presentSequence;
    public EvidenceSO[] evidence;
}