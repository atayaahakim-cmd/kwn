
using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterial",
    menuName = "PKN/Material")]
public class MaterialData : ScriptableObject
{
    public string title;

    [TextArea(5, 12)]
    public string content;
}
