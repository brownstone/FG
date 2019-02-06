using System;
using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour {

    public static CoinManager Instance;

    public int Coins
    {
        get { return currentCoins; }
        private set { currentCoins = value; }
    }

    public static event Action<int> CoinsUpdated = delegate { };

    [SerializeField]
    int initialCoins = 10;

    // Show the current coins value in editor for easy testing
    [SerializeField]
    int currentCoins;

    // key name to store high score in PlayerPrefs
    //const string PPK_COINS = "ONEFALL_COINS";


    void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        // Initialize coins
        //Coins = PlayerPrefs.GetInt(PPK_COINS, initialCoins);
        
        Coins = initialCoins;
    }

    public void AddCoins(int amount)
    {
        Coins += amount;


        // Store new coin value
        //PlayerPrefs.SetInt(PPK_COINS, Coins);

        // Fire event
        CoinsUpdated(Coins);
    }

    public void RemoveCoins(int amount)
    {
        Coins -= amount;

        // Store new coin value
        //PlayerPrefs.SetInt(PPK_COINS, Coins);

        // Fire event
        CoinsUpdated(Coins);
    }
}
