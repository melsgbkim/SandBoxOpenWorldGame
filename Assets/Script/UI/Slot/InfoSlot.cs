using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoSlot : Slot
{
    GameObject info = null;
    public override bool mouseClickUpdate(out List<object> result, Slot startClick)
    {
        result = new List<object>();
        if (MouseIn())
        {
            if (info == null)
            {
                //create info
            }
        }
        else
        {
            if (info != null)
            {
                //delete info
                info = null;
            }
        }
        return false;
    }

    public void OnDestroy()
    {
        if (info != null)
        {
            //delete info
            info = null;
        }
    }
}