using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMpStat : MpStat
{

    public BarUI ui;
    public PlayerMpStat()
    {
        DecreaseAction = delegate (float val)
        {

        };

        IncreaseAction = delegate (float val)
        {

        };

        SetMaxAction = delegate (float val)
        {
            ui.SetBarLength(val);
        };

    }

    void Start()
    {
        init();
    }



    public void init()
    {
        float val = 30f;
        IncreasePerSecond = 3f;
        ui.Init(val);
        SetMax(val);
        Increase(val);
    }

    public override void update()
    {
        base.update();
        ui.SetBarValue(value);
    }
}




