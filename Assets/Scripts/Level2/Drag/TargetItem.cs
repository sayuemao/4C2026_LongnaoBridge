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
        if(DragGameManager.Instance==null)
        {
            return;
        }
        if (completeHide == false && DragGameManager.Instance.IsdragGameStarted)
        {
            spriteRenderer.enabled = false;
            completeHide = true;
        }
    }

    public void ResetSelf()
    {
        completeHide = false;
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }
}
