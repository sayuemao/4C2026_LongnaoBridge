using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fungus;
using Newtonsoft.Json;
public class EndSceneManager : MonoBehaviour
{
    public SpriteRenderer background;
    public float comeOutSpeedWithFramePercent = 0.5f;
    public TextMeshProUGUI text;
    public string endText;
    public float typeSpeed = 0.05f; // 打字机速度
    public float elementDelay = 0.5f;

    public Flowchart flowchart;

    private bool hasEndDialog = false;
    // Start is called before the first frame update
    void Start()
    {
        InitElements();
        background.gameObject.SetActive(false);
        StartCoroutine(PlayAnimation());
    }
    private void Update()
    {
        if (!hasEndDialog&&flowchart.GetBooleanVariable("moveToNextScene"))
        {
            hasEndDialog = true;
            if(SceneTransitionManager.Instance)
            {
                SceneTransitionManager.Instance.TransitionToScene("EndAnim");
            }
            else
            {
                SceneManager.LoadScene("EndAnim");
            }
        }
    }

    void InitElements()
    {
        text.gameObject.SetActive(false);
    }
    IEnumerator PlayAnimation()
    {
        yield return StartCoroutine(TypeText(text, endText));
        yield return new WaitForSeconds(elementDelay);
        text.gameObject.SetActive(false);
        background.gameObject.SetActive(true);
        yield return StartCoroutine(ComeOutBackground());
        flowchart.ExecuteBlock("主角对白2");
    }
    IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText)
    {
        // 如果组件为空，直接退出协程
        if (textComponent == null) yield break;

        // 先清空文字
        textComponent.gameObject.SetActive(true);
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
    IEnumerator ComeOutBackground()
    {
        float t = 0f;
        while(t<1f)
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, t);
            t += comeOutSpeedWithFramePercent * Time.deltaTime;
            yield return null;
        }
        background.color = new Color(background.color.r, background.color.g, background.color.b, 1f);
        yield return null;
    }


}
