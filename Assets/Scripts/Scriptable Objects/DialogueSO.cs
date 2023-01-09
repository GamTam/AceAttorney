using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Ace Attorney/Dialogue")]
public class DialogueSO : ScriptableObject 
{
    [Header("Mandatory")]
    public TBLine[] dialogueText;
    public DialogueSO nextLine;
    
    [Header("Optional")]
    public Response[] responses;

    [Header("Cross Examination")] 
    public bool isCrossExamination;
    public DialogueSO prevLine;
    public DialogueSO pressSequence;
    public DialogueSO wrongPresentSequence;
    public DialogueSO presentSequence;
    public EvidenceSO[] evidence;

    private List<int> secondaryDialogueIndexes = new List<int>();

    public bool HasResponses => responses != null && responses.Length > 0;
    public bool HasNextLine => nextLine != null;
    public bool HasPresentSequence => presentSequence != null;
    public bool HasWrongPresentSequence => wrongPresentSequence != null;
    public bool HasPressingSequence => pressSequence != null;
    public bool HasSecondaryDialogues => secondaryDialogueIndexes.Count > 0;

    public EvidenceSO[] ReturnListOfEvidence() {
        return evidence;
    }
}