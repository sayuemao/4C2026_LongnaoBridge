using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡完成界面动画控制器
/// 功能：按顺序播放文字打字机效果、分隔线展开、面板展开动画
///
/// ==================== Inspector 配置说明 ====================
///
/// 【分隔线 separatorLine 的 Image 组件设置】（必须）：
///   1. Image Type  --> 选择 "Filled"（不是 Simple！）
///   2. Fill Method --> 选择 "Horizontal"（水平方向填充）
///   3. Fill Origin --> 选择 "Left"（从左边开始填充）
///   4. Fill Amount --> 设为 0 或 1 都可以（代码会在 InitElements 中重置为 0）
///
/// 【背景面板 backgroundPanel 的 Image 组件设置】（必须）：
///   1. Image Type  --> 选择 "Filled"
///   2. Fill Method --> 选择 "Horizontal"
///   3. Fill Origin --> 选择 "Left"
///   4. Fill Amount --> 设为 0 或 1 都可以
///
/// 【重要提示】
/// - 此脚本不会修改任何 RectTransform 属性（anchor/pivot/sizeDelta/scale）
/// - 所有UI布局完全由你在 Unity Editor 中的原始设置决定
/// - 动画效果仅通过 Image.fillAmount (0 -> 1) 实现
/// - 如果 Image Type 不是 Filled，运行时会输出警告并降级为直接显示
/// </summary>
public class LevelCompleteAnimator : MonoBehaviour
{
    // ==================== 拖拽到Inspector的UI元素 ====================

    // 章节标题文字（如"第一章 节奏打桩·固基"）
    [Header("文字元素 - 按顺序显示")]
    public TextMeshProUGUI chapterTitle;

    // 章节标题完整文本（用于打字机效果）
    [TextArea(2, 4)]
    public string chapterTitleText = "第一章 节奏打桩·固基";

    // 主标题文字（如"恭喜通关"）
    public TextMeshProUGUI mainTitle;

    // 主标题完整文本
    [TextArea(2, 4)]
    public string mainTitleText = "恭喜通关";

    // 分数文字（如"得分：126"）
    public TextMeshProUGUI scoreText;

    // 分数完整文本（可在代码中动态设置）
    public string scoreTextText = "得分：126";

    // 白色分隔线（横向的Image）
    [Header("分隔线")]
    public Image separatorLine;

    // 黄色背景面板（Image）
    [Header("底部面板")]
    public Image backgroundPanel;

    // 面板内部的描述文字（如"一锤定基..."）
    public TextMeshProUGUI descriptionText;

    // 描述文字完整文本
    [TextArea(3, 6)]
    public string descriptionTextText = "一锤定基，稳扎稳打\n你的节奏感很棒！";

    // ==================== 动画时间参数 ====================

    // 打字机效果：每个字之间的间隔时间（秒）
    // 数值越小，打字速度越快
    [Header("动画设置")]
    public float typeSpeed = 0.05f;

    // 元素与元素之间的等待时间（秒）
    // 例如打完标题后停顿多久再打下一个
    public float elementDelay = 0.5f;

    // 分隔线从左到右展开的动画时长（秒）
    public float lineExpandDuration = 0.8f;

    // 背景面板从左到右展开的动画时长（秒）
    public float panelExpandDuration = 1.0f;

    // ==================== 生命周期方法 ====================

    // Start方法：游戏开始时自动调用
    // 这里直接启动整个动画协程
    private void Start()
    {
        StartCoroutine(PlayFullAnimation());
    }

