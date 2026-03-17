using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// UIManager.cs - UI管理
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TextMeshProUGUI floatingTextPrefab;
    public Transform judgeBar;  // 判定条UI
    public RectTransform perfectZone; // 完美区域显示

    private void Awake()
    {
        if(Instance!= null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetupJudgeBar()
    {
        // 设置判定区域的视觉
        // 可以分成不同颜色区域：完美区（金色）、良好区（绿色）、普通区（蓝色）
    }

    public void ShowFloatingText(string text, Color color, Vector3? worldPos = null)
    {
        // 显示判定文字
    }

    internal void ShowMissFeedback()
    {
        throw new NotImplementedException();
    }
}
