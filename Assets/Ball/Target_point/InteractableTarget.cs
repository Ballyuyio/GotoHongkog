using UnityEngine;

public class InteractableTarget : MonoBehaviour
{
    [Header("Title / Header")]
    [Tooltip("หัวข้อหลักที่จะนำไปแสดงใน TitleText")]
    public string targetTitle = "USS Johnston";
    
    [Tooltip("หัวข้อย่อยหรือชื่อวัตถุ (เผื่อไว้ใช้งานเพิ่มเติม)")]
    public string subTitle = "DD-557";

    [Header("Description Content")]
    [TextArea(3, 8)]
    [Tooltip("ใส่ข้อความประวัติ/ข้อมูลทีละย่อหน้า")]
    public string[] descriptionParagraphs;

    [Header("Images")]
    [Tooltip("ใส่รูปภาพประกอบ")]
    public Sprite[] displayImages;
}