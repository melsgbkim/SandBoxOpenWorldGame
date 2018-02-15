using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : Slot
{
    public Vector2 index;
    public void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        s = new Vector2(transform.position.x, transform.position.y - rect.rect.height);
        e = new Vector2(transform.position.x + rect.rect.width, transform.position.y);
    }
    public override bool mouseClickUpdate(out List<object> result, Slot startClick)
    {
        result = new List<object>();
        if (MouseIn() && Input.GetMouseButtonDown(0))
        {
            result.Add(this);
            return true;
        }
        else if(MouseIn() && Input.GetMouseButtonUp(0))
        {
            ItemSlot startSlot = startClick as ItemSlot;
            if (startSlot != null)
            {
                if(startSlot != this)
                {//item slot change
                    object tmp = target; this.target = startSlot.target; startSlot.target = tmp;
                    bool tmpb = MouseIn();
                    tmpb = startSlot.MouseIn();
                    //Vector2 tmpindex = index; this.index = startSlot.index; startSlot.index = tmpindex;

                    PlayerInventory.inventory.ChangeEmptySlot(index,startSlot.index);
                    updateFromOther();
                    startSlot.updateFromOther();

                    PlayerInventory.inventory.SortEmptySlot();
                }
            }
            return true;
        }
        return false;
    }

    



    public void OnDestroy()
    {

    }
}

