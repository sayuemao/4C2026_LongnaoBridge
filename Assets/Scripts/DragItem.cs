using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTest : MonoBehaviour
{
    private Vector2 startPos;
    [SerializeField] private Transform targetPos;
    [SerializeField] private float multiplyer = 1.2f;
    private bool isFinished = false;
    private Vector3 normalScale;

    void Start()
    {
        startPos = transform.position;
        normalScale = transform.localScale;
    }

    private void OnMouseDrag()
    {
        if (isFinished) return;
        if(DragGameManager.Instance.IsdragGameOver) return;
        if (!DragGameManager.Instance.IsdragGameStarted) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z; // 保持原本z，避免被相机z影响
        transform.position = mouseWorld;
    }

    private void OnMouseEnter()
    {
        if (isFinished) return;
        if (DragGameManager.Instance.IsdragGameOver) return;
        if (!DragGameManager.Instance.IsdragGameStarted) return;
        transform.localScale = normalScale * multiplyer;
    }

    private void OnMouseExit()
    {
        transform.localScale = normalScale;
    }

    private void OnMouseUp()
    {
        if (isFinished) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target2D = targetPos.position;

        if (Vector2.Distance(mousePos, target2D) < 0.5f)
        {
            Vector3 snapped = targetPos.position;
            snapped.z = transform.position.z;
            transform.position = snapped;

            isFinished = true;
            transform.localScale = normalScale;
            DragGameManager.Instance.currentdragCount++;
        }
        else
        {
            transform.position = startPos;
        }
    }
}
