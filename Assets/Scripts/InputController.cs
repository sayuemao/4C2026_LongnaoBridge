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
        // 检测所有可能的按键
        foreach (KeyCode key in hitKeys)
        {
            if (Input.GetKeyDown(key))
            {
                OnPlayerPress();
                break;
            }
        }

        // 也支持鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            OnPlayerPress();
        }
    }

    void OnPlayerPress()
    {
        // 找到当前在判定区内的节奏点
        NoteController noteInJudge = FindNoteInJudgeArea();

        if (noteInJudge != null)
        {
            noteInJudge.OnPlayerPress();
        }
        else
        {
            // 判定区没有节奏点，按空了
            UIManager.Instance.ShowFloatingText("按早了!", Color.red);
        }
    }

    NoteController FindNoteInJudgeArea()
    {
        // 找到所有在判定区内的节奏点
        // 这里简化处理，实际应该通过列表查找
        return null;
    }
}
