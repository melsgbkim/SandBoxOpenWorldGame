﻿using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    private string _id = "";

    public string name = "";
    public Vector2 pos = Vector2.zero;
    //    public Vector2 size = Vector2.zero;
    public bool Stackable = false;
    public float _StackCount = 0;
    public float StackCount
    {
        get { return _StackCount; }
        set { _StackCount = value; UpdateCountToUI(); }
    }

    public float StackMax = 0;
    public CoolTime coolTime = new CoolTime();
    public string iconPath = "";
    public string previewPath = "";
    public string grade = "0";
    public ItemUI ui = null;

    public virtual bool ItemWillDelete() { return false; }
    public virtual bool ItemAction() { return false; }
    public virtual void ItemUse() { }
    public virtual float ItemGet(float count) { return count; }
    public virtual bool isItemUseEnd() { return true; }
    public virtual bool CanItemGet() { return StackCount < StackMax; }
    public virtual bool CanItemUse(float count) { return StackCount >= count; }
    public virtual void DeleteItem(float count) { StackCount = StackCount - count; }
    public virtual void ItemUpdate(GameObject Player) { }

    public void UpdateCountToUI()
    {
        if (ui != null) ui.UpdateStackCount();
    }

    public string id
    {
        get { return _id; }
        set
        {
            _id = value;
            XmlElement Item = XMLFileLoader.Loader.File("Item").GetNodeByID(value, "Item");
            name = XMLUtil.FindOneByTagIdValue(Item, "Name").InnerText;
            grade = XMLUtil.FindOneByTagIdValue(Item, "Grade").InnerText;
            iconPath = XMLUtil.FindOneByTagIdValue(Item, "Texture", "type", "ItemIcon").InnerText;
            XmlElement prefabElement = XMLUtil.FindOneByTagIdValue(Item, "PreviewItemPrefab");
            switch (prefabElement.GetAttribute("path"))
            {
                case "all": previewPath = prefabElement.InnerText; break;
                case "default": previewPath = PathManager.previewPath + prefabElement.InnerText; break;
            }
        }
    }

}

public class ItemEquipment : Item
{
    public ItemEquipment(string ItemCode)
    {
        iconPath = PathManager.iconPath + "item_tmp";
        previewPath = PathManager.previewPath + "2DItem";
    }
}

public class ItemStackable : Item
{
    public virtual bool ItemAction(float count) { return false; }
    public override bool ItemWillDelete()
    {
        return StackCount <= 0f;
    }
    public virtual void ItemUse(float count)
    {
        if (ItemAction(count))
            DeleteItem(count);
    }

    public override float ItemGet(float count)
    {
        int result = 0;
        StackCount = StackCount + count;
        if (StackCount >= StackMax)
        {
            result = (int)(StackCount - StackMax);
            StackCount = StackMax;
        }
        return result;
    }

    public bool ItemFull()
    {
        return StackCount == StackMax;
    }
}

public class ItemCube : ItemStackable
{
    public string type;
    public bool BulidMode = false;
    public bool start = false;
    public Vector3 BulidPosition = Vector3.zero;
    public GameObject point;
    public GameObject pointCube;

    public ItemCube(string id)
    {
        coolTime.time = 1 / 60f;
        this.type = id;
        Stackable = true;
        StackMax = 9999;
        StackCount = 1;

        this.id = id;
    }

    public ItemCube(string id,float count)
    {
        coolTime.time = 1 / 60f;
        this.type = id;
        Stackable = true;
        StackMax = 9999;
        StackCount = count;
        MonoBehaviour.print("ItemCube id:" + id+",   count:"+count);

        this.id = id;
    }

    public override void ItemUse(float count)
    {
        if (BulidMode == false && start == false && this.StackCount > 0)
        {
            BulidMode = true;
            start = true;
        }
        else if(BulidMode == true && start == false)
        {
            if (ItemAction(count))
                DeleteItem(count);
        }
    }
    public override bool isItemUseEnd()
    {
        return BulidMode == false;
    }
    public override bool ItemAction(float count)
    {
        XmlElement Item = XMLFileLoader.Loader.File("Item").GetNodeByID(id, "Item");
        XmlNodeList actionList = Item.GetElementsByTagName("Action");
        bool result = false;
        foreach (XmlElement node in actionList)
        {
            switch(node.GetAttribute("type"))
            {
                case "BuildCube": result = BlockManager.manager.AddBlock(new Vector3(Mathf.Round(BulidPosition.x * 3), Mathf.Round(BulidPosition.y * 3), Mathf.Round(BulidPosition.z * 3)), Vector3.one, node.InnerText, true); break;
            }
        }
        
        if (result == false)
        {
            MonoBehaviour.print(BulidPosition + " >> " + type + " >> cound : " + count);
        }
        return result;
    }

    public override void ItemUpdate(GameObject Player)
    {
        if (start)
        {
            start = false;
            BulidPosition = Player.transform.position;
            point = MonoBehaviour.Instantiate((GameObject)Resources.Load("prefab/SelectPoint"));
            pointCube = MonoBehaviour.Instantiate((GameObject)Resources.Load("prefab/SelectCube"));
        }
        Vector3 dir = Vector3.ClampMagnitude(Player.GetComponent<PlayerMove>().GetOtherVector3Dir(), 1);
        RaycastHit hit;
        float speed = Player.GetComponent<PlayerMove>().moveSpeed;
        BulidPosition += speed * Time.deltaTime * dir;
        point.transform.position = BulidPosition;
        pointCube.transform.position = new Vector3(Mathf.Round(BulidPosition.x * 3) / 3f, Mathf.Round(BulidPosition.y * 3) / 3f, Mathf.Round(BulidPosition.z * 3) / 3f);
        //MonoBehaviour.print(BulidPosition);
        if (Input.GetKeyDown(Player.GetComponent<PlayerInput>().KeyCancel) || StackCount <= 0)
        {
            BulidMode = false;
            MonoBehaviour.Destroy(point);
            MonoBehaviour.Destroy(pointCube);
            if (StackCount <= 0)
            {
                Player.GetComponent<PlayerInventory>().ItemExhaust(this);
                
            }
        }
    }
}



