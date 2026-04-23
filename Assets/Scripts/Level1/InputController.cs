using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// InputController.cs
public class InputController : MonoBehaviour
{
    public RhythmBarController rhythmBar;
    public KeyCode[] hitKeys = { KeyCode.Space, KeyCode.J, KeyCode.K }; // 支持多种按键

    void Update()
    {

        // 也支持鼠标点击
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayerPress();
        }
    }

    void OnPlayerPress()
    {
        AudioManager.Instance.PlaySFX(0);// 播放按键音效（若不需要可删去）
        // 找到当前在判定区内的节奏点
        NoteController noteInJudge = FindNoteInJudgeArea();

        if (noteInJudge != null)
        {
            noteInJudge.OnPlayerPress();
        }
        else
        {
            // 判定区没有节奏点，按空了
            NoteController firstNoteOutsideJudge = FindNoteFirstOutsideJudgeArea();
            if (firstNoteOutsideJudge)
            {
                firstNoteOutsideJudge.OnPlayerPress();
            }
        }
    }

    NoteController FindNoteInJudgeArea()
    {
        // 找到所有在判定区内的节奏点
        // 这里简化处理，实际应该通过列表查找
        foreach (NoteController note in rhythmBar.activeNotes)
        {
            if (note.hasBeenJudged) continue; // 已经被判定过的点不再考虑
            float distanceToJudge = Mathf.Abs(note.transform.position.x - rhythmBar.judgeArea.position.x);
            if (distanceToJudge <= rhythmBar.judgeWidth)
            {
                return note;
            }
        }

        return null;
    }

    NoteController FindNoteFirstOutsideJudgeArea()
    {
        // 找到第一个在判定区外的节奏点
        foreach (NoteController note in rhythmBar.activeNotes)
        {
            if (note.hasBeenJudged) continue; // 已经被判定过的点不再考虑
            float distanceToJudge = note.transform.position.x - rhythmBar.judgeArea.position.x;
            if (distanceToJudge > rhythmBar.judgeWidth)
            {
                return note;
            }
        }
        return null;
    }
}
