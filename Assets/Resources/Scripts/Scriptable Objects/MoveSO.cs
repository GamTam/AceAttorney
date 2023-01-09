using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Ace Attorney/Move Prompt")]
public class MoveSO : ScriptableObject 
{
    public string Name;
    public Sprite Preview;
    public string Scene;
    public string[] ConditionFlags;
}