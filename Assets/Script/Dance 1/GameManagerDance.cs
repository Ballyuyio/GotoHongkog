using UnityEngine;
using TMPro;

public class DanceGameManager : MonoBehaviour
{
    public static DanceGameManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hitText;

    private int score = 0;
    private int combo = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
        hitText.text = "";
    }

    // เพิ่มการรับค่าสี (Color textColor)
    public void AddScore(int amount, string message, Color textColor)
    {
        score += amount;
        combo++;
        UpdateUI();
        hitText.text = message;
        hitText.color = textColor; // สั่งให้ข้อความเปลี่ยนสีตามที่ส่งมา
    }

    public void MissNote()
    {
        combo = 0;
        UpdateUI();
        hitText.text = "Miss!";
        hitText.color = Color.red; // บังคับให้ Miss เป็นสีแดงเสมอ
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score + "\nCombo: " + combo;
    }
}