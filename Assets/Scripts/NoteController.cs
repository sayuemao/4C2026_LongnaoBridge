using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NoteController.cs - 单个节奏点
public class NoteController : MonoBehaviour
{
    public float speed;
    public Transform judgeArea;
    public float judgeWidth;

    public System.Action<NoteController> OnNoteMissed;

    private bool hasBeenJudged = false;
    private JudgeState currentState = JudgeState.Waiting;

    enum JudgeState
    {
        Waiting,        // 还没到判定区
        InJudgeArea,    // 在判定区内
        Passed          // 已过判定区
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

        // 超出左侧边界销毁
        if (transform.position.x < judgeArea.position.x - 2f)
        {
            Destroy(gameObject);
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
            return JudgeAccuracy.Ok;
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
            case JudgeAccuracy.Ok:
                text = "还行";
                color = Color.cyan;
                break;
        }

        UIManager.Instance.ShowFloatingText(text, color, transform.position);

        // 特效
        EffectManager.Instance.PlayHitEffect(transform.position, accuracy);

        // 销毁节奏点
        Destroy(gameObject);
    }

    void OnMissed()
    {
        if (hasBeenJudged) return;

        hasBeenJudged = true;
        UIManager.Instance.ShowFloatingText("错过...", Color.gray, transform.position);
        OnNoteMissed?.Invoke(this);

        // 渐隐消失
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float alpha = 1f;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * 3f;
            sr.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}

public enum JudgeAccuracy
{
    Perfect,
    Good,
    Ok
}
