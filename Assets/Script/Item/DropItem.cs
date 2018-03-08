using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour {
    public string DropItemPrefabPath = "";
    public Item item = null;
    public PlayerInventory Player = null;
    public Transform terget = null;
    public bool FollowMode = false;
    public float followTime = 1f/3f;
    public bool WillDeleted = false;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (FollowMode)
        {
            transform.position = (transform.position + (terget.position - transform.position) * (followTime * 2f));
            followTime -= Time.deltaTime;
            if (followTime < 0f)
            {
                if(Player == null)
                    terget.localScale = Vector3.one * 1.5f;
                Destroy(gameObject);
                return;
            }
            transform.localScale = Vector3.one * (followTime * 3f);
        }
        else
        {
            transform.localScale -= (transform.localScale - Vector3.one) * 0.5f;
            transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == gameObject.tag)
        {
            DropItem i = other.gameObject.GetComponent<DropItem>();
            if(i != null)
            {
                i.SumDropItem(this);
            }
            
        }
    }

    public void SumDropItem(DropItem other)
    {
        if (item.name == other.item.name && 
            other.WillDeleted == false && 
            WillDeleted == false)
        {
            ItemStackable s = other.item as ItemStackable;
            ItemStackable my = item as ItemStackable;
            
            if (s != null && my != null && (s.ItemFull() == false && my.ItemFull() == false))
            {
                float difference = s.StackCount;
                s.StackCount = my.ItemGet(s.StackCount);
                //print(difference+">>"+ s.StackCount);
                difference -= s.StackCount;
                if (s.StackCount <= 0)
                {
                    other.WillDeleted = true;
                    other.StartFollow(transform);
                }
                else if(difference > 0)
                {
                    
                    GameObject newBlock = Instantiate((GameObject)Resources.Load("prefab/DropItem"), other.transform.position, Quaternion.identity);
                    DropItem drop = newBlock.GetComponent<DropItem>();
                    drop.setItemObj(item.previewPath);
                    drop.WillDeleted = true;
                    drop.StartFollow(transform);
                }
            }
        }
    }

    public void StartFollowPlayer(PlayerInventory p)
    {        
        if (p.CanGetItem(item))
        {
            Player = p;
            p.GetItem(item);
            StartFollow(Player.transform);
        }
    }

    public void StartFollow(Transform t)
    {
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        terget = t;
        FollowMode = true;
    }

    public void setItem(Item i)
    {        
        item = i;
        GameObject obj = setItemObj(i.previewPath);
        ItemCube itemCube = i as ItemCube;
        if (itemCube != null)
            setItemTexture(obj,PathManager.CubeTexturePath(itemCube.type));
    }

    public GameObject setItemObj(string path)
    {
        GameObject obj = Instantiate((GameObject)Resources.Load(path), Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0,1/3f,0);
        obj.transform.localScale = Vector3.one / 3f / 1.5f;
        return obj;
    }
    public void setItemTexture(GameObject obj, string path)
    {
        setItemTexture(obj,TextureManager.Load(path));
    }
    public void setItemTexture(GameObject obj,Texture txt)
    {
        if (txt == null) return;

        obj.GetComponent<MeshRenderer>().material.mainTexture = txt;
    }




    public static void DropItemPos(Item i,Vector3 pos,Vector3 vel)
    {
        GameObject tmp = Instantiate((GameObject)Resources.Load("prefab/DropItem"), pos, Quaternion.identity);
        Rigidbody rigid = tmp.GetComponent<Rigidbody>();
        if(rigid != null)
        {
            rigid.velocity = vel;
        }

        DropItem drop = tmp.GetComponent<DropItem>();
        if(drop != null)
        {
            drop.setItem(i);
        }
    }

    public static void DropItemPosCube(Cube.TYPE type, float count,Vector3 pos, Vector3 vel)
    {
        ItemCube i = new ItemCube(type);
        i.ItemGet(count);
        DropItemPos(i, pos, vel);
    }

    public static Vector3 RandomUpperVel()
    {
        return (Quaternion.Euler(0, Random.RandomRange(0, 360), 0) * new Vector3(0, 0, 1) + Vector3.up*2);
    }




}
