using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DragGameManager : MonoBehaviour
{
    public static DragGameManager Instance { get; private set; }


    [SerializeField] private TMP_Text restartText;

    [SerializeField] private GameObject bottomFrame;
    private float Timer = 0;

    [SerializeField] private float dragCountDown1 = 10f;
    [SerializeField] private float dragCountDown2 = 20f;
    [SerializeField] int ImageCount = 5;

    [SerializeField] private GameObject dragPillars;
    [SerializeField] private GameObject putPillars;
    public int currentdragCount = 0;

    private int currentdragStage = 1;

    public bool isPaused = false;

    public bool IsdragGameStarted=false;
    public bool IsdragGameOver = false;
    public bool IsdragGameWin = false;

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
        InitialDragGame();
    }

    private void UpdateCountdownUI(float timeValue)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetCountdownDisplay(timeValue);
        }
    }


    private void Update()
    {
        if (IsdragGameWin) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartDragGame();
            return;
        }
        if (isPaused)
        {
            return;
        }
        if (currentdragStage == 1)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0)
            {
                currentdragStage = 2;
                Timer = dragCountDown2;
                IsdragGameStarted = true;
                return;
            }
            UpdateCountdownUI(Timer);
        }
        if (!IsdragGameOver && currentdragStage == 2)
        {
            if (Timer <= 0f)
            {
                IsdragGameOver = true;
                restartText.gameObject.SetActive(true);
                return;
            }

            if (currentdragCount < ImageCount)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                IsdragGameOver = true;
                IsdragGameWin = true;
                dragPillars.SetActive(false);
                putPillars.SetActive(true);
                return;
            }
            if (Timer < 0f) Timer = 0f;

            UpdateCountdownUI(Timer);
        }
    }
    private void RestartDragGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    private void InitialDragGame()
    {
        Timer = dragCountDown1;
        currentdragStage = 1;
        currentdragCount = 0;

        restartText.gameObject.SetActive(false);
        bottomFrame.SetActive(true);
        dragPillars.SetActive(true);
        putPillars.SetActive(false);

        IsdragGameStarted = false;
        IsdragGameOver = false;
        IsdragGameWin = false;

        UIManager.Instance.useInternalCountdown = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetScoreUIVisible(false);
        }

        UpdateCountdownUI(Timer);

        foreach (var item in FindObjectsOfType<DragItem>())
            item.ResetSelf();

        foreach (var t in FindObjectsOfType<TargetItem>())
            t.ResetSelf();
    }
    public void PauseGame()
    {
        isPaused = true;
    }
    public void StopPauseGame()
    {
        isPaused = false;
    }
}
