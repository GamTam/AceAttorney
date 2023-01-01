using UnityEngine;

[CreateAssetMenu(fileName = "New Talk", menuName = "Ace Attorney/Talk Prompt")]
public class TalkSO : ScriptableObject 
{
    public string Name;
    public DialogueSO DialogueSO;
    public string[] ConditionFlags;
}