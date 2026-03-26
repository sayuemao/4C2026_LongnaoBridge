using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PileManager.cs - 木桩管理
public class PileManager : MonoBehaviour
{
    public static PileManager Instance { get; private set; }

    public GameObject pilePrefab;         // 木桩预制体
    public float pileSpawnOffset = 2f;    // 木桩生成的X轴偏移
    public float leftMoveDuration = 0.5f; // 木桩左移动画持续时间
    public float destroyPositionX = -5f;  // 木桩销毁的X坐标位置
    public int maxPilesOnScreen = 5;      // 屏幕上最大木桩数量

    public Transform pileSpawnPoint;
    public List<Transform> piles = new List<Transform>();           // 多个木桩
    public List<float> pileHeights = new List<float>();              // 每个木桩的初始高度
    public float hitDownAmount = 0.2f;   // 每次打击下降距离
    public float minHeight = -2f;         // 最低高度（完全夯实）

    private int currentPileIndex = 0;     // 当前木桩索引
    private Queue<Transform> pilePool = new Queue<Transform>();     // 木桩对象池
    private bool isMoving = false;        // 是否正在移动木桩

    public Shader highlightShader;        // 高亮shader
    private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>(); // 保存原始材质

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
        // 初始化现有的木桩
        for (int i = 0; i < piles.Count; i++)
        {
            pileHeights.Add(piles[i].localPosition.y);
        }

        // 生成初始木桩
        GenerateInitialPiles();

