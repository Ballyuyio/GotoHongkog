using System.Collections;
using UnityEngine;

public class CameraSwitchFade : MonoBehaviour
{
    [Header("Cameras")]
    public Camera mainCamera;          // กล้องหลักบนผิวน้ำ
    public Camera underwaterCamera;    // กล้องใต้ท้องเรือตัวที่สร้างไว้

    [Header("Fade UI")]
    public CanvasGroup fadeCanvasGroup; // ตัว FadeImage ที่ใส่ CanvasGroup
    public float fadeDuration = 0.5f;   // เวลาในการค่อยๆ มืด/สว่าง (วินาที)

    [Header("Controls")]
    public KeyCode switchKey = KeyCode.C;

    private bool isUnderwater = false;
    private bool isTransitioning = false;

    void Start()
    {
        // เริ่มต้นให้กล้องหลักเปิด กล้องใต้น้ำปิด
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (underwaterCamera != null) underwaterCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey) && !isTransitioning)
        {
            StartCoroutine(SwitchCameraRoutine());
        }
    }

    private IEnumerator SwitchCameraRoutine()
    {
        isTransitioning = true;

        // 1. ค่อยๆ Fade หน้าจอดำ (Fade Out)
        yield return StartCoroutine(Fade(1f));

        // 2. สลับการทำงานของกล้อง 2 ตัว
        isUnderwater = !isUnderwater;
        mainCamera.gameObject.SetActive(!isUnderwater);
        underwaterCamera.gameObject.SetActive(isUnderwater);

        // 3. ค่อยๆ Fade หน้าจอกลับมาสว่าง (Fade In)
        yield return StartCoroutine(Fade(0f));

        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}