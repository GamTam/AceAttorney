using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "AceAttorney GDW/Dialogue")]
public class DialogueSO : ScriptableObject 
{
    [Header("Mandatory")]
    public TBLine[] dialogueText;
    public DialogueSO nextLine;
    
    [Header("Optional")]
    public Response[] responses;

    public bool HasResponses => responses != null && responses.Length > 0;
    public bool HasNextLine => nextLine != null;

    [Header("Cross Examination")] 
    public bool isCrossExamination;
    public DialogueSO prevLine;
    public DialogueSO pressSequence;
    public DialogueSO wrongPresentSequence;
    public DialogueSO presentSequence;
    public EvidenceSO[] evidence;

    public bool HasPresentSequence => presentSequence != null;
    public bool HasWrongPresentSequence => wrongPresentSequence != null;
    public bool HasPressingSequence => pressSequence != null;

    public EvidenceSO[] ReturnListOfEvidence() {
        return evidence;
    }
}