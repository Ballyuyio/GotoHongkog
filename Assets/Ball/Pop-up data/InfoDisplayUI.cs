using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplayUI : MonoBehaviour
{
    public static InfoDisplayUI Instance;

    [Header("UI Components")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private CanvasGroup popupCanvasGroup; // ลาก Canvas Group ที่อยู่บน Panel มาใส่
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image previewImage;

    [Header("Animation Settings")]
    [SerializeField] private float animDuration = 0.25f;

    private Coroutine activeAnimCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (popupRoot != null) popupRoot.SetActive(false);
    }

    public void DisplayData(InteractableTarget data)
    {
        if (data == null) return;

        if (titleText != null) titleText.text = data.targetName;

        if (descriptionText != null)
        {
            descriptionText.text = string.Join("\n\n", data.descriptionParagraphs);
        }

        if (previewImage != null)
        {
            if (data.displayImages != null && data.displayImages.Length > 0 && data.displayImages[0] != null)
            {
                previewImage.gameObject.SetActive(true);
                previewImage.sprite = data.displayImages[0];
            }
            else
            {
                previewImage.gameObject.SetActive(false);
            }
        }

        // แสดงผลพร้อมแอนิเมชัน
        if (popupRoot != null)
        {
            popupRoot.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (popupCanvasGroup != null)
            {
                if (activeAnimCoroutine != null) StopCoroutine(activeAnimCoroutine);
                activeAnimCoroutine = StartCoroutine(AnimatePopup(true));
            }
        }
    }

    public void Close()
    {
        if (popupRoot != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (popupCanvasGroup != null && gameObject.activeInHierarchy)
            {
                if (activeAnimCoroutine != null) StopCoroutine(activeAnimCoroutine);
                activeAnimCoroutine = StartCoroutine(AnimatePopup(false));
            }
            else
            {
                popupRoot.SetActive(false);
            }
        }
    }

    // แอนิเมชันแบบ Fade In + Scale Pop ขึ้นมา
    private IEnumerator AnimatePopup(bool opening)
    {
        RectTransform rect = popupCanvasGroup.GetComponent<RectTransform>();
        float elapsed = 0f;

        float startAlpha = opening ? 0f : 1f;
        float endAlpha = opening ? 1f : 0f;

        Vector3 startScale = opening ? Vector3.one * 0.85f : Vector3.one;
        Vector3 endScale = opening ? Vector3.one : Vector3.one * 0.85f;

        popupCanvasGroup.alpha = startAlpha;
        if (rect != null) rect.localScale = startScale;

        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            
            // ใช้ SmoothStep จำลอง CSS cubic-bezier / ease-out
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            popupCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, smoothT);
            if (rect != null) rect.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            yield return null;
        }

        popupCanvasGroup.alpha = endAlpha;
        if (rect != null) rect.localScale = endScale;

        if (!opening)
        {
            popupRoot.SetActive(false);
        }
    }
}