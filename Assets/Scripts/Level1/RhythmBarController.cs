using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RhythmBarController.cs - 判定条控制器
public class RhythmBarController : MonoBehaviour
{
    public Transform judgeArea;        // 判定区域（固定位置）
    public Transform startJudgePoint;
    public GameObject notePrefab;       // 音符预制体
    public Transform noteSpawnPoint;    // 生成点（左侧）
    public Transform noteDestroyPoint;  // 销毁点（右侧/判定区域）

    [Header("节奏模式设置")]
    public float[] rhythmPattern = { 1f, 0.8f, 1.2f, 0.6f, 1f }; // 节奏模式
    public bool isRandomPattern = false; // 是否使用随机模式
    public float minPattern;
    public float maxPattern;
    public float noteSpeed = 2f;        // 音符移动速度
    public float judgeWidth = 0.5f;     // 判定区域宽度

    [Header("判定区域宽度偏移")]
    public float judgeOffset = 0.3f; // 判定区域偏移
    public List<NoteController> activeNotes = new List<NoteController>();

    private Coroutine spawnNotesCoroutine;
    public bool hasStartedSpawnNotes = false;

    void Start()
    {
        if (judgeArea != null && judgeArea.GetComponent<SpriteRenderer>() != null)
        {
            judgeWidth = judgeArea.GetComponent<SpriteRenderer>().bounds.size.x / 2f + judgeOffset;
        }
    }

    void OnDestroy()
    {
        if (spawnNotesCoroutine != null)
        {
            StopCoroutine(spawnNotesCoroutine);
            spawnNotesCoroutine = null;
        }

        foreach (var note in activeNotes)
        {
            if (note != null)
            {
                note.OnNoteMissed -= HandleNoteMissed;
            }
        }
        activeNotes.Clear();
    }

    public void ResetController()
    {
        if (spawnNotesCoroutine != null)
        {
            StopCoroutine(spawnNotesCoroutine);
            spawnNotesCoroutine = null;
        }

        hasStartedSpawnNotes = false;

        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i] != null)
            {
                activeNotes[i].OnNoteMissed -= HandleNoteMissed;
                if (ObjectPoolManager.Instance != null)
                {
                    ObjectPoolManager.Instance.ReturnObject(activeNotes[i].gameObject);
                }
                else
                {
                    Destroy(activeNotes[i].gameObject);
                }
            }
        }
        activeNotes.Clear();
    }

    void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.startSpawnNotes)
        {
            if (spawnNotesCoroutine == null && !hasStartedSpawnNotes)
            {
                spawnNotesCoroutine = StartCoroutine(SpawnNotes());
                hasStartedSpawnNotes = true;
            }
        }
    }

    IEnumerator SpawnNotes()
    {
        int patternIndex = 0;

        while (true)
        {
            if (ObjectPoolManager.Instance == null || notePrefab == null)
            {
                yield break;
            }

            GameObject note = ObjectPoolManager.Instance.GetObject(notePrefab);

            if (note == null)
            {
                yield return null;
                continue;
            }

            NoteController nc = note.GetComponent<NoteController>();

            if (nc == null)
            {
                if (ObjectPoolManager.Instance != null)
                {
                    ObjectPoolManager.Instance.ReturnObject(note);
                }
                yield return null;
                continue;
            }

            nc.transform.SetParent(transform);
            nc.startJudgePoint = startJudgePoint;
            nc.endJudgePoint = noteDestroyPoint;
            nc.speed = noteSpeed;
            nc.judgeArea = judgeArea;
            nc.judgeWidth = judgeWidth;
            nc.OnNoteMissed += HandleNoteMissed;
            activeNotes.Add(nc);

            note.transform.SetPositionAndRotation(noteSpawnPoint.position, Quaternion.identity);

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
        if (note != null)
        {
            activeNotes.Remove(note);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMissFeedback();
        }
    }
}
