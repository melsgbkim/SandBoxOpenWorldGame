using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HpStat : Stat
{
    public HpStat()
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

    public void Init(float val)
    {
        SetMax(val);
        Increase(val);
    }

}




