using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "AceAttorney GDW/Dialogue")]
public class DialogueSO : ScriptableObject 
{
    public bool isPresentable;
    public EvidenceSO evidence = null;
    public string name;
    [TextArea(10, 5)] public string dialogueText;
    public DialogueSO prevLine;
    public DialogueSO nextLine;
}