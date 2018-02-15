using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Slot : MonoBehaviour
{
    public Slot() { }
    public object target = null;
    public Vector2 s = Vector2.zero;
    public Vector2 e = Vector2.zero;

    public virtual bool canStoreTarget(object value) { return false; }
    public virtual bool mouseClickUpdate(out List<object> result, Slot startClick) { result = new List<object>(); return false; }
    public delegate void updateOther();
    public updateOther updateFromOther = delegate() { };

    public bool MouseIn()
    {
        return MouseIn(Input.mousePosition);
    }

    public bool MouseIn(Vector3 pos)
    {
        RectTransform rect = GetComponent<RectTransform>();
        
        if (pos.x < s.x) return false;
        if (pos.y < s.y) return false;
        if (pos.x > e.x) return false;
        if (pos.y > e.y) return false;
        return true;
    }

    public void SetTarget(object obj)
    {
        target = obj;
        updateFromOther();
    }


}




