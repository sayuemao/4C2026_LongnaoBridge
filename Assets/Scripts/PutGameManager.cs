using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutGameManager : MonoBehaviour
{
    public static PutGameManager Instance { get; private set; }

    [SerializeField] private Board boardPrefab;
    [SerializeField] private Transform boardSpawnPoint;

    public int Score { get; private set; }

    private Board currentBoard;

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
        SpawnNextBoard();
    }

    private void SpawnNextBoard()
    {
        if (boardPrefab == null || boardSpawnPoint == null) return;

        currentBoard = Instantiate(boardPrefab, boardSpawnPoint.position, boardSpawnPoint.rotation);
    }

    public bool IsCurrentBoard(Board board) => board == currentBoard;

    public void OnBoardFinished(bool success)
    {
        if (success) Score++;

        // 简单：结算后立刻生成下一块
        SpawnNextBoard();
    }
}
