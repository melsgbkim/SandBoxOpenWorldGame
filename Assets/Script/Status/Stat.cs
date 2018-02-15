using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Stat : MonoBehaviour
{
    public float value;
    public float max = 0f;
    public bool enable;

    public Stat() { }
    public virtual void Decrease(float val)
    {
        value -= val;
        if (value < 0)
            value = 0;
        DecreaseAction(val);
        update();
    }

    public virtual void Increase(float val)
    {
        value += val;
        if (max > 0 && value > max)
            value = max;
        IncreaseAction(val);
        update();
    }

    public virtual void SetMax(float m)
    {
        max = m;
        SetMaxAction(m);
        update();
    }

    public bool CanChange(float val)
    {
        return 0 <= (value + val) && (value + val) <= max;
    }

    public delegate void ACTION(float val);
    public ACTION DecreaseAction;
    public ACTION IncreaseAction;
    public ACTION SetMaxAction;
    public virtual void update() { }
}




