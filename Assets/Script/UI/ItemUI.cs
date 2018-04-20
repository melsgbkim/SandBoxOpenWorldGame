using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour {
    public RawImage itemIcon;
    public RawImage itemGrade;
    public RawImage itemBack;
    public Text txt;
    public ItemCubeIcon itemCubeIcon;
    public Item item = null;
    public ItemSlot slot;
    // Use this for initialization
    void Start () {
        slot = GetComponent<ItemSlot>();
        slot.updateFromOther = UpdateFromSlot;
        UIClick.add(slot);

        if (item == null) setItem(item);
    }

    public void UpdateFromSlot()
    {
        ItemSlot slot = GetComponent<ItemSlot>();
        if(item != (slot.target as Item))
            setItem(slot.target as Item);
        UpdatePosition(slot.index);
        UpdateStackCount();
    }

    // Update is called once per frame
 
	void Update () {

	}

    public void setItem(Item i)
    {
        item = i;
        if (item == null)
        {
            itemIcon.enabled = false;
            itemGrade.enabled = false;
            itemBack.enabled = false;
            txt.enabled = false;
            itemCubeIcon.enabled = false;
            ItemSlot slot = GetComponent<ItemSlot>();
            slot.target = null;
        }
        else
        {
            i.ui = this;
            
            itemGrade.enabled = true;
            itemBack.enabled = true;
            txt.enabled = true;

            if(i as ItemCube == null)
            {
                itemIcon.enabled = true;
                itemIcon.texture = (Texture)Resources.Load("ItemIcon/item_" + item.name);
            }
            else
            {
                itemCubeIcon.enabled = true;

                //to do : change to xml
                itemCubeIcon.SetCubeType((i as ItemCube).type);
            }
            

            
            itemGrade.texture = (Texture)Resources.Load("ItemIcon/item_grade_" + item.grade);
            UpdateStackCount();
            ItemSlot slot = GetComponent<ItemSlot>();
            slot.target = item;
        }
    }

    public void UpdatePosition(Vector3 index)
    {
        RectTransform rect = GetComponent<RectTransform>();
        if(rect != null)
            rect.localPosition = new Vector3(index.x * 32, index.y * -32, 0) - new Vector3(256, -256, 0);
        ItemSlot slot = GetComponent<ItemSlot>();
        slot.index = index;
    }

    public void UpdateStackCount()
    {
        if (item != null && item.Stackable)
            txt.text = "" + (Mathf.Round(item.StackCount * 10) / 10f);
        else
            txt.text = "";
    }
}