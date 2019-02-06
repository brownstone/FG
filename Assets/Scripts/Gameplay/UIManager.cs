using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance { private set; get; }

    //Gameplay UI
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private Text scoreTxt;
    [SerializeField] private Text coinsTxt;

    //[SerializeField] private Text missileTxt;
    //[SerializeField] private Text bombTxt;
    //[SerializeField] private Text laserTxt;

    ////Revive UI
    [SerializeField] private GameObject reviveUI;
    //[SerializeField] private Image reviveCoverImg;

    ////Game over UI
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Text bestScoreTxt;
    [SerializeField] private GameObject gameName;
    //[SerializeField] private RawImage shareImage;
    //[SerializeField] private Button dailyRewardBtn;
    //[SerializeField] private Text dailyRewardTxt;
    //[SerializeField] private GameObject watchAdForCoinsBtn;
    [SerializeField] private GameObject playBtn;
    [SerializeField] private GameObject restartBtn;
    //[SerializeField] private GameObject shareBtn;
    //[SerializeField] private GameObject soundOnBtn;
    //[SerializeField] private GameObject soundOffBtn;
    //[SerializeField] private Animator servicesUIAnim;
    //[SerializeField] private Animator settingUIAnim;
    [SerializeField] private AnimationClip servicesUI_Show;
    [SerializeField] private AnimationClip servicesUI_Hide;
    [SerializeField] private AnimationClip settingUI_Show;
    [SerializeField] private AnimationClip settingUi_Hide;
    //[SerializeField] private RewardedCoinsController rewardCoinsControl;
    [SerializeField] private RectTransform coinImgTrans;


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
        if (obj == GameState.Revive)
        {
            reviveUI.SetActive(true);
            StartCoroutine(ReviveCountDown());
        }
        else if (obj == GameState.GameOver)
        {
            Invoke("ShowGameOverUI", 0.5f);
        }
        else if (obj == GameState.Playing)
        {
            gameplayUI.SetActive(true);
            gameOverUI.SetActive(false);
            //reviveUI.SetActive(false);
            //rewardCoinsControl.gameObject.SetActive(false);
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




    //// Use this for initialization
    //void Start()
    //{
    //    if (!GameManager.isRestart) //This is the first load
    //    {
    //        gameplayUI.SetActive(false);
    //        reviveUI.SetActive(false);
    //        rewardCoinsControl.gameObject.SetActive(false);

    //        shareImage.gameObject.SetActive(false);
    //        bestScoreTxt.gameObject.SetActive(false);
    //        restartBtn.SetActive(false);
    //        watchAdForCoinsBtn.SetActive(false);
    //        dailyRewardBtn.gameObject.SetActive(false);
    //        shareBtn.SetActive(false);
    //    }
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //    UpdateMuteButtons();

    //    scoreTxt.text = ScoreManager.Instance.Score.ToString();
    //    bestScoreTxt.text = "Best: " + ScoreManager.Instance.BestScore.ToString();
    //    coinsTxt.text = CoinManager.Instance.Coins.ToString();

    //    missileTxt.text = ItemsManager.Instance.Missiles.ToString();
    //    bombTxt.text = ItemsManager.Instance.Bombs.ToString();
    //    laserTxt.text = ItemsManager.Instance.Lasers.ToString();

    //    if (DailyRewardManager.Instance.CanRewardNow())
    //    {
    //        dailyRewardTxt.text = "Grap";
    //        dailyRewardBtn.interactable = true;
    //    }
    //    else
    //    {
    //        string hours = DailyRewardManager.Instance.TimeUntilNextReward.Hours.ToString();
    //        string minutes = DailyRewardManager.Instance.TimeUntilNextReward.Minutes.ToString();
    //        string seconds = DailyRewardManager.Instance.TimeUntilNextReward.Seconds.ToString();
    //        dailyRewardTxt.text = hours + ":" + minutes + ":" + seconds;
    //        dailyRewardBtn.interactable = false;
    //    }

    //}


    //////////////////////////////Publish functions

    public void PlayButtonSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.button);
    }

    //public void MissileBtn()
    //{
    //    GameManager.Instance.CreateMissile(PlayerController.Instance.transform.position);
    //}

    //public void BombBtn()
    //{
    //    GameManager.Instance.CreateBomb(PlayerController.Instance.transform.position);
    //}

    //public void LaserBtn()
    //{
    //    GameManager.Instance.CreateLaser(PlayerController.Instance.transform.position);
    //}

    //public void ToggleSound()
    //{
    //    SoundManager.Instance.ToggleMute();
    //}

    //public void NativeShareBtn()
    //{
    //    ShareManager.Instance.NativeShare();
    //}

    //public void RateAppBtn()
    //{
    //    Application.OpenURL(ShareManager.Instance.AppUrl);
    //}

    //public void FacebookShareBtn()
    //{
    //    ShareManager.Instance.FacebookShare();
    //}

    public void PlayBtn()
    {
        playBtn.SetActive(false);
        gameOverUI.SetActive(false);

        gameplayUI.SetActive(true);

        GameManager.Instance.PlayingGame();
    }

    public void RestartBtn(float delay)
    {
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name, delay);
    }

    //public void SettingBtn()
    //{
    //    settingUIAnim.Play(settingUI_Show.name);
    //    servicesUIAnim.Play(servicesUI_Hide.name);
    //}

    //public void BackBtn()
    //{
    //    settingUIAnim.Play(settingUi_Hide.name);
    //    servicesUIAnim.Play(servicesUI_Show.name);
    //}

    //public void DailyRewardBtn()
    //{
    //    DailyRewardManager.Instance.ResetNextRewardTime();
    //    StartReward(0.5f, DailyRewardManager.Instance.GetRandomReward());
    //}


    //public void WatchAdForCoinsBtn()
    //{
    //    watchAdForCoinsBtn.SetActive(false);
    //    AdManager.Instance.ShowRewardedVideoAd();
    //}

    //public void ReviveBtn()
    //{
    //    reviveUI.SetActive(false);
    //    AdManager.Instance.ShowRewardedVideoAd();
    //}

    //public void SkipBtn()
    //{
    //    GameManager.Instance.GameOver();
    //}

    //public void UpgradeBtn()
    //{
    //    GameManager.Instance.LoadScene("Upgrade", 0.5f);
    //}

    //public void CharacterBtn()
    //{
    //    GameManager.Instance.LoadScene("Character", 0.5f);
    //}


    ///////////////////////////////Private functions
    //void UpdateMuteButtons()
    //{
    //    if (SoundManager.Instance.IsMuted())
    //    {
    //        soundOnBtn.gameObject.SetActive(false);
    //        soundOffBtn.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        soundOnBtn.gameObject.SetActive(true);
    //        soundOffBtn.gameObject.SetActive(false);
    //    }
    //}

    void ShowGameOverUI()
    {
        gameplayUI.SetActive(false);
        //reviveUI.SetActive(false);
        //rewardCoinsControl.gameObject.SetActive(false);
        gameOverUI.SetActive(true);

        bestScoreTxt.gameObject.SetActive(true);
        gameName.SetActive(false);
        //shareImage.gameObject.SetActive(true);
        //shareImage.texture = GameManager.Instance.LoadedScrenshot();

        playBtn.SetActive(false);
        //restartBtn.SetActive(true);

        //dailyRewardBtn.gameObject.SetActive(true);
        //watchAdForCoinsBtn.SetActive(AdManager.Instance.IsRewardedVideoAdReady());

        //shareBtn.SetActive(true);
    }

    IEnumerator ReviveCountDown()
    {
        float t = 0;
        while (t < GameManager.Instance.reviveCountDownTime)
        {
            if (!reviveUI.activeInHierarchy)
                yield break;
            t += Time.deltaTime;
            float factor = t / GameManager.Instance.reviveCountDownTime;
            //reviveCoverImg.fillAmount = Mathf.Lerp(1, 0, factor);
            yield return null;
        }
        reviveUI.SetActive(false);
        GameManager.Instance.GameOver();
    }




    //public void StartReward(float delay, int coins)
    //{
    //    StartCoroutine(RewardingCoins(delay, coins));
    //}
    //IEnumerator RewardingCoins(float delay, int coins)
    //{
    //    yield return new WaitForSeconds(delay);
    //    rewardCoinsControl.gameObject.SetActive(true);
    //    rewardCoinsControl.StartReward(coins);
    //}


    public Vector2 GetCoinImgWorldPos()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(coinImgTrans.position);
        return worldPos;
    }
}
