using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClick : MonoBehaviour {
    public static UIClick clickManager = null;
    public List<UIData> uiList = new List<UIData>();
    public List<Slot> slotList;// = new List<Slot>();
    public enum ClickType
    {
        pressed = -2,
        down = -1,
        up = 1,
        none = 2
    }

    ClickType nowState = ClickType.none;

    public UIClick()
    {
        if (clickManager == null)
            clickManager = this;
    }
    // Use this for initialization
    void Start () {
        slotList = new List<Slot>();
        print(slotList.Count);

    }


    Slot startClick = null; 
    // Update is called once per frame
    void Update () {
             if (Input.GetMouseButtonDown(0))   nowState = ClickType.down;
        else if (Input.GetMouseButtonUp(0))     nowState = ClickType.up;
        else if (Input.GetMouseButton(0))       nowState = ClickType.pressed;
        else                                    nowState = ClickType.none;

        /*foreach(UIData data in uiList)
        {
            if (data.Callback(Input.mousePosition, nowState))
                break;
        }*/

        foreach(Slot slot in slotList)
        {
            List<object> list;
            if(slot.mouseClickUpdate(out list, startClick))
            {
                startClick = null;
                foreach (object obj in list)
                {
                    if (obj as Slot != null && startClick == null)
                        startClick = obj as Slot;
                }
                break;
            }
        }
    }

    

    public class UIData
    {
        public GameObject ui;
        public delegate bool CallbackFunc(Vector2 pos, UIClick.ClickType type);
        public CallbackFunc Callback;
        public UIData(GameObject ui, CallbackFunc func)
        {
            this.ui = ui;
            this.Callback = func;
        }
    }

    public static void add(GameObject ui, UIData.CallbackFunc func)
    {
        clickManager.uiList.Add(new UIData(ui, func));
    }

    public static void add(Slot slot)
    {
        clickManager.slotList.Add(slot);
    }
}
