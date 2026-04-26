using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragItem : MonoBehaviour
{
    private Vector2 startPos;
    [SerializeField] private Transform targetPos;
    [SerializeField] private float multiplyer = 1.2f;
    private bool isFinished = false;
    private Vector3 normalScale;

    private Transform startParent;

    void Awake()
    {
        startPos = transform.position;
        normalScale = transform.localScale;
        startParent = transform.parent;
    }

 
    public void ResetSelf()
    {
        isFinished = false;
        transform.SetParent(startParent, true);
        transform.position = startPos;
        transform.localScale = normalScale;
    }
    private void OnMouseDown()
    {
        if (isFinished) return;
        if (DragGameManager.Instance.IsdragGameOver) return;
        if (!DragGameManager.Instance.IsdragGameStarted) return;
        if (DragGameManager.Instance.isPaused) return;
        AudioManager.Instance.PlaySFX(0);
    }
    private void OnMouseDrag()
    {
        if (isFinished) return;
        if(DragGameManager.Instance.IsdragGameOver) return;
        if (!DragGameManager.Instance.IsdragGameStarted) return;
        if(DragGameManager.Instance.isPaused) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z; // 保持原本z，避免被相机z影响
        transform.position = mouseWorld;
    }

    private void OnMouseEnter()
    {
        if (isFinished) return;
        if (DragGameManager.Instance.IsdragGameOver) return;
        if (!DragGameManager.Instance.IsdragGameStarted) return;
        if (DragGameManager.Instance.isPaused) return;

        transform.localScale = normalScale * multiplyer;
    }

    private void OnMouseExit()
    {
        if (isFinished) return;
        if (DragGameManager.Instance.IsdragGameOver) return;
        if (!DragGameManager.Instance.IsdragGameStarted) return;
        if (DragGameManager.Instance.isPaused) return;
        transform.localScale = normalScale;
    }

    private void OnMouseUp()
    {
        if (isFinished) return;
        if (DragGameManager.Instance.IsdragGameOver) return;
        if (!DragGameManager.Instance.IsdragGameStarted) return;
        if (DragGameManager.Instance.isPaused) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target2D = targetPos.position;

        if (Vector2.Distance(mousePos, target2D) < 1f)
        {
            Vector3 snapped = targetPos.position;
            snapped.z = transform.position.z;
            transform.position = snapped;

            transform.SetParent(targetPos, true);
            transform.localScale = Vector3.one;

            isFinished = true;
            DragGameManager.Instance.currentdragCount++;
        }
        else
        {
            transform.position = startPos;
        }
    }

}
