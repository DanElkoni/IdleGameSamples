// Daniel Elkoni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is responsible for saving and loading data, most data was stored locally as the API was not completed, but this script does still log the user into the api and get/upload the data that was available to be stored/got from the api at the time

public class SaveManager : MonoBehaviour
{
    // Defining scripts
    public LogicManager lm;
    public UniManager unim;
    public UpgradeManager upm;
    public APIHandler su;
    public APIHandler api;

    // Defining simple classes to clean up saving especially for lists
    public void SaveI(int i, string name)
    {
        PlayerPrefs.SetInt(name, i);
    }

    public void LoadI(int i, string name)
    {
        i = PlayerPrefs.GetInt(name);
    }

    public void SaveF(float f, string name)
    {
        PlayerPrefs.SetFloat(name, f);
    }

    public void LoadF(float f, string name)
    {
        f += PlayerPrefs.GetFloat(name);
    }

    public void SaveArr(int[] arr, string name)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            PlayerPrefs.SetInt(name + i.ToString(), arr[i]);
        }
    }

    public void LoadArr(int[] arr, string name)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = PlayerPrefs.GetInt(name + i.ToString());
        }
    }

    public void OnApplicationQuit()
    {
        SaveF(lm.salvageProgress, "salvageProgress");
        SaveF(lm.dustCount, "dustCount");
        SaveI(lm.salvageCount, "salvageCount");
        SaveI(lm.botCount, "botCount");
        SaveF(lm.botLifeTime, "botLifeTime");
        SaveF(lm.botProductionProgress, "botProductionProgress");
        SaveF(lm.dirtToDust, "dirtToDust");
        SaveF(lm.dirtProgress, "dirtProgress");
        SaveI(lm.unprocessedDirt, "unprocessedDirt");

        SaveArr(upm.salvage, "salvage");
        SaveArr(upm.refinery, "refinery");
        SaveArr(upm.factory, "factory");
        SaveArr(upm.powerPlant, "powerPlant");
        SaveArr(upm.livingQuarters, "livingQuarters");
        SaveArr(upm.storage, "storage");

        SaveArr(unim.availableUnis, "availableUnis");
        SaveArr(unim.salvage1Unis, "salvage1Unis");
        SaveArr(unim.salvage2Unis, "salvage2Unis");
        SaveArr(unim.space1Unis, "space1Unis");
        SaveArr(unim.space2Unis, "space2Unis");
        SaveArr(unim.factory1Unis, "factory1Unis");
        SaveArr(unim.factory2Unis, "factory2Unis");
        SaveArr(unim.powerPlant1Unis, "powerPlant1Unis");
        SaveArr(unim.powerPlant2Unis, "powerPlant2Unis");
        SaveArr(unim.refinery1Unis, "refinery1Unis");
        SaveArr(unim.refinery2Unis, "refinery2Unis");
        SaveArr(unim.storage1Unis, "storage1Unis");
        SaveArr(unim.storage2Unis, "storage2Unis");
        SaveArr(unim.shipping1Unis, "shipping1Unis");
        SaveArr(unim.shipping2Unis, "shipping2Unis");

        PlayerPrefs.SetString("id", api.usrId);
        PlayerPrefs.SetString("token", api.token);
        PlayerPrefs.SetString("refresh", api.refreshToken);

        for (int i = 0; i < su.unisList.Count; i++)
        {
            if (su.unisList[i].IsTasked == true)
            {
                PlayerPrefs.SetInt("taskedUnis" + i.ToString(), 1);
            }
            else
            {
                PlayerPrefs.SetInt("taskedUnis" + i.ToString(), 0);
            }
        }
        
        for (int i = 0; i < su.unisList.Count; i++)
        {
            if (su.unisList[i].IsTasked == true)
            {
                PlayerPrefs.SetInt("taskedUni" + i.ToString() + "Loc", su.unisList[i].TaskLoc);
            }
        }

        PlayerPrefs.Save();
    }
    
    public void Awake()
    {
        if (PlayerPrefs.GetInt("logged") == 1)
        {
            api.usrId = PlayerPrefs.GetString("id");
            api.token = PlayerPrefs.GetString("token");
            api.refreshToken = PlayerPrefs.GetString("refresh");
        }
        lm.salvageProgress = PlayerPrefs.GetFloat("salvageProgress");
        lm.dustCount = PlayerPrefs.GetFloat("dustCount");
        lm.salvageCount = PlayerPrefs.GetInt("salvageCount");
        lm.botCount = PlayerPrefs.GetInt("botCount");
        lm.botLifeTime = PlayerPrefs.GetFloat("botLifeTime");
        lm.botProductionProgress = PlayerPrefs.GetFloat("botProductionProgress");
        lm.dirtToDust = PlayerPrefs.GetFloat("dirtToDust");
        lm.dirtProgress = PlayerPrefs.GetFloat("dirtProgress");
        lm.unprocessedDirt = PlayerPrefs.GetInt("unprocessedDirt");

        LoadArr(upm.salvage, "salvage");
        LoadArr(upm.refinery, "refinery");
        LoadArr(upm.factory, "factory");
        LoadArr(upm.powerPlant, "powerPlant");
        LoadArr(upm.livingQuarters, "livingQuarters");
        LoadArr(upm.storage, "storage");

        LoadArr(unim.availableUnis, "availableUnis");
        LoadArr(unim.salvage1Unis, "salvage1Unis");
        LoadArr(unim.salvage2Unis, "salvage2Unis");
        LoadArr(unim.space1Unis, "space1Unis");
        LoadArr(unim.space2Unis, "space2Unis");
        LoadArr(unim.factory1Unis, "factory1Unis");
        LoadArr(unim.factory2Unis, "factory2Unis");
        LoadArr(unim.powerPlant1Unis, "powerPlant1Unis");
        LoadArr(unim.powerPlant2Unis, "powerPlant2Unis");
        LoadArr(unim.refinery1Unis, "refinery1Unis");
        LoadArr(unim.refinery2Unis, "refinery2Unis");
        LoadArr(unim.storage1Unis, "storage1Unis");
        LoadArr(unim.storage2Unis, "storage2Unis");
        LoadArr(unim.shipping1Unis, "shipping1Unis");
        LoadArr(unim.shipping2Unis, "shipping2Unis");

        Debug.Log("Loaded");
    }

}
