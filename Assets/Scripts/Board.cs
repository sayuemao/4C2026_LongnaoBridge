using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3f;

    private readonly HashSet<Collider2D> touchingPillars = new HashSet<Collider2D>();
    private bool isFalling;
    private bool isFinished;
    private Rigidbody2D rb;

    private Transform landedPillar; // 落地时绑定的柱子

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    private void Start()
    {
        isFalling = false;
        isFinished = false;
    }
    private void Update()
    {
        if (!isFalling || isFinished) return;

        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        if (isFinished) return;
        if (PutGameManager.Instance == null) return;
        if (!PutGameManager.Instance.IsCurrentBoard(this)) return;

        isFalling = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFinished) return;
        if (!collision.collider.CompareTag("Pillar")) return;

        touchingPillars.Add(collision.collider);

        // 第一次接触到柱子时，记录要跟随的柱子
        if (landedPillar == null)
            landedPillar = collision.collider.transform;

        if (isFalling)
        {
            isFalling = false;
            FinishAndReport();
        }
    }

    

    private void FinishAndReport()
    {
        if (isFinished) return;
        isFinished = true;

        bool success = touchingPillars.Count >= 2;

        // 跟随：成为柱子的子物体（会跟着柱子移动）
        if (landedPillar != null)
            transform.SetParent(landedPillar, true);

        // 锁死：避免物理抖动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        PutGameManager.Instance.OnBoardFinished(success);
    }
}