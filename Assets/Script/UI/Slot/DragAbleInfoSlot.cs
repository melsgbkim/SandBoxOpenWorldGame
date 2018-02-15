using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAbleInfoSlot : Slot
{
    public override bool mouseClickUpdate(out List<object> result, Slot startClick)
    {
        result = new List<object>();
        if (MouseIn() && Input.GetMouseButtonDown(0))
        {
            result.Add(target);
            return true;
        }
        return false;
    }

    public void OnDestroy()
    {

    }
}

