using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSlot : Slot
{
    public void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();

        s = new Vector2(transform.position.x , transform.position.y) - new Vector2(rect.rect.width, rect.rect.height) / 2f;
        e = new Vector2(transform.position.x , transform.position.y) + new Vector2(rect.rect.width, rect.rect.height) / 2f;
    }
    public override bool mouseClickUpdate(out List<object> result, Slot startClick)
    {
        result = new List<object>();

        if (MouseIn() && Input.GetMouseButtonDown(0))
        {
            result.Add(this);
            return true;
        }
        else if (MouseIn() && Input.GetMouseButtonUp(0))
        {
            ActionSlot startActionSlot = startClick as ActionSlot;
            if (startActionSlot != null)
            {
                if (startActionSlot != this)
                {//item slot change
                    object tmp = target; this.target = startActionSlot.target; startActionSlot.target = tmp;
                    startActionSlot.updateFromOther();
                    updateFromOther();
                }
                return true;
            }

            DragAbleInfoSlot startInfoSlot = startClick as DragAbleInfoSlot;
            if(startInfoSlot != null)
            {
                this.target = startInfoSlot.target;
                updateFromOther();
                return true;
            }

            ItemSlot startItemSlot = startClick as ItemSlot;
            if (startItemSlot != null)
            {
                this.target = startItemSlot.target;
                updateFromOther();
                return true;
            }
        }
        return false;
    }



    public void OnDestroy()
    {

    }
}

