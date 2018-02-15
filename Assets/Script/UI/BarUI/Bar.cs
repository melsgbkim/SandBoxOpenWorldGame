using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public GameObject bar;
    public Vector2 barRange;
    public float _value;
    public float Max;
    public float ScaleBar
    {
        set { bar.transform.localScale = new Vector3(value, 1, 1); }
    }
    public float ScaleX
    {
        set { transform.localScale = new Vector3(value, 1, 1); }
    }

    public void SetValue(float val, Vector2 AllRange)
    {
        _value = val;
        Vector2 range = barRange;

        float tmp = ((val - range.x) / (range.y - range.x));
        if (tmp < 0) tmp = 0;
        else if (tmp > 1) tmp = 1;
        ScaleBar = tmp;
    }
}