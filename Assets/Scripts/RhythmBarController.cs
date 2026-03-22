using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RhythmBarController.cs - 判定条控制器
public class RhythmBarController : MonoBehaviour
{
    public Transform judgeArea;        // 判定区域（固定位置）
    public GameObject notePrefab;       // 节奏点预制体
    public Transform noteSpawnPoint;    // 生成点（右侧）
    public Transform noteDestroyPoint;  // 销毁点（左侧/判定区后）

    public float noteSpeed = 2f;        // 节奏点移动速度
    public float judgeWidth = 0.5f;     // 判定区域宽度

    public List<NoteController> activeNotes = new List<NoteController>();

    void Start()
    {
        judgeWidth = judgeArea.GetComponent<SpriteRenderer>().bounds.size.x / 2f; // 根据判定区宽度自动设置判定范围
        StartCoroutine(SpawnNotes());
    }

    IEnumerator SpawnNotes()
    {
        float[] rhythmPattern = { 1f, 0.8f, 1.2f, 0.6f, 1f }; // 节奏间隔
        int patternIndex = 0;

        while (true)
        {
            // 生成新的节奏点
            GameObject note = Instantiate(notePrefab, noteSpawnPoint.position, Quaternion.identity);
            NoteController nc = note.GetComponent<NoteController>();
            nc.transform.SetParent(transform); // 让节奏点成为判定条的子对象
            nc.speed = noteSpeed;
            nc.judgeArea = judgeArea;
            nc.judgeWidth = judgeWidth;
            nc.OnNoteMissed += HandleNoteMissed;  // 错过事件
            activeNotes.Add(nc);

            // 等待下一个节奏点
            yield return new WaitForSeconds(rhythmPattern[patternIndex]);
            patternIndex = (patternIndex + 1) % rhythmPattern.Length;
        }
    }

    void HandleNoteMissed(NoteController note)
    {
        // 节奏点错过（没按/按晚）
        activeNotes.Remove(note);
        UIManager.Instance.ShowMissFeedback();
    }
}
