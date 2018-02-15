using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public float OpenPercent = 0f;
    public float OpenSpeed = 1f;
    public bool enable = false;
    public PlayerInventory playerinventory;

    public void Start()
    {
        UIClick.add(gameObject, ClickUpdate);
    }

    public bool ClickUpdate(Vector2 pos, UIClick.ClickType type)
    {
        if (type != UIClick.ClickType.none)
        {
            RectTransform rect = GetComponent<RectTransform>();
            float width = rect.rect.width / 2f;
            float height = rect.rect.height / 2f;
            
            Vector2 s = new Vector2(transform.position.x, transform.position.y) - new Vector2(width, height);
            Vector2 e = new Vector2(transform.position.x, transform.position.y) + new Vector2(width, height);

            if (s.x < pos.x && pos.x < e.x && s.y < pos.y && pos.y < e.y)
            {
                Vector2 index = new Vector2(Mathf.FloorToInt((pos.x - s.x) / 32f), Mathf.FloorToInt((e.y - pos.y) / 32f));
                print(index + "  " + pos + "   " + type);
                if (0f <= index.x && index.x < 16f && 0f <= index.y && index.y < 16f)//in item range
                    return playerinventory.ClickUpdate(pos,index,type);

            }
        }
        return false;
    }

    public void Update()
    {
        if(enable)
        {
            OpenPercent += Time.deltaTime * OpenSpeed;
            if (OpenPercent > 1f) OpenPercent = 1f;
            transform.localScale = Vector3.one * OpenPercent;
        }
        else
        {
            OpenPercent -= Time.deltaTime * OpenSpeed;
            if (OpenPercent < 0f) OpenPercent = 0f;
            transform.localScale = Vector3.one * OpenPercent;
        }
    }

    public void Toggle()
    {
        enable = !enable;
    }


}