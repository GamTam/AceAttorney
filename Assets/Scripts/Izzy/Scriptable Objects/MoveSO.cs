using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "AceAttorney GDW/Move Prompt")]
public class MoveSO : ScriptableObject 
{
    public string Name;
    public Sprite Preview;
    public string Scene;
}