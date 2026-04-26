using System;
using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private string pillarTag = "Pillar";
    [SerializeField] private float autoClearAfterFallingSeconds = 10f;

    public event Action<Board> Landed;

    private Collider2D col;
    private Rigidbody2D rb;
    private bool isFalling;
    private bool isLanded;
    private Coroutine autoClearCoroutine;


    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        // 按空格前禁止木板移动：冻结位置（也禁止旋转）
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void Update()
    {
        // 在结束时禁止木板继续移动：直接把水平速度设为0（但不冻结，保持物理状态），让其自然停下来
        if (PutGameManager.Instance.isGameOver)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    public void StartFalling()
    {
        if (isLanded) return;
        if (isFalling) return;

        isFalling = true;

        if (rb != null)
        {
            // 解冻：允许物理运动（不禁止旋转）
            rb.constraints = RigidbodyConstraints2D.None;
            rb.WakeUp();
        }

        if (autoClearCoroutine != null) StopCoroutine(autoClearCoroutine);
        autoClearCoroutine = StartCoroutine(AutoClearAfterFalling());
    }

    private IEnumerator AutoClearAfterFalling()
    {
        if (autoClearAfterFallingSeconds > 0f)
            yield return new WaitForSeconds(autoClearAfterFallingSeconds);

        // 超时仍未落地，强制结束，避免一直堆积
        if (!isLanded)
        {
            isLanded = true;
            isFalling = false;
            Landed?.Invoke(this);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLanded) return;
        if (!isFalling) return;
        if (!collision.collider.CompareTag(pillarTag)) return;

        // 一碰到任意柱子就认为落地结束（你设定板一定能碰到至少一根）
        isLanded = true;
        isFalling = false;

        // 落地后立刻锁定，避免后续物理抖动
        if (rb != null)
        {
            // 不锁定、不冻结，让其保留物理状态（包括旋转）
            // rb.velocity = Vector2.zero;
            // rb.angularVelocity = 0f;
            // rb.constraints = RigidbodyConstraints2D.FreezeAll;

            if (PutGameManager.Instance != null)
            {
                float pillarVx = -PutGameManager.Instance.pillarMoveSpeed; // 柱子往左
                rb.velocity = new Vector2(pillarVx, rb.velocity.y);
            }
        }

        Landed?.Invoke(this);
    }

    /// <summary>
    /// 落地后调用一次：返回最终同时接触到的柱子数量（去重）。
    /// </summary>
    public int CheckPillarCount()
    {
        if (col == null) return 0;

        Bounds b = col.bounds;
        Collider2D[] hits = Physics2D.OverlapBoxAll(b.center, b.size, 0f);

        int count = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D h = hits[i];
            if (h == null || h == col) continue;
            if (!h.CompareTag(pillarTag)) continue;

            bool duplicated = false;
            for (int j = 0; j < i; j++)
            {
                if (hits[j] == h) { duplicated = true; break; }
            }

            if (!duplicated) count++;
        }
        if (count >= 2)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 落地后如果接触到2根或以上柱子，锁定旋转（但不锁定位置），让其保持稳定
        }

        return count;
    }
}