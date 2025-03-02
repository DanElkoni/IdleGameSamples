// Daniel Elkoni, made in 2022

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// C# code from an idle game, your bots would process salvage and dirt over time, and you would use the materials to upgrade your characters, the unicorns
// This file does rely on external APIs that are no longer running to function, so it does not work in the conventional sense

// This file mainly handled most of logc for the game while running client side, including what happens per game tick, etc.

public class LogicManager : MonoBehaviour
{
    public bool salvageOut = false;

    public BotAnimation botAnim;

    public APIHandler su;
    
    public Slider energyBar;
    public TextMeshProUGUI energyText;
    public float energy;

    public int botsMade;
    public int dirtRefined;
    public int scrapRuns;

    public float salvageProgress;

    public float dustCount;
    public int salvageCount;
    
    public int salvageTime = 3600;

    public int[] salvageRange = {5,5,5,5,5,10,10,10,10,10,10,10,10,10,10,15,15,15,25,25,50};

    public int botCount;
    public float botLifeTime;
    public float botMaxLifeTime;

    public float botProductionProgress;
    public float botProductionTime;

    public float dirtToDust;
    public float dirtToDustTime;

    public float dirtProgress;
    public float dirtTime;
    public int unprocessedDirt;

    public bool refineryBackedUp;
    public bool isprocessing;

    public int dustStorage;
    public int salvageStorage;

    public float shipProg;

    public bool facIsActive;

    public GameObject[] taskSlots = new GameObject[10];

    // Manager imports
    public UniManager uniManager;
    public UpgradeManager upgradeManager;

