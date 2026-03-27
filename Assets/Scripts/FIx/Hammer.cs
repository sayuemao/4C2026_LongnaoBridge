using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField] private float multiplyer = 1.2f;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float hitCheckRadius = 0.2f;

    private Vector3 startPos;
    private Vector3 normalScale;

    private void Start()
    {
        startPos = transform.position;
        normalScale = transform.localScale;
    }

    private void OnMouseEnter() => transform.localScale = normalScale * multiplyer;

    private void OnMouseExit() => transform.localScale = normalScale;

    private void OnMouseDrag()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p.z = transform.position.z;
        transform.position = p;
    }

    private void OnMouseUp()
    {
        // 用锤子当前位置检测（比用鼠标点更直观）
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitCheckRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null) continue;


            if (!hit.CompareTag("Error")) continue;

            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, hit.bounds.center, Quaternion.identity);

            Destroy(hit.gameObject);
            FixGameManager.Instance.AddError();
        }

        transform.position = startPos;
        transform.localScale = normalScale;
    }
}
