using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    public ItemType itemType = ItemType.COIN;
    [HideInInspector]
    public float fallingSpeed = 0;

    private SpriteRenderer spRender = null;
    private bool stopFalling = false;
    public void MoveDown()
    {
        stopFalling = false;
        if (spRender == null)
            spRender = GetComponent<SpriteRenderer>();
        StartCoroutine(Falling());
    }

    IEnumerator Falling()
    {
        while (gameObject.activeInHierarchy)
        {
            if (stopFalling)
                yield break;
            Vector2 pos = transform.position;
            pos += Vector2.down * fallingSpeed * Time.deltaTime;
            transform.position = pos;
            yield return null;

            if (GameManager.Instance.GameState == GameState.Revive)
            {
                gameObject.SetActive(false);
                yield break;
            }

            Vector2 checkPos = (Vector2)transform.position + Vector2.up * spRender.bounds.size.y;
            if (checkPos.y < PlayerController.Instance.limitBottom)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.item);
            if (itemType == ItemType.COIN)
            {
                stopFalling = true;
                CoinManager.Instance.AddCoins(1);
                StartCoroutine(MoveToCoinImg());
                return;
            }
            else if (itemType == ItemType.BOMB)
                ItemsManager.Instance.AddBomb(1);
            else if (itemType == ItemType.LASER)
                ItemsManager.Instance.AddLaser(1);
            else if (itemType == ItemType.MISSILE)
                ItemsManager.Instance.AddMissile(1);
            gameObject.SetActive(false);
        }
    }

    IEnumerator MoveToCoinImg()
    {
        Vector2 endPos = UIManager.Instance.GetCoinImgWorldPos();
        Vector2 startPos = transform.position;
        float t = 0;
        float lerpTime = 1f;
        while (t < lerpTime)
        {
            t += Time.deltaTime;
            float factor = t / lerpTime;
            transform.position = Vector2.Lerp(startPos, endPos, factor);
            transform.eulerAngles += new Vector3(0, 350 * Time.deltaTime, 0);
            yield return null;
        }
        transform.eulerAngles = Vector3.zero;
        gameObject.SetActive(false);
    }

}