    // ==================== 主动画流程 ====================
    /// <summary>
    /// 完整动画播放流程（协程）
    /// 使用协程实现：先初始化元素，然后按顺序播放每个动画
    /// yield return 会暂停执行，等待动画完成后再继续下一步
    /// </summary>
    IEnumerator PlayFullAnimation()
    {
        // 第一步：初始化所有UI元素
        // 把文字清空，分隔线和面板先缩小到0
        InitElements();

        // 第二步：打字机效果显示章节标题
        // 使用独立的文本字段（chapterTitleText），而不是组件的text属性
        // 因为InitElements已经把组件的text清空了
        yield return StartCoroutine(TypeText(chapterTitle, chapterTitleText));

        // 等待0.5秒（elementDelay），然后继续下一个
        yield return new WaitForSeconds(elementDelay);

        // 第三步：打字机效果显示主标题"恭喜通关"
        yield return StartCoroutine(TypeText(mainTitle, mainTitleText));
        yield return new WaitForSeconds(elementDelay);

        // 第四步：打字机效果显示分数
        yield return StartCoroutine(TypeText(scoreText, scoreTextText));
        yield return new WaitForSeconds(elementDelay);

        // 第五步：分隔线从左到右展开
        yield return StartCoroutine(ExpandLine(separatorLine));
        yield return new WaitForSeconds(elementDelay);

        // 第六步：背景面板从左到右展开
        yield return StartCoroutine(ExpandPanel(backgroundPanel));

        // 第七步：面板展开完成后，打字机效果显示描述文字
        yield return StartCoroutine(TypeText(descriptionText, descriptionTextText));

        yield return null;

        LevelCompleteSceneManager.Instance.animComplete = true;
    }

    // ==================== 初始化方法 ====================

    /// <summary>
    /// 初始化所有UI元素
    /// 在动画开始前，把它们设置到初始状态
    /// - 文字先清空
    /// - 分隔线和面板通过 fillAmount=0 隐藏（需要Image Type设为Filled）
    ///
    /// 【重要】此方法不会修改任何 RectTransform 属性（anchor/pivot/sizeDelta/scale等），
    /// 所有UI布局保持用户在Inspector中的原始设置不变。
    /// </summary>
    void InitElements()
    {
        // 初始化章节标题
        if (chapterTitle)
        {
            chapterTitle.text = "";  // 先清空文字
            chapterTitle.gameObject.SetActive(true);  // 确保显示
        }

        // 初始化主标题
        if (mainTitle)
        {
            mainTitle.text = "";
            mainTitle.gameObject.SetActive(true);
        }

        // 初始化分数文字
        if (scoreText && scoreText.gameObject.activeSelf)
        {
            scoreText.text = "";
            scoreText.gameObject.SetActive(true);
            scoreTextText = "得分：" + LevelCompleteSceneManager.Instance.levelData.levelScores[LevelCompleteSceneManager.Instance.nowLevelNumber - 1];
            scoreTextText += " 最高：" + LevelCompleteSceneManager.Instance.levelData.levelMaxScores[LevelCompleteSceneManager.Instance.nowLevelNumber - 1];
        }

        // 初始化分隔线：仅将fillAmount设为0，不修改任何transform属性
        // 前提条件：Image组件的 Image Type 必须设为 "Filled"
        if (separatorLine)
        {
            separatorLine.fillAmount = 0f;
        }

        // 初始化背景面板：仅将fillAmount设为0，不修改任何transform属性
        // 前提条件：Image组件的 Image Type 必须设为 "Filled"
        if (backgroundPanel)
        {
            backgroundPanel.fillAmount = 0f;
        }

        // 初始化描述文字
        if (descriptionText)
        {
            descriptionText.text = "";
            descriptionText.gameObject.SetActive(true);
        }
    }

    // ==================== 打字机效果 ====================

