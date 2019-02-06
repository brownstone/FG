using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum PlayerState
{
    Prepare,
    Living,
    Die,
}

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { private set; get; }
    public static event System.Action<PlayerState> PlayerStateChanged = delegate { };

    public PlayerState PlayerState
    {
        get
        {
            return playerState;
        }

        private set
        {
            if (value != playerState)
            {
                value = playerState;
                PlayerStateChanged(playerState);
            }
        }
    }


    private PlayerState playerState = PlayerState.Die;


    [Header("Player Config")]
    public float limitBottom = -20;
    [SerializeField]
    private float limitTop = -5;

    [Header("Player References")]
    [SerializeField]
    private GameObject centerGun = null;
    [SerializeField]
    private GameObject leftGun = null;
    [SerializeField]
    private GameObject rightGun = null;

    private SpriteRenderer spRender = null;
    private PolygonCollider2D polyCollider = null;
    private SpriteRenderer centerGunRender = null;
    private Vector2 firstPos = Vector2.zero;
    private float limitLeft = 0;
    private float limitRight = 0;
    private int gunCount = 1;
    private bool allowMove = false;

    private int HP = 10;

    private void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }
    private void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    private void GameManager_GameStateChanged(GameState obj)
    {
        if (obj == GameState.Playing)
        {
            PlayerLiving();
        }
        else if (obj == GameState.Prepare)
        {
            PlayerPrepare();
        }
        else if (obj == GameState.Revive || obj == GameState.GameOver)
        {
            PlayerDie();
        }
    }



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

	// Update is called once per frame
	void Update () {

        if (GameManager.Instance.GameState == GameState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                allowMove = false;
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    allowMove = true;
                    firstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else if (Input.GetMouseButton(0) && allowMove)
            {
                Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                float distanceX = Mathf.Abs(Mathf.Abs(firstPos.x) - Mathf.Abs(currentPos.x));
                float distanceY = Mathf.Abs(Mathf.Abs(firstPos.y) - Mathf.Abs(currentPos.y));


                if (currentPos.x < firstPos.x) //Swipe left
                {
                    if (transform.position.x > limitLeft)
                    {
                        transform.position -= new Vector3(distanceX, 0, 0);
                    }
                }
                else //Swipe right
                {
                    if (transform.position.x < limitRight)
                    {
                        transform.position += new Vector3(distanceX, 0, 0);
                    }
                }

                if (currentPos.y < firstPos.y) //Swipe down
                {
                    if (transform.position.y > limitBottom)
                    {
                        transform.position -= new Vector3(0, distanceY, 0);
                    }
                }
                else //Swipe up
                {
                    if (transform.position.y < limitTop)
                    {
                        transform.position += new Vector3(0, distanceY, 0);
                    }
                }
                firstPos = currentPos;
            }
        }
    }

    private void PlayerPrepare()
    {
        //Fire event
        PlayerState = PlayerState.Prepare;
        playerState = PlayerState.Prepare;

        //Cache components
        spRender = GetComponent<SpriteRenderer>();
        polyCollider = GetComponent<PolygonCollider2D>();
        centerGunRender = centerGun.GetComponent<SpriteRenderer>();

        //Replce sprite renderer and polygon collider with selected character
        GameObject character = CharacterManager.Instance.characters[CharacterManager.Instance.SelectedIndex];
        SpriteRenderer charRender = character.GetComponent<SpriteRenderer>();
        PolygonCollider2D charCollider = character.GetComponent<PolygonCollider2D>();
        spRender.sprite = charRender.sprite;
        polyCollider.points = charCollider.points;

        spRender.enabled = false;
        centerGun.SetActive(false);
        leftGun.SetActive(false);
        rightGun.SetActive(false);

        //Define the limit left and right
        float horizontalBorder = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;
        limitLeft = horizontalBorder + spRender.bounds.size.x / 2;
        limitRight = Mathf.Abs(horizontalBorder) - spRender.bounds.size.x / 2;

    }

    private void PlayerLiving()
    {
        //Fire event
        PlayerState = PlayerState.Living;
        playerState = PlayerState.Living;

        //Add another actions here
        gunCount = 1;
        spRender.enabled = true;
        polyCollider.enabled = true;
        centerGun.SetActive(true);
        leftGun.SetActive(true);
        rightGun.SetActive(true);

        StartCoroutine(FireBullets());
    }

    private void PlayerDie()
    {
        //Fire event
        PlayerState = PlayerState.Die;
        playerState = PlayerState.Die;

        //Add another actions here

        spRender.enabled = false;
        polyCollider.enabled = false;
        centerGun.SetActive(false);
        leftGun.SetActive(false);
        rightGun.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            ItemController itemControl = collision.GetComponent<ItemController>();
            if (itemControl.itemType != ItemType.COIN)
            {
                GameManager.Instance.PlayBallExplode(collision.transform.position);
                //if (itemControl.itemType == ItemType.HIDDEN_GUNS)
                //{
                //    if (gunCount == 1)
                //        StartCoroutine(SetGuncount());
                //}
            }
        }
        else if (collision.CompareTag("Ball"))
        {
            HP--;
            if (HP < 1)
            {
                GameManager.Instance.GameOver();
            }
            Debug.Log(HP);
        }
    }


    IEnumerator SetGuncount()
    {
        float value = Random.value;
        if (value >= 0 && value <= 0.2) //Triple guns
        {
            gunCount = 3;
            yield return new WaitForSeconds(GameManager.Instance.tripleGunsTime);
        }
        else //Double guns
        {
            gunCount = 2;
            yield return new WaitForSeconds(GameManager.Instance.doubleGunsTime);
        }
        gunCount = 1;
    }


    IEnumerator FireBullets()
    {
        while (playerState == PlayerState.Living)
        {
            if (gunCount == 1) //Fire center gun
            {
                //Hide left and right gun
                centerGun.SetActive(true);
                leftGun.SetActive(false);
                rightGun.SetActive(false);

                //Create position, moving direction for center bullet
                Vector2 centerDir = centerGun.transform.TransformDirection(Vector2.up);
                Vector2 centerBulletPoint = (Vector2)centerGun.transform.position
                    + centerDir * (centerGunRender.bounds.size.y / 2);

                //Enable center bullet and move
                GameManager.Instance.FireBullet(centerBulletPoint, centerDir);
            }
            else if (gunCount == 2) //Fire left and right guns
            {
                //Hide center gun
                centerGun.SetActive(false);
                leftGun.SetActive(true);
                rightGun.SetActive(true);

                //Create position, moving direction for left bullet
                Vector2 leftDir = leftGun.transform.TransformDirection(Vector2.up);
                Vector2 leftBulletPoint = (Vector2)leftGun.transform.position
                    + leftDir * (centerGunRender.bounds.size.y / 2);
                //Enable left bullet and move
                GameManager.Instance.FireBullet(leftBulletPoint, leftDir);



                //Create position, moving direction for right bullet
                Vector2 rightDir = rightGun.transform.TransformDirection(Vector2.up);
                Vector2 rightBulletPoint = (Vector2)rightGun.transform.position
                    + rightDir * (centerGunRender.bounds.size.y / 2);

                //Enable right bullet and move
                GameManager.Instance.FireBullet(rightBulletPoint, rightDir);

            }
            else //Fire three guns
            {
                //Enable three guns
                centerGun.SetActive(true);
                leftGun.SetActive(true);
                rightGun.SetActive(true);


                //Create position, moving direction for center bullet
                Vector2 centerDir = centerGun.transform.TransformDirection(Vector2.up);
                Vector2 centerBulletPoint = (Vector2)centerGun.transform.position
                    + centerDir * (centerGunRender.bounds.size.y / 2);
                //Enable center bullet and move
                GameManager.Instance.FireBullet(centerBulletPoint, centerDir);


                //Create position, moving direction for left bullet
                Vector2 leftDir = leftGun.transform.TransformDirection(Vector2.up);
                Vector2 leftBulletPoint = (Vector2)leftGun.transform.position
                    + leftDir * (centerGunRender.bounds.size.y / 2);
                //Enable left bullet and move
                GameManager.Instance.FireBullet(leftBulletPoint, leftDir);



                //Create position, moving direction for right bullet
                Vector2 rightDir = rightGun.transform.TransformDirection(Vector2.up);
                Vector2 rightBulletPoint = (Vector2)rightGun.transform.position
                    + rightDir * (centerGunRender.bounds.size.y / 2);

                //Enable right bullet and move
                GameManager.Instance.FireBullet(rightBulletPoint, rightDir);

            }
            yield return new WaitForSeconds(DataController.GetShootingWaitTime());
        }
    }
}
