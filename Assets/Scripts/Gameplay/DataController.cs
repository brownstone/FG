using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController {

    private const string SS_Level_PPK = "SS_LEVEL_KEY";
    private const string BP_Level_PPK = "BP_LEVEL_KEY";
    public const float Max_SS_Level = 50;
    public const float Max_BP_Level = 50;

    private static float originalShootingWaitTime = 0.12f;
    private static float minShootingWaitTime = 0.03f;

    private static float originalBulletSpeed = 40f;
    private static float maxBulletSpeed = 80f;

    /// <summary>
    /// Get current shooting speed level
    /// </summary>
    /// <returns></returns>
    public static int Get_SS_Level()
    {
        if (!PlayerPrefs.HasKey(SS_Level_PPK))
        {
            PlayerPrefs.SetInt(SS_Level_PPK, 1);
            return 1;
        }
        else
            return PlayerPrefs.GetInt(SS_Level_PPK);
    }


    /// <summary>
    /// Get current bullet speed level
    /// </summary>
    /// <returns></returns>
    public static int Get_BP_Level()
    {
        if (!PlayerPrefs.HasKey(BP_Level_PPK))
        {
            PlayerPrefs.GetInt(BP_Level_PPK, 1);
            return 1;
        }
        else
            return PlayerPrefs.GetInt(BP_Level_PPK);
    }


    /// <summary>
    /// Get current shooting wait time
    /// </summary>
    /// <returns></returns>
    public static float GetShootingWaitTime()
    {
        int currentLevel = Get_SS_Level();
        float decreaseValue = (originalShootingWaitTime - minShootingWaitTime) / Max_SS_Level;
        float result = originalShootingWaitTime;
        for(int i = 0; i < currentLevel; i++)
        {
            result -= decreaseValue;
        }
        return result;
    }


    /// <summary>
    /// Get current bullet speed
    /// </summary>
    /// <returns></returns>
    public static float GetBulletSpeed()
    {
        int currentLevel = Get_BP_Level();
        float increaseValue = (maxBulletSpeed - originalBulletSpeed) / Max_BP_Level;
        float result = originalBulletSpeed;
        for(int i = 0; i < currentLevel; i++)
        {
            result += increaseValue;
        }
        return result;
    }

    /// <summary>
    /// Increase shooting speed level 
    /// </summary>
    /// <param name="increaseAmount"></param>
    /// <returns></returns>
    public static void Increase_SS_Level(int increaseAmount)
    {
        PlayerPrefs.SetInt(SS_Level_PPK, Get_SS_Level() + increaseAmount);
    }

    /// <summary>
    /// Increase bullet speed level
    /// </summary>
    /// <param name="increaseAmount"></param>
    public static void Increase_BP_Level(int increaseAmount)
    {
        PlayerPrefs.SetInt(BP_Level_PPK, Get_BP_Level() + increaseAmount);
    }

}
