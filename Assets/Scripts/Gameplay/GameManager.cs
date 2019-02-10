using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;


public enum GameState
{
    Prepare,
    Playing,
    Revive,
    GameOver,
}


public enum ItemType
{
    COIN,
    HIDDEN_GUNS,
    MISSILE,
    BOMB,
    LASER,
    ASTRONAUT,
}

public enum BoostUpType
{
    MISSILE,
    BOMB,
    LASER,
}

public enum EnemyType
{
    PLANET,
    METEOR,
    SATELLITE,
}

[System.Serializable]
public struct BallNumberConfig
{
    public float MinScale;
    public float MaxScale;
    public int MinNumber;
    public int MaxNumber;
}

public class GameManager : MonoBehaviour {

    public static GameManager Instance { private set; get; }
    public static event System.Action<GameState> GameStateChanged = delegate { };
    public static bool isRestart = false;

    public GameState GameState
    {
        get
        {
            return gameState;
        }
        private set
        {
            if (value != gameState)
            {
                gameState = value;
                GameStateChanged(gameState);
            }
        }
    }


    [Header("Gameplay Config")]
    public float minBallScale = 0.3f;
    [SerializeField] private float originalBallFallingSpeed = 1.5f;
    [SerializeField] private float maxBallFallingSpeed = 15f;
    [SerializeField] private float ballFallingSpeedIncreaseFactor = 1f;
    [SerializeField] private int scoreToIncreaseBallFallingSpeed = 500;
    public float ballExplodeScaleUpFactor = 3;
    public float ballExplodeScaleUpTime = 0.5f;
    public float missileMovingSpeed = 30f;
    public float bombMovingSpeed = 5f;
    public float laserMovingSpeed = 25;
    public float doubleGunsTime = 10f;
    public float tripleGunsTime = 5f;
    public float reviveCountDownTime = 4f;
    [SerializeField] [Range(0f, 1f)] private float coinFequency = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float hiddenGunsFrequency = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float boostUpFrequency;
    public BallNumberConfig[] ballNumberConfig;
    [SerializeField] private Color[] ballColors;


    private int _stageIndex = 0;
    [SerializeField] private GameObject[] _stages;


    [Header("Gameplay References")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletExplodePrefab;
    [SerializeField] private GameObject ballExplodeParticlePrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject missileItemPrefab;
    [SerializeField] private GameObject bombItemPrefab;
    [SerializeField] private GameObject laserItemPrefab;
    [SerializeField] private GameObject hiddenGunsItemPrefab;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject laserPrefab;


    public bool IsRevive { private set; get; }


    private GameState gameState = GameState.GameOver;
    private List<BallController> listBallControl = new List<BallController>();
    private List<BulletController> listBulletControl = new List<BulletController>();
    private List<ParticleSystem> listBulletExplode = new List<ParticleSystem>();
    private List<ParticleSystem> listBallExplodeParticle = new List<ParticleSystem>();
    private List<ItemController> listCoin = new List<ItemController>();
    private List<ItemController> listHiddenGunsItem = new List<ItemController>();
    private List<ItemController> listBombItem = new List<ItemController>();
    private List<ItemController> listLaserItem = new List<ItemController>();
    private List<ItemController> listMissileItem = new List<ItemController>();

    private BoostUpController missileControl = null;
    private BoostUpController bombControl = null;
    private BoostUpController laserControl = null;
    private float leftPoint;
    private float rightPoint;
    private float _yPos;
    private float currentBallFallingSpeed = 0;
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

    // Use this for initialization
    void Start () {

        Application.targetFrameRate = 60;
        ScoreManager.Instance.Reset();

        PrepareGame();
    }

    public void PrepareGame()
    {
        //Fire event
        GameState = GameState.Prepare;
        gameState = GameState.Prepare;

        //Add another actions here

        //Set IsRevived equal to false
        IsRevive = false;

        currentBallFallingSpeed = originalBallFallingSpeed;

        float borderLeft = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).x;
        float borderRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).x;
        float radius = ballPrefab.GetComponent<CircleCollider2D>().radius;
        leftPoint = borderLeft + radius;
        rightPoint = borderRight - radius;
        _yPos = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y + radius + 0.5f;


