using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    public Vector2 size;
    public InventoryUI inventoryUI;
    public GameObject itemUI;

    public Hashtable ItemTable = new Hashtable();
    public List<Vector2> EmptyList = new List<Vector2>();
    public ItemSlot[][] ItemSlotArr;
    public static PlayerInventory inventory = null;


    public KeyCode InventoryOpenKey = KeyCode.I;
    // Use this for initialization
    void Start () {
        ItemSlotArr = new ItemSlot[16][];
        for (int i = 0; i < 16; i++)
        {
            ItemSlotArr[i] = new ItemSlot[16];
        }

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                EmptyList.Add(new Vector2(j, i));
                
                GameObject itemui = Instantiate(itemUI);
                RectTransform rect = itemui.GetComponent<RectTransform>();
                rect.SetParent(inventoryUI.transform);
                itemui.GetComponent<ItemUI>().UpdatePosition(new Vector2(j, i));
                //rect.localPosition = new Vector3(j * 32, i * -32, 0) - new Vector3(256, -256, 0);
                rect.localScale = Vector3.one;
                ItemSlotArr[i][j] = itemui.GetComponent<ItemSlot>();

            }
        }

        if (inventory == null) inventory = this;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.F3))
        {
            //Instantiate((GameObject)Resources.Load("prefab/DropItem"), new Vector3(-3, 8, 0), Quaternion.identity);
            GameObject tmp;
            tmp = Instantiate((GameObject)Resources.Load("prefab/DropItem"), new Vector3(-3, 3, 0), Quaternion.identity);
            DropItem drop = tmp.GetComponent<DropItem>();
            ItemCube item = new ItemCube(Cube.TYPE.Grass);
            item.StackCount = 9999;
            drop.setItem(item);

            //AddItem(item);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ItemCube item;
            for (int i = 0; i < 255; i++)
            {
                item = new ItemCube(Cube.TYPE.Grass);
                item.StackCount = 9999;
                AddItem(item);
            }
            item = new ItemCube(Cube.TYPE.Grass);
            item.StackCount = 9998;
            AddItem(item);
        }

        if (Input.GetKeyDown(InventoryOpenKey))
            inventoryUI.Toggle();
    }

    void AddItem(Item i)
    {
        if(ItemTable.ContainsKey(i.name) && i.Stackable && ItemTable[i.name] != null && (ItemTable[i.name] as List<Item>).Count > 0)
        {
            List<Item> list = ItemTable[i.name] as List<Item>;
            Item item = list[0] as Item;
            float newCount = item.ItemGet(i.StackCount);
            if(newCount > 0)
            {
                i.StackCount = newCount;
                newItem(i, list);
            }
        }
        else if(ItemTable.ContainsKey(i.name))
        {
            newItem(i, ItemTable[i.name] as List<Item>);
        }
        else
        {
            List<Item> list = new List<Item>();
            ItemTable.Add(i.name, list);
            newItem(i, list);
        }
    }

    void newItem(Item i)
    {
        List<Item> list;
        if (ItemTable.ContainsKey(i.name))
        {
            list = ItemTable[i.name] as List<Item>;
        }
        else
        {
            list = new List<Item>();
            ItemTable.Add(i.name, list);
        }
        newItem(i, list);
    }


    void newItem(Item i, List<Item> list)
    {        
        if (EmptyList.Count > 0)
        {
            if (list.Count > 0) list.Insert(0, i);
            else list.Add(i);
            ItemSlotArr[(int)(EmptyList[0].y)][(int)(EmptyList[0].x)].SetTarget(i);
            print("Pos : " + EmptyList[0] + "  count : "+i.StackCount);
            EmptyList.RemoveAt(0);
        }
        else
        {//drop New Item
            DropItem.DropItemPos(i, transform.position, DropItem.RandomUpperVel()*3);
        }
    }

    public void GetItem(Item i)
    {
        if (i != null)
        {
            AddItem(i);
        }
    }

    public bool CanGetItem(Item i)
    {
        if (isInventoryFull() == false)
            return true;
        if (ItemTable.ContainsKey(i.name) && i.Stackable && ItemTable[i.name] != null)
            return ((ItemTable[i.name] as List<Item>)[0] as ItemStackable).ItemFull() == false;
        else
            return false;
    }

    public bool isInventoryFull()
    {
        return EmptyList.Count == 0;
    }

    public void SortEmptySlot()
    {
        EmptyList.Sort(delegate (Vector2 v1, Vector2 v2)
        {
            if (v1.y > v2.y) return 1;
            else if (v1.y < v2.y) return -1;
            else
            {
                if (v1.x > v2.x) return 1;
                else if (v1.x < v2.x) return -1;
                else return 0;
            }
        });
    }

    public void ChangeEmptySlot(Vector2 v1, Vector2 v2)
    {
        int index1 = EmptyList.IndexOf(v1);
        int index2 = EmptyList.IndexOf(v2);

        if (index1 > -1) EmptyList[index1] = v2;
        if (index2 > -1) EmptyList[index2] = v1;
    }

    public void ItemExhaust(Item i)
    {
        if(i.Stackable == false || (i.Stackable && i.StackCount <= 0f))
        {
            List<Item> itemlist = ItemTable[i.name] as List<Item>;
            itemlist.Remove(i);
            Vector2 index = new Vector2(i.ui.slot.index.x, i.ui.slot.index.y);
            ItemSlotArr[(int)index.y][(int)index.x].target = null;
            ItemSlotArr[(int)index.y][(int)index.x].updateFromOther();
            EmptyList.Add(index);
            SortEmptySlot();
        }

        List<Item> list = ItemTable[i.name] as List<Item>;
    }

    bool dragItem = false;
    Vector2 clickIndex = -Vector2.one;
    public bool ClickUpdate(Vector2 pos, Vector2 index, UIClick.ClickType type)
    {
        
        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DropItem")
        {
            DropItem item = other.gameObject.GetComponent<DropItem>();
            item.StartFollowPlayer(this);
        }
    }
}
