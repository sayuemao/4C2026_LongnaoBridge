using Fungus;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PutGameManager : MonoBehaviour
{
    public static PutGameManager Instance { get; private set; }

    [SerializeField] private Transform boardSpawnPoint;
    [SerializeField] private Board[] boardPrefabs;

    [SerializeField] private float spawnDelayAfterSettle = 2f;

    [Header("Pillars")]
    [SerializeField] private Transform[] pillars;
    [SerializeField] private float pillarSpacing = 4f;
    [SerializeField] private Transform pillarLeftResetPoint;
    public float pillarMoveSpeed = 4f;
    private float pillarOffsetX = 0f;
    private float[] pillarBaseX;
    private Rigidbody2D[] pillarBodies;

    public int score;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownSeconds = 20f;



    private float remainingSeconds;

    private Board currentBoard;
    private Coroutine spawnCoroutine;

    public bool isPaused = false;
    public bool isBegin = false;
    public bool isGameOver = false;
    public bool finishOverTextShowed = false;
    

    private bool canDrop = true;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void Update()
    {
        if (isPaused) return;
        CheckStart();
        CountDown();
        if (isGameOver)
        {
                if (!finishOverTextShowed)
                {
                    finishOverTextShowed = true;
            }
            return;
        }
        CheckInput();
    }
    private void FixedUpdate()
    {
        if (isGameOver) return;
        UpdatePillars();
    }
    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentBoard != null && canDrop)
        {
            canDrop = false;
            currentBoard.StartFalling();
        }
    }

    private void CountDown()
    {
        if (remainingSeconds > 0f)
        {
            remainingSeconds -= Time.deltaTime;
            if (remainingSeconds < 0f)
            {
                remainingSeconds = 0f;
                isGameOver = true;
            }
            UpdateCountdownText();
        }
    }

    private void CheckStart()
    {
        if (!isBegin && DragGameManager.Instance.IsdragGameWin)
        {
            score = 0;
            UpdateScoreText();

            remainingSeconds = countdownSeconds;
            UpdateCountdownText();

            SpawnBoard();
            if (pillars != null)
            {
                pillarBaseX = new float[pillars.Length];
                pillarBodies = new Rigidbody2D[pillars.Length];

                for (int i = 0; i < pillars.Length; i++)
                {
                    pillarBaseX[i] = pillars[i].position.x;

                    pillarBodies[i] = pillars[i].GetComponent<Rigidbody2D>();
                    if (pillarBodies[i] != null)
                        pillarBodies[i].bodyType = RigidbodyType2D.Kinematic;
                }
            }
            isBegin = true;
        }
    }

    private void UpdatePillars()
    {
        if (pillars == null || pillars.Length == 0) return;
        if (pillarBaseX == null || pillarBaseX.Length != pillars.Length) return;

        // 追赶式：用累计偏移量计算“应该在的位置”，避免每帧叠加误差
        pillarOffsetX -= pillarMoveSpeed * Time.fixedDeltaTime;

        // 整体左移
        for (int i = 0; i < pillars.Length; i++)
        {
            Transform p = pillars[i];
            if (p == null) continue;

            float targetX = pillarBaseX[i] + pillarOffsetX; // 往左走：offsetX为负
            Vector2 targetPos = new Vector2(targetX, p.position.y);

            Rigidbody2D body = pillarBodies != null ? pillarBodies[i] : null;
            if (body != null)
                body.MovePosition(targetPos);
            else
                p.position = new Vector3(targetPos.x, targetPos.y, p.position.z);
        }

        if (pillarLeftResetPoint == null) return;
        float leftResetX = pillarLeftResetPoint.position.x;

        // 出界则放到最右侧后面，保持间距
        for (int i = 0; i < pillars.Length; i++)
        {
            Transform p = pillars[i];
            if (p == null) continue;

            if (p.position.x <= leftResetX)
            {
                Transform rightMost = GetRightMostPillar();
                if (rightMost == null) return;

                float newX = rightMost.position.x + pillarSpacing;
                Vector3 newPos = new Vector3(newX, p.position.y, p.position.z);

                Rigidbody2D body = pillarBodies != null ? pillarBodies[i] : null;
                if (body != null)
                    body.position = newPos; // 瞬移复位用 position 直接赋值即可
                else
                    p.position = newPos;

                // 同步“基准位置”，保证追赶式不跳回去
                pillarBaseX[i] = newX - pillarOffsetX;
            }
        }
    }

    private Transform GetRightMostPillar()
    {
        Transform rightMost = null;
        float maxX = float.MinValue;

        for (int i = 0; i < pillars.Length; i++)
        {
            Transform p = pillars[i];
            if (p == null) continue;

            if (p.position.x > maxX)
            {
                maxX = p.position.x;
                rightMost = p;
            }
        }

        return rightMost;
    }

    private void SpawnBoard()
    {
        if (boardPrefabs == null || boardPrefabs.Length == 0 || boardSpawnPoint == null) return;

        int randomIndex = Random.Range(0, boardPrefabs.Length);
        Board boardPrefab = boardPrefabs[randomIndex];

        currentBoard = Instantiate(boardPrefab, boardSpawnPoint.position, boardSpawnPoint.rotation);
        currentBoard.Landed += OnBoardLanded;
        canDrop = true;
    }

    private void OnBoardLanded(Board board)
    {
        board.Landed -= OnBoardLanded;

        // 先结算
        Settle(board);

        // 再延迟生成下一块（避免重复启动协程）
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnBoardAfterDelay());
    }

    private void Settle(Board board)
    {
        int pillarCount = board.CheckPillarCount();
        if (pillarCount >= 2) score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateCountdownText()
    {
        if (countdownText != null)
            countdownText.text = "Time left: " + Mathf.CeilToInt(remainingSeconds);
    }

    private IEnumerator SpawnBoardAfterDelay()
    {
        if (spawnDelayAfterSettle > 0f)
            yield return new WaitForSeconds(spawnDelayAfterSettle);

        if (isGameOver) yield break;

        SpawnBoard();
        spawnCoroutine = null;
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
