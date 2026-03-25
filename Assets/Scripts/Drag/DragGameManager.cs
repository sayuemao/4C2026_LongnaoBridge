using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragGameManager : MonoBehaviour
{
    public static DragGameManager Instance { get; private set; }


    [SerializeField] private TMP_Text countdownText;   // ÍÏ×§UI Text½øÀ´
    private float Timer = 0;

    [SerializeField] private float dragCountDown1 = 10f;
    [SerializeField] private float dragCountDown2 = 20f;
    [SerializeField] int ImageCount = 5;
    public int currentdragCount = 0;

    private int currentdragStage = 1;

    public bool IsdragGameStarted;
    public bool IsdragGameOver;
    public bool IsdragGameWin;

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
        RestartDragGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartDragGame();
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
            if (countdownText != null)
                countdownText.text = "MemoryTime left:" + Mathf.CeilToInt(Timer);
        }
        if (!IsdragGameOver && currentdragStage == 2)
        {
            if (Timer <= 0f)
            {
                IsdragGameOver = true;
                countdownText.text = "Time's up! Game Over!";
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
                return;
            }
            if (Timer < 0f) Timer = 0f;

            if (countdownText != null)
                countdownText.text = "RestoreTime left:" + Mathf.CeilToInt(Timer);
        }
    }

    private void RestartDragGame()
    {
        Timer = dragCountDown1;
        currentdragStage = 1;
        currentdragCount = 0;

        IsdragGameOver = false;
        IsdragGameWin = false;

        if (countdownText != null)
            countdownText.text = "MemoryTime left:" + Mathf.CeilToInt(Timer);
        foreach (var item in FindObjectsOfType<DragItem>())
            item.ResetSelf();

        foreach (var t in FindObjectsOfType<TargetItem>())
            t.ResetSelf();
    }
}
