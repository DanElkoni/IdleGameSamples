// Daniel Elkoni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Script that handles idle logic
// This script accounts for offline time allowing progress up to the limit, and running the game for said time

public class IdleScript : MonoBehaviour
{
    public LogicManager logicManager;

    public TimeSpan diffOfTimes;
    public double secsBetween;

    // Time limit set for max offline time
    private int limit;

    // Start is called before the first frame update
    void Awake()
    {
        // Limit will be hard set, in this case one hour
        limit = 3600;

        // Getting current date
        DateTime currentDate = DateTime.Now;
        print(currentDate.ToString());

        // Loading last date that was aquired on app shutdown
        long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString"));
        DateTime oldDate = DateTime.FromBinary(temp);
        print(oldDate.ToString());

        // Getting difference between now and last opened date
        diffOfTimes = currentDate.Subtract(oldDate);

        secsBetween = diffOfTimes.TotalSeconds;

        // Time limit, if offline timer longer than the limit only one hour of progress is awarded
        if (secsBetween < 3600)
        {
            for (double d = 0; d <= secsBetween; d++)
            {
                logicManager.OneTick();
            }
        }
        else
        {
            for (double d = 0; d <= 3600; d++)
            {
                logicManager.OneTick();
            }
        }
    }

    // Saves date on quitting application
    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
    }
}
