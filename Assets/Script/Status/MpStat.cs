using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MpStat : Stat
{
    public float IncreasePerSecond;
    public MpStat()
    {
        DecreaseAction = delegate (float val)
        {

        };

        IncreaseAction = delegate (float val)
        {

        };

        SetMaxAction = delegate (float val)
        {

        };
    }

    void Update()
    {
        Increase(IncreasePerSecond * Time.deltaTime);
    }
}




