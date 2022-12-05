using UnityEngine;

[CreateAssetMenu(fileName = "New Evidence", menuName = "AceAttorney GDW/Evidence")]
public class EvidenceSO : ScriptableObject 
{
    public string Name;     
    public Sprite Icon;
    public Sprite CheckImage;
    [TextArea(3, 4)]public string Description;   
}