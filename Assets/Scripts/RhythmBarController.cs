using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RhythmBarController.cs - 判定条控制器
public class RhythmBarController : MonoBehaviour
{
    public Transform judgeArea;        // 判定区域（固定位置）
    public Transform startJudgePoint;
    public GameObject notePrefab;       // 节奏点预制体
    public Transform noteSpawnPoint;    // 生成点（右侧）
    public Transform noteDestroyPoint;  // 销毁点（左侧/判定区后）

    [Header("节奏间隔参数")]
    public float[] rhythmPattern = { 1f, 0.8f, 1.2f, 0.6f, 1f }; // 节奏间隔
    public bool isRandomPattern = false; // 是否随机节奏间隔
    public float minPattern;
    public float maxPattern;
    public float noteSpeed = 2f;        // 节奏点移动速度
    public float judgeWidth = 0.5f;     // 判定区域宽度

    [Header("判定区域宽容区域")]
    public float judgeOffset = 0.3f; // 判定区域偏移量
    public List<NoteController> activeNotes = new List<NoteController>();

    void Start()
    {
        // 初始化判定区域宽度（根据SpriteRenderer组件）
        judgeWidth = judgeArea.GetComponent<SpriteRenderer>().bounds.size.x / 2f + judgeOffset; // 根据判定区宽度自动设置判定范围
        StartCoroutine(SpawnNotes());
    }

    IEnumerator SpawnNotes()
    {

        int patternIndex = 0;

        while (true)
        {
            // 从对象池获取节奏点
            GameObject note = ObjectPoolManager.Instance.GetObject(notePrefab);

            NoteController nc = note.GetComponent<NoteController>();
            nc.transform.SetParent(transform); // 保持世界空间变换不变，避免缩放受父对象影响
            nc.speed = noteSpeed;
            nc.judgeArea = judgeArea;
            nc.judgeWidth = judgeWidth;
            nc.OnNoteMissed += HandleNoteMissed;  // 错过事件
            activeNotes.Add(nc);

            note.transform.position = noteSpawnPoint.position;
            note.transform.rotation = Quaternion.identity;

            //nc.sr.sprite = nc.noteSprites[0];

            // 等待下一个节奏点时间
            if (isRandomPattern)
            {
                float randomDelay = Random.Range(minPattern, maxPattern);
                yield return new WaitForSeconds(randomDelay);
            }
            else
            {
                yield return new WaitForSeconds(rhythmPattern[patternIndex]);
                patternIndex = (patternIndex + 1) % rhythmPattern.Length;
            }

        }
    }

    void HandleNoteMissed(NoteController note)
    {
        // 节奏点错过（没按/按晚）
        activeNotes.Remove(note);
        UIManager.Instance.ShowMissFeedback();
    }
}
