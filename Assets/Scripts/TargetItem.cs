using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetItem : MonoBehaviour
{
    private bool completeHide = false;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (completeHide == false && DragGameManager.Instance.IsdragGameStarted)
        {
            spriteRenderer.enabled = false;
        }
    }
}
