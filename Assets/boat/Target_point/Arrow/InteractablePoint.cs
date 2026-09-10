using UnityEngine;

public class InteractablePoint : MonoBehaviour
{
    [Header("Info")]
    public string locationName = "จุดสำรวจใต้ทะเล";
    [TextArea] public string description = "กด E เพื่อสำรวจ";

    [Header("Settings")]
    public float interactRadius = 15f; // ระยะที่อนุญาตให้กด Interact ได้
    public bool isExplored = false;     // สำรวจไปแล้วหรือยัง

    // ทำ Gizmo วงกลมสีเหลืองให้เห็นระยะ Interact ชัดๆ ในหน้า Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}