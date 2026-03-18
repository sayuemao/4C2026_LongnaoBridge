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
    
    [SerializeField] private float dragCountDown1 = 0f;
    [SerializeField] private float dragCountDown2 = 5f;
    [SerializeField] int ImageCount = 3;
    public int currentdragCount = 0;
    
    private int currentdragStage = 1;

    public bool IsdragGameStarted;
    public bool IsdragGameOver;

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
        Timer = dragCountDown1; 
        IsdragGameOver = false;
    }

    private void Update()
    {
        if(currentdragStage==1)
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
        if (!IsdragGameOver&&currentdragStage==2)
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
                countdownText.text = "Congratulations! You win!";
                return;
            }
            if (Timer < 0f) Timer = 0f;

            if (countdownText != null)
                countdownText.text = "RestoreTime left:" + Mathf.CeilToInt(Timer);
        }
    }
}
