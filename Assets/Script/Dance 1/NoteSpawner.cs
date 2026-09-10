using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("ใส่ Prefab โน้ตสั้น 4 ทิศ (0=ซ้าย, 1=ขึ้น, 2=ลง, 3=ขวา)")]
    public GameObject[] notePrefabs = new GameObject[4];

    [Header("ใส่ Prefab โน้ตยาว 4 ทิศ (เรียงแบบเดียวกัน)")]
    public GameObject[] longNotePrefabs = new GameObject[4];

    [Header("ใส่จุดเกิด Spawn 4 เลน")]
    public Transform[] spawnPoints = new Transform[4];

    [Header("ตั้งค่าความเร็วการเสก")]
    public float spawnInterval = 1f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandomNote();
            timer = 0f;
        }
    }

    void SpawnRandomNote()
    {
        int lane = Random.Range(0, 4);

        // สุ่ม 20% โอกาสที่จะเป็น Long Note, 80% เป็นโน้ตธรรมดา (ปรับตัวเลข 0.8f ได้)
        bool isLongNote = Random.value > 0.8f;
        GameObject prefabToSpawn = isLongNote ? longNotePrefabs[lane] : notePrefabs[lane];

        GameObject spawnedNote = Instantiate(prefabToSpawn, spawnPoints[lane].position, Quaternion.identity);

        if (lane == 0) spawnedNote.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (lane == 1) spawnedNote.transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (lane == 2) spawnedNote.transform.rotation = Quaternion.Euler(0, 0, -90);
        else if (lane == 3) spawnedNote.transform.rotation = Quaternion.Euler(0, 0, 180);
    }
}