using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    List<Bar> BarList = new List<Bar>();
    public static Texture blank;
    public float length = 0f;
    public float value = 0f;
    public Bar bar = null;
    public void SetBarLength(float len)
    {
        if (len < 0) return;
        length = len;
    }

    public void SetBarValue(float val)
    {
        if (val < 0) val = 0;
        if (val > length) val = length;
        for (int i = 0; i < 20; i++)
        {
            BarList[i].SetValue(val / length, new Vector2(0, length));
        }

        value = val;
    }

    private void Init(float len, float val)
    {
        for(int i=0;i<20;i++)
        {
            Bar obj = Instantiate(bar) as Bar;
            BarList.Add(obj);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(i * 32, 0, 0);
            obj.barRange = new Vector2(i, i + 1)/20f;
        }


        SetBarLength(len);
        value = val;
    }

    public void Init(float len)
    {
        Init(len, len);
    }


    void Start()
    {
        //Init(1000f);
    }

    void Update()
    {
        //SetBarValue(value - 10 * Time.deltaTime);


    }
}