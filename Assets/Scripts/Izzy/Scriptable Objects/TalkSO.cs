using UnityEngine;

[CreateAssetMenu(fileName = "New Talk", menuName = "AceAttorney GDW/Talk Prompt")]
public class TalkSO : ScriptableObject 
{
    public string Name;
    public DialogueSO DialogueSO;
    public bool talked;
}