    // Logic for one game tick/one second
    public void OneTick()
    {
        if (shipProg >= 3600f)
        {
            if (dustCount >= (upgradeManager.shipping[0]+1) * .1f * 10 + 10) 
            {
                shipProg = 0;
                dustCount -= (upgradeManager.shipping[0] + 1) * .1f * 10 + 10;
                su.SendDust((upgradeManager.shipping[0] + 1) * .1f * 10 + 10);
            }
            else
            {   
                // Api calls adding 
                shipProg = 0;
                su.SendDust(dustCount);
                dustCount = 0;
            }
        }
        else
        {
            // Progress multipliers using the different "tiers" of characters
            shipProg += uniManager.shipping1Unis[0] * .04f + uniManager.shipping1Unis[1] * .08f + uniManager.shipping1Unis[2] * .16f + uniManager.shipping1Unis[3] * .32f + upgradeManager.shipping[2] * .1f; 
        }
        if (dustCount < dustStorage)
        {
            if (refineryBackedUp != true)
            {
                float dirtProgressBoost = (upgradeManager.factory[2] * .1f) + (uniManager.factory2Unis[0] * .04f) + (uniManager.factory2Unis[1] * .08f) + (uniManager.factory2Unis[2] * 0.16f) + (uniManager.factory2Unis[3] * .32f);
                float botProgressBoost = (upgradeManager.factory[0] * .1f) + (uniManager.factory1Unis[0] * .04f) + (uniManager.factory1Unis[1] * .08f) + (uniManager.factory1Unis[2] * 0.16f) + (uniManager.factory1Unis[3] * .32f);

            if (facIsActive == true && salvageCount > 0) {
                botProductionProgress += 1 + botProgressBoost;

                if (botProductionProgress >= botProductionTime)
                {
                    botAnim.PlayAnim("Factory ", 5);
                    botCount++;
                    botsMade++;
                    botProductionProgress = 0;
                    salvageCount--;
                }

            }
                if (botCount > 0)
                {
                    if (botLifeTime >= botMaxLifeTime)
                    {
                        botCount--;
                        botLifeTime = 0;
                    }

                    if (botCount > 0)
                    {
                        botLifeTime++;

                        if (dirtProgress >= dirtTime)
                        {
                            botAnim.PlayAnim("Mine ", 5);
                            unprocessedDirt += 1;
                            dirtProgress = 0;
                        }
                        else
                        {
                            dirtProgress += 1 + dirtProgressBoost;
                        }
                    }
                }
            }
        }

        if (unprocessedDirt > 0 & isprocessing == false)
        {
            isprocessing = true;
            dirtToDust = 0;
        }

        if (isprocessing == true & dirtToDust < dirtToDustTime)
        {
            dirtToDust += 1 + (upgradeManager.refinery[0] * .1f) + (uniManager.refinery1Unis[0] * .04f) + (uniManager.refinery1Unis[1] * .08f) + (uniManager.refinery1Unis[2] * 0.16f) + (uniManager.refinery1Unis[3] * .32f);
        }

        if (dirtToDust >= dirtToDustTime & unprocessedDirt > 0)
        {
            float randomDust = salvageRange[Random.Range(0, salvageRange.Length - 1)] / 100f;
            float dirtBoost = 1f + (upgradeManager.refinery[2] * .1f) + (uniManager.refinery2Unis[0] * .04f) + (uniManager.refinery2Unis[1] * .08f) + (uniManager.refinery2Unis[2] * 0.16f) + (uniManager.refinery2Unis[3] * .32f);

            if (dustCount + (Mathf.CeilToInt(randomDust * dirtBoost * 100f) / 100f) > dustStorage)
            {
                dustCount = dustStorage;
            }
            else
            {
                dustCount += (Mathf.CeilToInt(randomDust * dirtBoost * 100f) / 100f);
            }

            dirtRefined++;
            unprocessedDirt--;
            dirtToDust = 0;
            isprocessing = false;
            botAnim.PlayAnim("Refinery ", 5);
        }

        // Salvage logic
            
        if (salvageCount < salvageStorage)
        {
            if (salvageProgress >= salvageTime)
            {
                float randomSalvage = salvageRange[Random.Range(0, salvageRange.Length - 1)];
                float salvageBoost = 1 + (upgradeManager.salvage[0] * .1f) + (uniManager.salvage2Unis[0] * .04f) + (uniManager.salvage2Unis[1] * .08f) + (uniManager.salvage2Unis[2] * 0.16f) + (uniManager.salvage2Unis[3] * .32f);

                if (salvageCount + Mathf.CeilToInt(randomSalvage * salvageBoost) > salvageStorage)
                {
                    salvageCount = salvageStorage;
                }
                else
                {
                    salvageCount += Mathf.CeilToInt(randomSalvage * salvageBoost);
                }

                scrapRuns++;
                salvageProgress = 0;
                botAnim.PlayAnim("SalvageOut ", 5);
                salvageOut = true;
            }
            else 
            {
                float progressBoost = (upgradeManager.salvage[0] * .1f) + (uniManager.salvage1Unis[0] * .04f) + (uniManager.salvage1Unis[1] * .08f) + (uniManager.salvage1Unis[2] * 0.16f) + (uniManager.salvage1Unis[3] * .32f);
                salvageProgress += 1 + progressBoost; 
            }
        }
        if (salvageCount >= 3595 & salvageOut == true)
        {
            botAnim.PlayAnim("SalvageIn ", 5);
        }
    }

    // Logic for updating storage, grabbing storage values and keeping them consistent
    void UpdateStorage()
    {
        if (unprocessedDirt >= 5)
        {
            refineryBackedUp = true;
        }
        else
        {
            refineryBackedUp = false;
        }

        float dustStorageBoost = (upgradeManager.storage[0] * .1f) + (uniManager.storage1Unis[0] * .04f) + (uniManager.storage1Unis[1] * .08f) + (uniManager.storage1Unis[2] * 0.16f) + (uniManager.storage1Unis[3] * .32f);
        float salvageStorageBoost = (upgradeManager.storage[2] * .1f) + (uniManager.storage2Unis[0] * .04f) + (uniManager.storage2Unis[1] * .08f) + (uniManager.storage2Unis[2] * 0.16f) + (uniManager.storage2Unis[3] * .32f);

        dustStorage = 100 + Mathf.CeilToInt(100 * dustStorageBoost);
        salvageStorage = 100 + Mathf.CeilToInt(100 * salvageStorageBoost);

        if (energy <= 0)
        {
            energy = 0;
        }

    }

    void Start()
    {
        isprocessing = false;
        facIsActive = true;
    }

    // Run a tick every second
    void FixedUpdate()
    {
        OneTick();
    }

    // Constantly update storage numbers
    void Update()
    {
        UpdateStorage();
    }
}
