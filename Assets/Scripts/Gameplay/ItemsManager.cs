using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{

    public static ItemsManager Instance { private set; get; }

    private const string PPK_BOMB = "BOMB";
    private const string PPK_LASER = "LASER";
    private const string PPK_MISSILE = "MISSILE";


    public int Missiles
    {
        private set { }
        get { return PlayerPrefs.GetInt(PPK_MISSILE); }
    }

    public int Bombs
    {
        private set { }
        get { return PlayerPrefs.GetInt(PPK_BOMB); }
    }

    public int Lasers
    {
        private set { }
        get { return PlayerPrefs.GetInt(PPK_LASER); }
    }


    [SerializeField]
    private int initialMissiles = 1;
    [SerializeField]
    private int initialBombs = 1;
    [SerializeField]
    private int initialLasers = 1;


    private void Awake()
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

    private void Start()
    {
        if (!PlayerPrefs.HasKey(PPK_MISSILE))
            PlayerPrefs.SetInt(PPK_MISSILE, initialMissiles);
        if (!PlayerPrefs.HasKey(PPK_BOMB))
            PlayerPrefs.SetInt(PPK_BOMB, initialBombs);
        if (!PlayerPrefs.HasKey(PPK_LASER))
            PlayerPrefs.SetInt(PPK_LASER, initialLasers);
    }

    public void AddMissile(int amount)
    {
        int value = Missiles + amount;
        PlayerPrefs.SetInt(PPK_MISSILE, value);
    }

    public void AddBomb(int amount)
    {
        int value = Bombs + amount;
        PlayerPrefs.SetInt(PPK_BOMB, value);
    }

    public void AddLaser(int amount)
    {
        int value = Lasers + amount;
        PlayerPrefs.SetInt(PPK_LASER, value);
    }

    public void AddAstronaut(int amount)
    {
        Debug.Log("Astronaut");
    }

}