        //Clone a missile
        missileControl = Instantiate(missilePrefab, Vector2.zero, Quaternion.identity).GetComponent<BoostUpController>();
        missileControl.gameObject.SetActive(false);

        //Clone a bomb
        bombControl = Instantiate(bombPrefab, Vector2.zero, Quaternion.identity).GetComponent<BoostUpController>();
        bombControl.gameObject.SetActive(false);

        //Clone a laser
        laserControl = Instantiate(laserPrefab, Vector2.zero, Quaternion.identity).GetComponent<BoostUpController>();
        laserControl.gameObject.SetActive(false);

        if (isRestart)
            PlayingGame();
    }


    /// <summary>
    /// Actual start the game
    /// </summary>
    public void PlayingGame()
    {
        //Fire event
        GameState = GameState.Playing;
        gameState = GameState.Playing;

        //Add another actions here

        //StartCoroutine(CreateBalls());
        //StartCoroutine(CameraScroll());
        StartCoroutine(StageScroll());
        //StartCoroutine(IncreaseBallFallingSpeed());
    }

    /// <summary>
    /// Call Revive event
    /// </summary>
    public void Revive()
    {
        //Fire event
        GameState = GameState.Revive;
        gameState = GameState.Revive;

        //Add another actions here
        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
    }

    /// <summary>
    /// Call GameOver event
    /// </summary>
    public void GameOver()
    {
        //Fire event
        GameState = GameState.GameOver;
        gameState = GameState.GameOver;

        //Add another actions here
        isRestart = true;
        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
    }


    public void LoadScene(string sceneName, float delay)
    {
        Debug.Log(sceneName);
        StartCoroutine(LoadingScene(sceneName, delay));
    }

    private IEnumerator LoadingScene(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    //Get the inactive ball
    private BallController GetBallControl()
    {
        //Find the inactive ball
        foreach (BallController o in listBallControl)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }


        //Didn't find one -> create new ball
        BallController ballControl = Instantiate(ballPrefab, Vector2.zero, Quaternion.identity).GetComponent<BallController>();
        listBallControl.Add(ballControl);
        ballControl.gameObject.SetActive(false);
        return ballControl;
    }

    //Get the inactive bullet
    private BulletController GetBulletControl()
    {
        //Find the inactive bullet
        foreach (BulletController o in listBulletControl)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one (run out of bullet) -> create new one
        BulletController bulletControl = Instantiate(bulletPrefab, Vector2.zero, Quaternion.identity).GetComponent<BulletController>();
        listBulletControl.Add(bulletControl);
        bulletControl.gameObject.SetActive(false);
        return bulletControl;
    }

    //Get the bullet explore particle object
    private ParticleSystem GetBulletExplode()
    {
        //Find the inactive particle
        foreach (ParticleSystem o in listBulletExplode)
        {
            if (!o.gameObject.activeInHierarchy)
            {
                return o;
            }
        }

        //Didn't find one (run out of particle) -> create new one
        ParticleSystem explore = Instantiate(bulletExplodePrefab, Vector2.zero, Quaternion.identity).GetComponent<ParticleSystem>();
        listBulletExplode.Add(explore);
        explore.gameObject.SetActive(false);
        return explore;
    }

    private ParticleSystem GetBallExplodeParticle()
    {
        foreach (ParticleSystem o in listBallExplodeParticle)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        ParticleSystem ballExplode = Instantiate(ballExplodeParticlePrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
        listBallExplodeParticle.Add(ballExplode);
        ballExplode.gameObject.SetActive(false);
        return ballExplode;
    }



    //Get coin item
    private ItemController GetCoinItem()
    {
        //Find an inactive missile item
        foreach (ItemController o in listCoin)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one -> create new coin item
        ItemController coin = Instantiate(coinPrefab, Vector2.zero, Quaternion.identity).GetComponent<ItemController>();
        listCoin.Add(coin);
        coin.gameObject.SetActive(false);
        return coin;
    }


    //Get hidden guns item
    private ItemController GetHiddenGunsItem()
    {
        //Find an inactive hidden guns item
        foreach (ItemController o in listHiddenGunsItem)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one -> create new hidden guns item
        ItemController hiddenGuns = Instantiate(hiddenGunsItemPrefab, Vector2.zero, Quaternion.identity).GetComponent<ItemController>();
        listHiddenGunsItem.Add(hiddenGuns);
        hiddenGuns.gameObject.SetActive(false);
        return hiddenGuns;
    }

    //Get missile item
    private ItemController GetMissileItem()
    {
        //Find an inactive missile item
        foreach (ItemController o in listMissileItem)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one -> create new missile item
        ItemController missile = Instantiate(missileItemPrefab, Vector2.zero, Quaternion.identity).GetComponent<ItemController>();
        listMissileItem.Add(missile);
        missile.gameObject.SetActive(false);
        return missile;
    }

    //Get bomb item
    private ItemController GetBombItem()
    {
        //Find an inactive missile item
        foreach (ItemController o in listBombItem)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one -> create new bomb item
        ItemController bomb = Instantiate(bombItemPrefab, Vector2.zero, Quaternion.identity).GetComponent<ItemController>();
        listBombItem.Add(bomb);
        bomb.gameObject.SetActive(false);
        return bomb;
    }

    //Get missile item
    private ItemController GetLaserItem()
    {
        //Find an inactive laser item
        foreach (ItemController o in listLaserItem)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one -> create new laser item
        ItemController laser = Instantiate(laserItemPrefab, Vector2.zero, Quaternion.identity).GetComponent<ItemController>();
        listLaserItem.Add(laser);
        laser.gameObject.SetActive(false);
        return laser;
    }

    private IEnumerator StageScroll()
    {
        while (gameState == GameState.Playing)
        {
            GameObject go = _stages[_stageIndex];


            Vector2 pos = go.transform.position;
            pos -= Vector2.up * 1.1f * Time.deltaTime;
            Vector3 newPos = pos;
            newPos.z = 0.0f;
            go.transform.position = newPos;
            yield return null;
        }

        yield return null;
    }
    private IEnumerator CameraScroll()
    {
        while (gameState == GameState.Playing)
        {
            Vector2 pos = Camera.main.transform.position;
            pos += Vector2.up * 1.1f * Time.deltaTime;
            Vector3 newPos = pos;
            newPos.z = -10.0f;
            Camera.main.transform.position = newPos;
            yield return null;
        }

        yield return null;
    }

    private IEnumerator CreateBalls()
    {
        while (gameState == GameState.Playing)
        {
            //Create position 
            Vector2 pos = new Vector2(Random.Range(leftPoint, rightPoint), _yPos);

            if (Random.value <= coinFequency) //Create coin
            {
                //Cache components
                ItemController coinControl = GetCoinItem();
                CircleCollider2D itemCollider = coinControl.GetComponent<CircleCollider2D>();

                //Set position and falling speed
                coinControl.transform.position = pos;
                coinControl.fallingSpeed = currentBallFallingSpeed;

                //enable object and disable collider
                coinControl.gameObject.SetActive(true);
                itemCollider.enabled = false;

                while (Physics2D.OverlapCircle(coinControl.transform.position, itemCollider.radius, 1 << LayerMask.NameToLayer("Overlap")) != null)
                {
                    yield return null;

                    //Renew position
                    pos = new Vector2(Random.Range(leftPoint, rightPoint), _yPos);
                    //Set position
                    coinControl.transform.position = pos;
                }

                //Enable collider
                itemCollider.enabled = true;

                //Move the coin
                coinControl.MoveDown();
            }
            else //Create ball
            {
                //Scale for the ball
                float scale = Random.Range(minBallScale, 1f);

                //Cache components
                BallController ballControl = GetBallControl();
                CircleCollider2D ballCollider = ballControl.GetComponent<CircleCollider2D>();
                SpriteRenderer ballRender = ballControl.GetComponent<SpriteRenderer>();

                //Set position and scale for the ball
                ballControl.transform.position = pos;
                ballControl.transform.localScale = new Vector2(scale, scale);

                //Set falling speed, color and number for the ball
                ballControl.fallingSpeed = currentBallFallingSpeed;
                ballRender.color = ballColors[Random.Range(0, ballColors.Length)];
                foreach (BallNumberConfig o in ballNumberConfig)
                {
                    if (scale >= o.MinScale && scale < o.MaxScale)
                    {
                        ballControl.number = Random.Range(o.MinNumber, o.MaxNumber);
                    }
                }

                ballControl.gameObject.SetActive(true);
                ballCollider.enabled = false;

                //The collider is overlapping another     
                while (Physics2D.OverlapCircle(ballControl.transform.position, ballCollider.radius, 1 << LayerMask.NameToLayer("Overlap")) != null)
                {
                    yield return null;

                    //Renew position, scale
                    pos = new Vector2(Random.Range(leftPoint, rightPoint), _yPos);
                    scale = Random.Range(minBallScale, 1f);

                    //Reset position, scale of the ball
                    ballControl.transform.position = pos;
                    ballControl.transform.localScale = new Vector2(scale, scale);

                    //Reset color and number of the ball
                    ballRender.color = ballColors[Random.Range(0, ballColors.Length)];
                    foreach (BallNumberConfig o in ballNumberConfig)
                    {
                        if (scale >= o.MinScale && scale < o.MaxScale)
                        {
                            ballControl.number = Random.Range(o.MinNumber, o.MaxNumber);
                        }
                    }

                }

                //No overlapping aymore -> enable collider
                ballCollider.enabled = true;

                //Move the ball down
                ballControl.MoveBall();
            }
            yield return null;
        }
    }

    private IEnumerator IncreaseBallFallingSpeed()
    {
        while (gameState == GameState.Playing)
        {
            if (currentBallFallingSpeed < maxBallFallingSpeed)
            {
                if (ScoreManager.Instance.Score != 0 && ScoreManager.Instance.Score % scoreToIncreaseBallFallingSpeed == 0)
                {
                    currentBallFallingSpeed += ballFallingSpeedIncreaseFactor;

                    foreach (BallController o in listBallControl)
                    {
                        if (o.gameObject.activeInHierarchy)
                            o.fallingSpeed = currentBallFallingSpeed;
                    }

                    ItemController[] items = FindObjectsOfType<ItemController>();
                    foreach (ItemController o in items)
                    {
                        o.fallingSpeed = currentBallFallingSpeed;
                    }
                }
            }
            else
            {
                currentBallFallingSpeed = maxBallFallingSpeed;
                yield break;
            }
            yield return null;
        }
    }


    private IEnumerator PlayBulletExploreParticle(Vector2 position, Vector2 lookDir)
    {
        ParticleSystem par = GetBulletExplode();

        par.gameObject.transform.position = position;
        par.transform.up = -lookDir;
        par.gameObject.SetActive(true);

        par.Play();
        var main = par.main;
        yield return new WaitForSeconds(main.startLifetimeMultiplier);
        par.gameObject.SetActive(false);
    }

    private IEnumerator PlayBallExplodeParticle(ParticleSystem par)
    {
        par.Play();
        yield return new WaitForSeconds(par.main.startLifetimeMultiplier);
        par.gameObject.SetActive(false);
    }

    ////////////////////////////////////////Publish functions


    /// <summary>
    /// Fire the bullet with given position and direction
    /// </summary>
    /// <param name="position"></param>
    /// <param name="movingDirection"></param>
    public void FireBullet(Vector2 position, Vector2 movingDirection)
    {
        BulletController bulletControl = GetBulletControl();
        bulletControl.transform.position = position;
        bulletControl.movingDirection = movingDirection;
        bulletControl.movingSpeed = DataController.GetBulletSpeed();

        bulletControl.gameObject.SetActive(true);
        bulletControl.MoveBullet();
    }


    /// <summary>
    /// Create boost up (added bullet, bomb, laser)
    /// </summary>
    /// <param name="pos"></param>
    public void CreateBoostUp(Vector2 pos)
    {
        ItemController itemControl = null;
        if (Random.value <= hiddenGunsFrequency) //Create hidden guns
        {
            itemControl = GetHiddenGunsItem();
            itemControl.transform.position = pos;
            itemControl.fallingSpeed = currentBallFallingSpeed;
            itemControl.gameObject.SetActive(true);
            itemControl.MoveDown();
        }
        else if (Random.value <= boostUpFrequency) //Create boost up
        {
            float value = Random.value;
            if (value >= 0 && value < 0.5f) //Create missile item
            {
                itemControl = GetMissileItem();
            }
            else if (value >= 0.5f && value < 0.8f) //Create bomb item
            {
                itemControl = GetBombItem();
            }
            else //Create laser item
            {
                itemControl = GetLaserItem();
            }
            itemControl.transform.position = pos;
            itemControl.fallingSpeed = currentBallFallingSpeed;
            itemControl.gameObject.SetActive(true);
            itemControl.MoveDown();
        }
    }



    /// <summary>
    /// Create missile with given position
    /// </summary>
    /// <param name="pos"></param>
    public void CreateMissile(Vector2 pos)
    {
        if (!bombControl.gameObject.activeInHierarchy)
        {
            if (ItemsManager.Instance.Missiles > 0)
            {
                ItemsManager.Instance.AddMissile(-1);
                missileControl.transform.position = pos;
                missileControl.gameObject.SetActive(true);
                missileControl.MoveUp();
            }
        }
    }

    /// <summary>
    /// Create bomb with given position
    /// </summary>
    /// <param name="pos"></param>
    public void CreateBomb(Vector2 pos)
    {
        if (!bombControl.gameObject.activeInHierarchy)
        {
            if (ItemsManager.Instance.Bombs > 0)
            {
                ItemsManager.Instance.AddBomb(-1);
                bombControl.transform.position = pos;
                bombControl.gameObject.SetActive(true);
                bombControl.MoveUp();
            }
        }
    }

    /// <summary>
    /// Create laser with given position
    /// </summary>
    /// <param name="pos"></param>
    public void CreateLaser(Vector2 pos)
    {
        if (!laserControl.gameObject.activeInHierarchy)
        {
            if (ItemsManager.Instance.Lasers > 0)
            {
                ItemsManager.Instance.AddLaser(-1);
                laserControl.transform.position = pos;
                laserControl.gameObject.SetActive(true);
                laserControl.MoveUp();
            }
        }
    }


    /// <summary>
    /// Play bullet explore particle with given position and  z angle
    /// </summary>
    /// <param name="position"></param>
    /// <param name="zAngle"></param>
    public void PlayBulletExplore(Vector2 position, Vector2 lookdir)
    {
        StartCoroutine(PlayBulletExploreParticle(position, lookdir));
    }


    /// <summary>
    /// Play ball explore with given position
    /// </summary>
    /// <param name="position"></param>
    public void PlayBallExplode(Vector2 position)
    {
        ParticleSystem ballExplode = GetBallExplodeParticle();
        ballExplode.gameObject.SetActive(true);
        ballExplode.transform.position = position;
        StartCoroutine(PlayBallExplodeParticle(ballExplode));
    }



    ///// <summary>
    ///// Load the saved screenshot
    ///// </summary>
    ///// <returns></returns>
    //public Texture LoadedScrenshot()
    //{
    //    byte[] bytes = File.ReadAllBytes(ShareManager.Instance.ScreenshotPath);
    //    Texture2D tx = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
    //    tx.LoadImage(bytes);
    //    return tx;
    //}

    /// <summary>
    /// Continue the game
    /// </summary>
    public void SetContinueGame()
    {
        IsRevive = true;
        PlayingGame();
    }
}
