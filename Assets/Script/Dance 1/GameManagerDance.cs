using UnityEngine;
using UnityEngine.UI; // ต้องเพิ่มบรรทัดนี้เพื่อใช้งาน UI Image
using TMPro;

public class DanceGameManager : MonoBehaviour
{
    public static DanceGameManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hitText;

    [Header("ลาก UI จอดำ (Dim Background) มาใส่ช่องนี้")]
    public Image dimBackground;

    public Transform mainCamera;
    private Vector3 cameraOriginalPos;
    private float shakeTimer = 0f;

    private int score = 0;
    private int combo = 0;

    private int currentLevel = 1;
    private bool isGameEnded = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
        if (hitText != null) hitText.text = "";

        if (mainCamera != null)
        {
            cameraOriginalPos = mainCamera.position;
        }

        // ซ่อนจอดำไว้ก่อนตอนเริ่มเกม
        if (dimBackground != null)
        {
            Color c = dimBackground.color;
            c.a = 0f; // ตั้งค่าความโปร่งใสเป็น 0 (มองไม่เห็น)
            dimBackground.color = c;
            dimBackground.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. ระบบจัดการตัวหนังสือ
        if (hitText != null)
        {
            // ถ้าจบเกมแล้ว ให้ตัวหนังสือค้างไว้ที่ขนาด 3 เท่า แต่ถ้ายังไม่จบให้หดกลับมาที่ขนาด 1
            Vector3 targetScale = isGameEnded ? Vector3.one * 3f : Vector3.one;
            hitText.transform.localScale = Vector3.Lerp(hitText.transform.localScale, targetScale, Time.deltaTime * 10f);
        }

        // 2. ระบบเฟดหน้าจอมืดตอนจบเกม
        if (isGameEnded && dimBackground != null)
        {
            Color c = dimBackground.color;
            // ค่อยๆ เปลี่ยนความโปร่งใสจากเดิม ไปหยุดที่ 0.8f (มืด 80%)
            c.a = Mathf.Lerp(c.a, 0.8f, Time.deltaTime * 3f);
            dimBackground.color = c;
        }

        // 3. ระบบสั่นกล้อง
        if (shakeTimer > 0 && mainCamera != null)
        {
            mainCamera.position = cameraOriginalPos + Random.insideUnitSphere * 0.1f;
            shakeTimer -= Time.deltaTime;
        }
        else if (mainCamera != null)
        {
            mainCamera.position = cameraOriginalPos;
        }
    }

    public void AddScore(int amount, string message, Color textColor)
    {
        if (isGameEnded) return;

        score += amount;
        combo++;
        UpdateUI();

        if (hitText != null)
        {
            hitText.text = message;
            hitText.color = textColor;
            hitText.transform.localScale = Vector3.one * 1.5f;
        }

        if (score >= 3500 && !isGameEnded)
        {
            isGameEnded = true;

            if (hitText != null)
            {
                hitText.text = "STAGE CLEAR!";
                hitText.color = Color.yellow;
                // เด้งขยายใหญ่ไปที่ 4 เท่าก่อน แล้วโค้ดด้านบนจะค่อยๆ ดึงกลับมาหยุดค้างไว้ที่ 3 เท่า
                hitText.transform.localScale = Vector3.one * 4f;
            }

            // เปิดใช้งานจอดำให้พร้อมสำหรับการค่อยๆ มืดลง (Fade)
            if (dimBackground != null) dimBackground.gameObject.SetActive(true);

            NoteSpawner spawner = FindObjectOfType<NoteSpawner>();
            if (spawner != null) spawner.enabled = false;

            GameObject[] remainingNotes = GameObject.FindGameObjectsWithTag("Note");
            foreach (GameObject note in remainingNotes)
            {
                Destroy(note);
            }

            Debug.Log("จบมินิเกมแล้ว! หน้าจอกำลังมืดลง");
        }
        else if (score >= 2000 && currentLevel == 2)
        {
            currentLevel = 3;
            if (hitText != null)
            {
                hitText.text = "MAX SPEED!";
                hitText.color = Color.red;
                hitText.transform.localScale = Vector3.one * 2.5f;
            }
            NoteSpawner spawner = FindObjectOfType<NoteSpawner>();
            if (spawner != null) spawner.IncreaseDifficulty(3);
        }
        else if (score >= 1000 && currentLevel == 1)
        {
            currentLevel = 2;
            if (hitText != null)
            {
                hitText.text = "SPEED UP!";
                hitText.color = Color.magenta;
                hitText.transform.localScale = Vector3.one * 2.5f;
            }
            NoteSpawner spawner = FindObjectOfType<NoteSpawner>();
            if (spawner != null) spawner.IncreaseDifficulty(2);
        }
    }

    public void MissNote()
    {
        if (isGameEnded) return;

        combo = 0;
        UpdateUI();

        if (hitText != null)
        {
            hitText.text = "Miss!";
            hitText.color = Color.red;
            hitText.transform.localScale = Vector3.one * 1.5f;
        }
    }

    public void ShakeCamera(float duration)
    {
        shakeTimer = duration;
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score + "\nCombo: " + combo;
        }
    }
}