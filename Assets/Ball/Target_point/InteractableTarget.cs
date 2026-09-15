using UnityEngine;

public class InteractableTarget : MonoBehaviour
{
    [Header("Information")]
    public string targetName;
    
    [TextArea(2, 5)]
    public string[] descriptionParagraphs; // ใส่กี่ท่อนก็ได้ผ่าน Inspector

    [Header("Images")]
    public Sprite[] displayImages; // ใส่กี่รูปก็ได้
}