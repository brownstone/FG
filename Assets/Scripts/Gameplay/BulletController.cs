using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    [HideInInspector]
    public Vector2 movingDirection = Vector2.zero;
    [HideInInspector]
    public float movingSpeed = 0f;

    public void MoveBullet()
    {
        StartCoroutine(Moving());
    }

    IEnumerator Moving()
    {
        while (gameObject.activeInHierarchy)
        {
            Vector2 pos = transform.position;
            pos += movingDirection * movingSpeed * Time.deltaTime;
            transform.position = pos;
            yield return null;

            Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.x > 1.1f || viewportPos.x < -0.1f || viewportPos.y > 1.1f || viewportPos.y < -0.1f)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Ball"))
    //    {
    //        BallController ballControl = collision.GetComponent<BallController>();
    //        if (ballControl.IsVisible)
    //        {
    //            SoundManager.Instance.PlaySound(SoundManager.Instance.bulletExplore);
    //            Vector2 contactPos = collision.bounds.ClosestPoint(transform.position);
    //            Vector3 lookDirection = (collision.transform.position - transform.position).normalized;
    //            GameManager.Instance.PlayBulletExplore(contactPos, lookDirection);
    //            gameObject.SetActive(false);
    //        }
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            BallController ballControl = collision.GetComponent<BallController>();
            if (ballControl)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.bulletExplore);
                Vector2 contactPos = collision.bounds.ClosestPoint(transform.position);
                Vector3 lookDirection = (collision.transform.position - transform.position).normalized;
                GameManager.Instance.PlayBulletExplore(contactPos, lookDirection);
                gameObject.SetActive(false);
            }
        }
    }
}
