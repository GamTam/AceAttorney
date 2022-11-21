using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "AceAttorney GDW/Dialogue")]
public class DialogueSO : ScriptableObject 
{
    [Header("Mandatory")]
    [TextArea(10, 5)] public string[] dialogueText;
    public DialogueSO nextLine;
    
    [Header("Optional")]
    public Response[] responses;

    public bool HasResponses => responses != null && responses.Length > 0;
    
    [Header("Cross Examination")] 
    public DialogueSO prevLine;
    public DialogueSO pressSequence;
    public DialogueSO wrongPresentSequence;
    public DialogueSO presentSequence;
    public EvidenceSO[] evidence;
}