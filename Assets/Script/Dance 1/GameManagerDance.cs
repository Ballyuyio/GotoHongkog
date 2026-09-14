using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // เพิ่มบรรทัดนี้เพื่อใช้คำสั่งโหลดฉากใหม่ (Restart)

public class DanceGameManager : MonoBehaviour
{
    public static DanceGameManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hitText;

    [Header("UI หน้าจอต่างๆ")]
    public Image dimBackground;
    public GameObject startMenuPanel;
    public GameObject gameOverPanel; // เพิ่มหน้าจอ Game Over

    [Header("ระบบเลือด (HP)")]
    public int maxHP = 10; // เลือดสูงสุด (ตั้งค่าได้ใน Inspector)
    private int currentHP;
    public Slider hpBar; // ใช้ UI Slider มาทำหลอดเลือด

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
        // ตั้งค่าเลือดเริ่มต้น
        currentHP = maxHP;
        if (hpBar != null)
        {
            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
        }

        UpdateUI();
        if (hitText != null) hitText.text = "";

        if (mainCamera != null)
        {
            cameraOriginalPos = mainCamera.position;
        }

        if (dimBackground != null)
        {
            Color c = dimBackground.color;
            c.a = 0f;
            dimBackground.color = c;
            dimBackground.gameObject.SetActive(false);
        }

        // ปิดหน้า Game Over ไว้ก่อนตอนเริ่มเกม
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (startMenuPanel != null) startMenuPanel.SetActive(true);

        NoteSpawner spawner = FindObjectOfType<NoteSpawner>();
        if (spawner != null) spawner.enabled = false;
    }

    public void StartGame()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(false);

        NoteSpawner spawner = FindObjectOfType<NoteSpawner>();
        if (spawner != null) spawner.enabled = true;
    }

    // ฟังก์ชันสำหรับปุ่ม Restart (เล่นใหม่)
    public void RestartGame()
    {
        // โหลด Scene ปัจจุบันซ้ำอีกครั้ง เพื่อเริ่มเกมใหม่หมด
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (hitText != null)
        {
            Vector3 targetScale = isGameEnded ? Vector3.one * 3f : Vector3.one;
            hitText.transform.localScale = Vector3.Lerp(hitText.transform.localScale, targetScale, Time.deltaTime * 10f);
        }

        if (isGameEnded && dimBackground != null)
        {
            Color c = dimBackground.color;
            c.a = Mathf.Lerp(c.a, 0.8f, Time.deltaTime * 3f);
            dimBackground.color = c;
        }

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
                hitText.transform.localScale = Vector3.one * 4f;

                hitText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                hitText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                hitText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                hitText.rectTransform.sizeDelta = new Vector2(1200f, 400f);
                hitText.rectTransform.anchoredPosition = new Vector2(-540f, 0f);
                hitText.alignment = TextAlignmentOptions.Center;
            }

            if (dimBackground != null) dimBackground.gameObject.SetActive(true);

            NoteSpawner spawner = FindObjectOfType<NoteSpawner>();
            if (spawner != null) spawner.enabled = false;

            GameObject[] remainingNotes = GameObject.FindGameObjectsWithTag("Note");
            foreach (GameObject note in remainingNotes)
            {
                Destroy(note);
            }
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

        // ----------------------------------------------------
        // ระบบลดเลือดเมื่อพลาด
        // ----------------------------------------------------
        currentHP--;
        if (hpBar != null) hpBar.value = currentHP;

        // ถ้าเลือดหมด (น้อยกว่าหรือเท่ากับ 0) ให้เรียกฟังก์ชัน Game Over
        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    // ฟังก์ชันจัดการตอนแพ้เกม
    // ฟังก์ชันจัดการตอนแพ้เกม
    void GameOver()
    {
        isGameEnded = true;

        // ล้างข้อความทิ้งไปเลย จะได้ไม่มีตัวอักษรสีแดงลอยอยู่กลางจอ
        if (hitText != null)
        {
            hitText.text = "";
        }

        if (dimBackground != null) dimBackground.gameObject.SetActive(true);

        // เปิดหน้าต่าง Game Over (ปุ่ม Restart)
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        NoteSpawner spawner = FindObjectOfType<NoteSpawner>();
        if (spawner != null) spawner.enabled = false;

        GameObject[] remainingNotes = GameObject.FindGameObjectsWithTag("Note");
        foreach (GameObject note in remainingNotes)
        {
            Destroy(note);
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