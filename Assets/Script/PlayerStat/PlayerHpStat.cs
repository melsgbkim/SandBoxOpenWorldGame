using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHpStat : HpStat
{
    public BarUI ui;
    public PlayerHpStat()
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

    void Update()
    {
        
    }

    public void init()
    {
        ui.Init(100);
        SetMax(100);
        Increase(100);        
    }

    public override void update()
    {
        base.update();
        ui.SetBarValue(value);
    }
}




