using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JudgeAccuracy
{
    Perfect,
    Good,
    OK,
}
// NoteController.cs - 单个节奏点
public class NoteController : MonoBehaviour
{
    public float speed;
    public Transform judgeArea;
    public float judgeWidth;

    public System.Action<NoteController> OnNoteMissed;

    public bool hasBeenJudged = false;


    enum JudgeState
    {
        Waiting,        // 还没到判定区
        InJudgeArea,    // 在判定区内
        Passed          // 已过判定区
    }
    private JudgeState currentState = JudgeState.Waiting;

    [Space]
    [Header("销毁前的延迟时间")]
    public float destroyDelay = 0.1f; // 销毁前的延迟时间

    [Header("跳跃隐退参数")]
    public float jumpHeight = 0.5f;          // 跳跃高度（世界单位）
    public float retreatDistance = 1.0f;     // 向左退的距离（世界单位）
    public float disappearDuration = 0.6f;   // 隐退消失动画时长（秒）

    private SpriteRenderer rhythmBarSR;
    private SpriteRenderer sr;//自己的SpriteRenderer

    private Vector3 originalScale;
    private Color originalColor;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        originalColor = sr.color;
    }

    void Start()
    {
        rhythmBarSR = transform.parent.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        hasBeenJudged = false;
        currentState = JudgeState.Waiting;
        transform.localScale = originalScale;
        sr.color = originalColor;
    }

    void OnDisable()
    {
        OnNoteMissed = null;
    }

    void Update()
    {
        // 向左移动
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // 检测是否进入判定区域
        float distanceToJudge = Mathf.Abs(transform.position.x - judgeArea.position.x);

        if (!hasBeenJudged && distanceToJudge <= judgeWidth)
        {
            currentState = JudgeState.InJudgeArea;
        }
        else if (currentState == JudgeState.InJudgeArea && distanceToJudge > judgeWidth)
        {
            // 已经离开判定区域还没被按，算错过
            currentState = JudgeState.Passed;
            OnMissed();
        }

        // 超出左侧边界回收
        if (transform.position.x < rhythmBarSR.bounds.min.x - 2f)
        {
            ObjectPoolManager.Instance.ReturnObject(gameObject);
        }
    }

    // 玩家按下按键时调用
    public void OnPlayerPress()
    {
        if (hasBeenJudged) return;

        // 计算判定结果
        float distanceToJudgeCenter = Mathf.Abs(transform.position.x - judgeArea.position.x);

        if (distanceToJudgeCenter <= judgeWidth)
        {
            // 在判定区内
            JudgeAccuracy accuracy = GetAccuracy(distanceToJudgeCenter);
            OnJudged(accuracy);
        }
        else if (transform.position.x > judgeArea.position.x + judgeWidth)
        {
            // 按早了
            hasBeenJudged = true;
            StartCoroutine(FadeOut());
            UIManager.Instance.ShowFloatingText("按早了!", Color.red);
        }
        // 按晚了的情况由上面的OnMissed处理
    }

    JudgeAccuracy GetAccuracy(float distance)
    {
        float judgeRange = judgeWidth;

        if (distance <= judgeRange * 0.2f)
            return JudgeAccuracy.Perfect;
        else if (distance <= judgeRange * 0.6f)
            return JudgeAccuracy.Good;
        else
            return JudgeAccuracy.OK;
    }

    void OnJudged(JudgeAccuracy accuracy)
    {
        hasBeenJudged = true;

        // 通知PileManager：木桩下降
        PileManager.Instance.OnHitPile(accuracy);

        // 显示判定文字
        string text = "";
        Color color = Color.white;

        switch (accuracy)
        {
            case JudgeAccuracy.Perfect:
                text = "完美！";
                color = Color.yellow;
                break;
            case JudgeAccuracy.Good:
                text = "不错";
                color = Color.green;
                break;
            case JudgeAccuracy.OK:
                text = "还行";
                color = Color.cyan;
                break;
        }

        UIManager.Instance.ShowFloatingText(text, color);

        // 特效
        EffectManager.Instance.PlayHitEffect(transform.position, accuracy);

        // 销毁节奏点
        //Destroy(gameObject);
        StartCoroutine(JumpRetreatDisappear());
    }

    void OnMissed()
    {
        if (hasBeenJudged) return;

        hasBeenJudged = true;
        //UIManager.Instance.ShowFloatingText("错过...", Color.gray);
        OnNoteMissed?.Invoke(this);

        // 渐隐消失
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()// 渐隐消失协程
    {
        yield return new WaitForSeconds(destroyDelay); // 等待一段时间后开始渐隐

        float alpha = 1f;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * 3f;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }

        ObjectPoolManager.Instance.ReturnObject(gameObject);
    }

    IEnumerator JumpRetreatDisappear()
    {
        //yield return new WaitForSeconds(destroyDelay);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.left * retreatDistance; // 向左隐退
        Vector3 startScale = transform.localScale;

        float elapsed = 0f;

        while (elapsed < disappearDuration)
        {
            float t = Mathf.Clamp01(elapsed / disappearDuration);

            // X 线性向左退，Y 用正弦曲线制造抛物线跳跃效果
            float x = Mathf.Lerp(startPos.x, endPos.x, t);
            float y = startPos.y + Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(x, y, startPos.z);

            // 透明度从 1 -> 0
            float alpha = Mathf.Lerp(1f, 0f, t);
            if (sr != null)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

            // 轻微缩小增加消失感
            float scale = Mathf.Lerp(1f, 0.85f, t);
            transform.localScale = startScale * scale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        ObjectPoolManager.Instance.ReturnObject(gameObject);
    }
}


