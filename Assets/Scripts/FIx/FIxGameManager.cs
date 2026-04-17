using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixGameManager : MonoBehaviour
{
    public static FixGameManager Instance { get; private set; }

    [Header("Stage Settings")]
    [SerializeField] private GameObject rainStageObjects;
    [SerializeField] private GameObject riverStageObjects;
    [SerializeField] private GameObject windStageObjects;

    [Header("Game Rules")]
    [SerializeField] private int totalErrorCount = 3;
    [SerializeField] private float stageTransitionDelay = 1f;
    public int currentErrorCount = 0;

    [Header("Hint")]
    [SerializeField] private float hintDelay = 15f;
    [SerializeField] private string hintObjectName = "hint";
    private float hintTimer = 0f;
    private bool hintActivated = false;

    private enum WeatherType
    {
        Rain,
        River,
        Wind,
    }

    [SerializeField] private WeatherType currentWeather;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // 初始化游戏，只激活第一个阶段
        InitializeGame();
    }

    private void Update()
    {
        UpdateHintTimer();
    }

    private void InitializeGame()
    {
        currentWeather = WeatherType.Rain;
        currentErrorCount = 0;
        AudioManager.Instance.PlaySFXLoop(8); // 播放雨声
        rainStageObjects.SetActive(true);
        riverStageObjects.SetActive(false);
        windStageObjects.SetActive(false);

        ResetHintTimer();
        // 您可以在这里重置其他游戏状态
    }

    public void AddError()
    {
        currentErrorCount++;
        if (currentErrorCount >= totalErrorCount)
        {
            CompleteStage();
        }
    }

    public void CompleteStage()
    {
        // 不再直接调用 GameOver，而是启动阶段转换
        StartCoroutine(TransitionToNextStage());
    }

    private IEnumerator TransitionToNextStage()
    {
        currentErrorCount = 0; // 重置错误计数，准备进入下一个阶段
        // 等待一小段时间，可以播放阶段完成的动画或音效
        yield return new WaitForSeconds(stageTransitionDelay);

        // 根据当前阶段，切换到下一个阶段
        switch (currentWeather)
        {
            case WeatherType.Rain:
                AudioManager.Instance.StopSFXLoop(8); // 停止雨声
                currentWeather = WeatherType.River;
                rainStageObjects.SetActive(false);
                riverStageObjects.SetActive(true);
                break;
            case WeatherType.River:
                currentWeather = WeatherType.Wind;
                riverStageObjects.SetActive(false);
                windStageObjects.SetActive(true);
                AudioManager.Instance.PlaySFXLoop(9); // 播放风声
                break;
            case WeatherType.Wind:
                AudioManager.Instance.StopSFXLoop(9); // 停止风声
                // 所有阶段都已完成
                Debug.Log("Game Over! You have completed all levels.");
                // 在这里可以触发游戏胜利的UI或事件
                yield break; // 结束协程
        }

        // 为新阶段重置错误计数
        currentErrorCount = 0;
        ResetHintTimer();
        // 您也可以在这里更新 totalErrorCount，如果每个阶段的目标不同
    }

    private void UpdateHintTimer()
    {
        if (hintActivated) return;

        GameObject currentStageRoot = GetCurrentStageRoot();
        if (currentStageRoot == null || !currentStageRoot.activeInHierarchy) return;

        hintTimer += Time.deltaTime;
        if (hintTimer >= hintDelay)
        {
            ActivateHintsInCurrentStage(currentStageRoot);
            hintActivated = true;
        }
    }

    private void ResetHintTimer()
    {
        hintTimer = 0f;
        hintActivated = false;
    }

    private GameObject GetCurrentStageRoot()
    {
        switch (currentWeather)
        {
            case WeatherType.Rain:
                return rainStageObjects;
            case WeatherType.River:
                return riverStageObjects;
            case WeatherType.Wind:
                return windStageObjects;
            default:
                return null;
        }
    }

    private void ActivateHintsInCurrentStage(GameObject stageRoot)
    {
        int activatedCount = 0;
        Transform[] children = stageRoot.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < children.Length; i++)
        {
            Transform child = children[i];
            if (child == stageRoot.transform) continue;

            if (child.name.Equals(hintObjectName, StringComparison.OrdinalIgnoreCase))
            {
                child.gameObject.SetActive(true);
                activatedCount++;
            }
        }

    }
}
