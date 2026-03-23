using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PileManager.cs - 木桩管理
public class PileManager : MonoBehaviour
{
    public static PileManager Instance { get; private set; }

    public Transform[] piles;           // 多个木桩
    public float[] pileHeights;          // 每个木桩的初始高度
    public float hitDownAmount = 0.2f;   // 每次打击下降距离
    public float minHeight = -2f;         // 最低高度（完全夯实）

    private int currentPileIndex = 0;     // 当前要夯的木桩

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
        // 记录初始高度
        pileHeights = new float[piles.Length];
        for (int i = 0; i < piles.Length; i++)
        {
            pileHeights[i] = piles[i].localPosition.y;
        }
    }

    public void OnHitPile(JudgeAccuracy accuracy)
    {
        if(GameManager.Instance.levelComplete)
        {
            return;
        }
        // 根据判定决定下降多少
        float downDistance = hitDownAmount;

        switch (accuracy)
        {
            case JudgeAccuracy.Perfect:
                downDistance *= 1.5f;  // 完美下降更多
                break;
            case JudgeAccuracy.Good:
                downDistance *= 1f;
                break;
            case JudgeAccuracy.OK:
                downDistance *= 0.5f;   // 还行下降少
                break;
        }

        // 当前木桩下降
        Transform currentPile = piles[currentPileIndex];
        float newHeight = pileHeights[currentPileIndex] - downDistance;

        // 不能低于最低高度
        newHeight = Mathf.Max(newHeight, minHeight);
        pileHeights[currentPileIndex] = newHeight;

        // 更新位置（带动画）
        StartCoroutine(MovePileSmooth(currentPile, newHeight));

        // 检查木桩是否完成
        if (newHeight <= minHeight)
        {
            // 当前木桩夯实完成
            OnPileCompleted();
        }
    }

    IEnumerator MovePileSmooth(Transform pile, float targetY)
    {
        float startY = pile.localPosition.y;
        float duration = 0.1f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float y = Mathf.Lerp(startY, targetY, t);
            pile.localPosition = new Vector3(pile.localPosition.x, y, pile.localPosition.z);
            yield return null;
        }
    }

    void OnPileCompleted()
    {
        // 切换到下一个木桩
        currentPileIndex++;

        if (currentPileIndex >= piles.Length)
        {
            // 所有木桩完成，过关
            GameManager.Instance.LevelComplete();
        }
        else
        {
            // 提示切换到下一个木桩
            //UIManager.Instance.ShowFloatingText($"下一个木桩！", Color.cyan);
            Debug.Log($"下一个木桩！{currentPileIndex}"+"Complete!");
            // 可以在UI上高亮下一个要夯的木桩
            HighlightPile();
        }
    }

    void HighlightPile()
    {
        // 高亮当前木桩的逻辑
        for (int i = 0; i < piles.Length; i++)
        {
            // 设置高亮效果
        }
    }
}