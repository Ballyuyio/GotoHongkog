using UnityEngine;
using UnityEngine.InputSystem; // 1. เพิ่ม namespace ของ New Input System

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ความลึกสูงสุดที่ต้องการตรวจจับลงไปใต้น้ำ")]
    [SerializeField] private float interactDistance = 10f;
    [SerializeField] private string targetTag = "Interactable";

    [Header("Offset Settings")]
    [Tooltip("ปรับจุดปล่อย Raycast เช่น เลื่อนลงต่ำกว่าตัวเรือ")]
    [SerializeField] private Vector3 rayOriginOffset = new Vector3(0f, -0.5f, 0f);

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = true;

    void Update()
    {
        CheckUndergroundInteractable();
    }

    private void CheckUndergroundInteractable()
    {
        Vector3 origin = transform.position + rayOriginOffset;
        Vector3 direction = Vector3.down;
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, interactDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                // 2. เช็คการกดปุ่ม E ด้วย New Input System
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    ExecuteInteraction(hit.collider.gameObject);
                }
            }
        }
    }

    private void ExecuteInteraction(GameObject targetObject)
    {
        Debug.Log("Interact สำเร็จกับ: " + targetObject.name);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugRay) return;

        Vector3 origin = transform.position + rayOriginOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, Vector3.down * interactDistance);
    }
}