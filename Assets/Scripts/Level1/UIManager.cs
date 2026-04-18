using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
// UIManager.cs - UI管理
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TextMeshProUGUI floatingTextPrefab;
    public Transform floatingTextPosition;
    public float floatingTextDestroyDelay = 1f;

    public Transform missShowPosition; // 错过反馈显示位置

    public Transform judgeBar;  // 判定条UI
    public RectTransform perfectZone; // 完美区域显示

    public Image reflectImage; // 小方块显示判定结果
    private Color defaultReflectColor;
    public float reflectImageDuration = 0.5f;
    private Coroutine reflectImageCoroutine;

    public TextMeshProUGUI countdownText; // 显示倒计时的文本
    public float countdownTime = 1f; // 倒计时时间,分钟

    public TextMeshProUGUI scoreText; // 显示分数的文本
    private int maxScore = 9999; // 最大分数
    public int currentScore = 0; // 当前分数
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        defaultReflectColor = reflectImage.color;
        countdownTime *= 60;// 转换为秒
        scoreText.text = "0";
    }

    private void Update()
    {
        ShowCountdown();
    }
    
    private void ShowCountdown()
    {
        // 显示倒计时
        if (countdownTime < 0)
        {
            countdownTime = 0;
            TimeOut();
        }
        int fenzhong = (int)countdownTime / 60;
        int seconds = (int)countdownTime % 60;
        countdownText.text = $"{fenzhong:00}:{seconds:00}";
        countdownTime -= Time.deltaTime;
    }

    public void UpdateScore(int score)
    {
        // 更新分数显示
        currentScore = Mathf.Min(currentScore + score, maxScore);
        scoreText.text = currentScore.ToString();
    }
    public void SetupJudgeBar()
    {
        // 设置判定区域的视觉
        // 可以分成不同颜色区域：完美区（金色）、良好区（绿色）、普通区（蓝色）
    }

    public void ShowFloatingText(string text, Color color, Vector3? worldPos = null)
    {
        // 显示判定文字
        TextMeshProUGUI floatingText = Instantiate(floatingTextPrefab, floatingTextPosition);
        floatingText.text = text;
        floatingText.color = color;
        if (worldPos.HasValue)
        {
            floatingText.transform.position = worldPos.Value;
        }
        else
        {
            floatingText.transform.position = floatingTextPosition.position;
        }
        Destroy(floatingText.gameObject, floatingTextDestroyDelay);

        ShowReflectImage(color, reflectImageDuration);
    }

    public void ShowMissFeedback()
    {
        // 显示错过反馈
        ShowFloatingText("Miss", Color.red, missShowPosition.position);
    }

    public void ShowReflectImage(Color color, float duration = 0.5f)
    {
        if (reflectImageCoroutine != null)
        {
            StopCoroutine(reflectImageCoroutine);
        }
        reflectImageCoroutine = StartCoroutine(ShowReflectImageCoroutine(color, duration));
    }
    private IEnumerator ShowReflectImageCoroutine(Color color, float duration = 0.5f)
    {
        // 显示小方块反馈颜色       
        reflectImage.color = color;
        yield return new WaitForSeconds(duration);
        reflectImage.color = defaultReflectColor;
    }

    public void BackToSelectLevel()
    {
        // 返回选择选择界面
        GameManager.Instance.BackToSelectLevel();
    }

    private void TimeOut()
    {
        // 时间到，结束游戏
        GameManager.Instance.LevelComplete();
    }
}