        // 初始化高亮状态
        HighlightPile();
    }

    // 生成初始木桩
    void GenerateInitialPiles()
    {
        if (piles.Count == 0 && pilePrefab != null)
        {
            for (int i = 0; i < maxPilesOnScreen; i++)
            {
                SpawnNewPile(i * pileSpawnOffset);
            }
        }
    }

    public void OnHitPile(JudgeAccuracy accuracy)
    {
        if (GameManager.Instance.levelComplete)
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
        float delta = Mathf.Min(Time.deltaTime, 0.033f);

        while (elapsed < duration)
        {
            elapsed += delta;
            float t = elapsed / duration;
            float y = Mathf.Lerp(startY, targetY, t);
            pile.localPosition = new Vector3(pile.localPosition.x, y, pile.localPosition.z);
            yield return null;
        }

        // 确保精确到达目标位置
        pile.localPosition = new Vector3(pile.localPosition.x, targetY, pile.localPosition.z);
    }

    void OnPileCompleted()
    {
        // 当前木桩锤好后立即切换到下一个木桩
        currentPileIndex++;

        // 确保currentPileIndex不超出范围
        if (currentPileIndex >= piles.Count)
        {
            currentPileIndex = piles.Count - 1;
        }

        // 提示切换到下一个木桩
        Debug.Log($"下一个木桩！{currentPileIndex} Complete!");

        // 更新高亮状态
        HighlightPile();

        // 同时启动左移动画
        StartCoroutine(MoveAllPilesLeft());
    }

    void HighlightPile()
    {
        // 高亮当前木桩的逻辑
        for (int i = 0; i < piles.Count; i++)
        {
            if (!originalMaterials.ContainsKey(piles[i]))
            {
                Renderer renderer = piles[i].GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    originalMaterials[piles[i]] = renderer.material;
                }
            }

            // 恢复原始材质
            Renderer r = piles[i].GetComponent<Renderer>();
            if (r != null && originalMaterials.ContainsKey(piles[i]))
            {
                r.material = originalMaterials[piles[i]];
            }
        }

        // 高亮当前木桩
        if (currentPileIndex >= 0 && currentPileIndex < piles.Count)
        {
            Transform currentPile = piles[currentPileIndex];
            Renderer renderer = currentPile.GetComponent<Renderer>();

            if (renderer != null && highlightShader != null)
            {
                Material highlightMaterial = new Material(highlightShader);

                // 保持原始颜色，高亮颜色
                if (originalMaterials.ContainsKey(currentPile))
                {
                    highlightMaterial.SetColor("_BaseColor", originalMaterials[currentPile].color);
                }

                renderer.material = highlightMaterial;
            }
        }
    }

    // 生成新木桩
    void SpawnNewPile(float spawnX)
    {
        Transform newPile = GetPileFromPool();

        if (newPile == null && pilePrefab != null)
        {
            GameObject pileObject = Instantiate(pilePrefab, pileSpawnPoint);
            newPile = pileObject.transform;
        }

        if (newPile != null)
        {
            newPile.localPosition = new Vector3(spawnX, 0, 0);
            newPile.gameObject.SetActive(true);
            piles.Add(newPile);
            pileHeights.Add(0f);
        }
    }

    // 从对象池获取木桩
    Transform GetPileFromPool()
    {
        if (pilePool.Count > 0)
        {
            Transform pile = pilePool.Dequeue();
            pile.gameObject.SetActive(true);
            return pile;
        }
        return null;
    }

    // 将木桩放回对象池
    void ReturnPileToPool(Transform pile)
    {
        pile.gameObject.SetActive(false);
        pilePool.Enqueue(pile);
    }

    // 移动所有木桩向左的协程
    IEnumerator MoveAllPilesLeft()
    {
        isMoving = true;

        List<Vector3> startPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();

        // 记录开始位置和目标位置
        for (int i = 0; i < piles.Count; i++)
        {
            startPositions.Add(piles[i].localPosition);

            if (i > 0)
            {
                // 移动到前一个木桩的位置，使用pileHeights中的目标高度
                targetPositions.Add(new Vector3(piles[i - 1].localPosition.x, pileHeights[i], piles[i].localPosition.z));
            }
            else
            {
                // 第一个木桩向左移动一个偏移量，使用pileHeights中的目标高度
                targetPositions.Add(new Vector3(piles[i].localPosition.x - pileSpawnOffset, pileHeights[i], piles[i].localPosition.z));
            }
        }

        // 执行移动动画
        float elapsed = 0;
        float delta = Mathf.Min(Time.deltaTime, 0.033f);
        while (elapsed < leftMoveDuration)
        {
            elapsed += delta;
            float t = elapsed / leftMoveDuration;

            // 使用缓动函数，先快速后减缓
            t = EaseOutQuad(t);

            for (int i = 0; i < piles.Count; i++)
            {
                piles[i].localPosition = Vector3.Lerp(startPositions[i], targetPositions[i], t);
            }

            yield return null;
        }

        // 确保精确到达目标位置
        for (int i = 0; i < piles.Count; i++)
        {
            piles[i].localPosition = targetPositions[i];
        }

        // 检查并销毁左侧超出屏幕的木桩
        CheckAndDestroyOffscreenPiles();

        // 生成新的木桩在最右侧
        float lastPileX = piles[piles.Count - 1].localPosition.x;
        SpawnNewPile(lastPileX + pileSpawnOffset);

        // 更新高亮状态（新木桩生成后）
        HighlightPile();

        isMoving = false;
    }

    // 检查并销毁超出屏幕左侧的木桩
    void CheckAndDestroyOffscreenPiles()
    {
        List<int> pilesToRemove = new List<int>();

        for (int i = 0; i < piles.Count; i++)
        {
            if (piles[i].localPosition.x < destroyPositionX)
            {
                pilesToRemove.Add(i);
            }
        }

        // 从后往前移除，避免索引错乱
        for (int i = pilesToRemove.Count - 1; i >= 0; i--)
        {
            int index = pilesToRemove[i];
            Transform pileToRemove = piles[index];

            // 恢复原始材质
            if (originalMaterials.ContainsKey(pileToRemove))
            {
                Renderer renderer = pileToRemove.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 保持原始颜色
                    renderer.material = originalMaterials[pileToRemove];
                }
                originalMaterials.Remove(pileToRemove);
            }

            ReturnPileToPool(pileToRemove);
            piles.RemoveAt(index);
            pileHeights.RemoveAt(index);

            // 如果移除的是当前木桩或之前的木桩，调整currentPileIndex
            if (index <= currentPileIndex)
            {
                currentPileIndex--;
            }
        }

        // 确保currentPileIndex有效
        currentPileIndex = Mathf.Max(0, currentPileIndex);
    }

    // 缓动函数 - 二次方缓出
    float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }
}