    /// <summary>
    /// 打字机效果协程
    /// 功能：文字不是一下子全部显示，而是一个字一个字地出现
    /// </summary>
    /// <param name="textComponent">要显示的文字组件（TextMeshProUGUI）</param>
    /// <param name="fullText">完整的文字内容</param>
    IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText)
    {
        // 如果组件为空，直接退出协程
        if (textComponent == null || !textComponent.gameObject.activeSelf) yield break;

        // 先清空文字
        textComponent.text = "";

        // 遍历文字中的每个字符
        foreach (char c in fullText)
        {
            // 把当前字符追加到文字后面
            textComponent.text += c;
            // 等待一小段时间（typeSpeed），然后继续下一个字符
            // 这样就形成了打字机的效果
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    // ==================== 分隔线展开动画 ====================

    /// <summary>
    /// 分隔线从左到右展开的动画协程
    ///
    /// 【实现原理】使用 Unity Image 组件的 Fill Method（Filled 类型）
    /// 通过 fillAmount 从 0 渐变到 1 实现从左到右的展开效果。
    ///
    /// 【前置条件】separatorLine 的 Image 组件必须在 Inspector 中设置：
    ///   - Image Type = Filled
    ///   - Fill Method = Horizontal
    ///   - Fill Origin = Left
    ///
    /// 此方法不会修改任何 RectTransform 属性（anchor/pivot/sizeDelta/scale等）。
    /// </summary>
    /// <param name="line">分隔线的Image组件（必须为Filled类型）</param>
    IEnumerator ExpandLine(Image line)
    {
        if (line == null) yield break;

        // 检查 Image 是否为 Filled 类型，如果不是则给出警告并跳过
        if (line.type != Image.Type.Filled)
        {
            Debug.LogWarning(
                $"[Level1CompleteAnimator] separatorLine ({line.gameObject.name}) 的 Image Type 不是 Filled！\n" +
                "请在 Inspector 中将 Image Type 改为 \"Filled\"，Fill Method 设为 \"Horizontal\"，Fill Origin 设为 \"Left\"。",
                line.gameObject);
            // 即使不是 Filled 类型，也直接显示完整图像作为降级处理
            line.fillAmount = 1f;
            yield break;
        }

        float elapsed = 0f;  // 已流逝的时间

        // 使用while循环实现平滑动画：fillAmount 从 0 渐变到 1
        while (elapsed < lineExpandDuration)
        {
            // 计算动画进度（0到1之间的比例）
            float progress = elapsed / lineExpandDuration;
            // 只修改 fillAmount，不触碰任何 transform 属性
            line.fillAmount = Mathf.Lerp(0f, 1f, progress);
            // 等待一帧（让动画平滑）
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 动画结束时，确保 fillAmount 精确等于 1（完全显示）
        line.fillAmount = 1f;
    }

    // ==================== 背景面板展开动画 ====================

    /// <summary>
    /// 背景面板从左到右展开的动画协程
    ///
    /// 【实现原理】使用 Unity Image 组件的 Fill Method（Filled 类型）
    /// 通过 fillAmount 从 0 渐变到 1 实现从左到右的展开效果。
    ///
    /// 【前置条件】backgroundPanel 的 Image 组件必须在 Inspector 中设置：
    ///   - Image Type = Filled
    ///   - Fill Method = Horizontal
    ///   - Fill Origin = Left
    ///
    /// 此方法不会修改任何 RectTransform 属性（anchor/pivot/sizeDelta/scale等）。
    /// </summary>
    /// <param name="panel">背景面板的Image组件（必须为Filled类型）</param>
    IEnumerator ExpandPanel(Image panel)
    {
        if (panel == null) yield break;

        // 检查 Image 是否为 Filled 类型，如果不是则给出警告并跳过
        if (panel.type != Image.Type.Filled)
        {
            Debug.LogWarning(
                $"[Level1CompleteAnimator] backgroundPanel ({panel.gameObject.name}) 的 Image Type 不是 Filled！\n" +
                "请在 Inspector 中将 Image Type 改为 \"Filled\"，Fill Method 设为 \"Horizontal\"，Fill Origin 设为 \"Left\"。",
                panel.gameObject);
            // 即使不是 Filled 类型，也直接显示完整图像作为降级处理
            panel.fillAmount = 1f;
            yield break;
        }

        float elapsed = 0f;  // 已流逝的时间

        // 使用while循环实现平滑动画：fillAmount 从 0 渐变到 1
        while (elapsed < panelExpandDuration)
        {
            // 计算动画进度（0到1之间的比例）
            float progress = elapsed / panelExpandDuration;
            // 只修改 fillAmount，不触碰任何 transform 属性
            panel.fillAmount = Mathf.Lerp(0f, 1f, progress);
            // 等待一帧
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 动画结束时，确保 fillAmount 精确等于 1（完全显示）
        panel.fillAmount = 1f;
    }

    // ==================== 公共API ====================

    /// <summary>
    /// 在播放动画前设置分数（必须在Start/PlayFullAnimation之前调用）
    /// 使用示例：animator.SetScore(126);
    /// </summary>
    /// <param name="score">玩家得分</param>
    public void SetScore(int score)
    {
        scoreTextText = $"得分：{score}";
    }

    /// <summary>
    /// 设置自定义描述文字（必须在Start/PlayFullAnimation之前调用）
    /// </summary>
    /// <param name="description">自定义描述文本</param>
    public void SetDescription(string description)
    {
        descriptionTextText = description;
    }
}
