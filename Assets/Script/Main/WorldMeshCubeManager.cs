using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMeshCubeManager : MonoBehaviour {
    Hashtable MeshCubeManagerTable = new Hashtable();
    string targetType = "cube_00000001";
    Vector3 pos = Vector3.zero;

    public WorldMeshCube prefab;
    static WorldMeshCubeManager _manager = null;
    public static WorldMeshCubeManager Get{get{return _manager;}}
    // Use this for initialization
    void Start () {
        if (_manager == null) _manager =this;

    }
	
	// Update is called once per frame
	void Update () {
        int max = 0;
        if (Input.GetKeyDown(KeyCode.F6))   max = 1;
        if (Input.GetKeyDown(KeyCode.F7))   max = 20;

        if (Input.GetKeyDown(KeyCode.Keypad1)) targetType = "cube_00000001";
        if (Input.GetKeyDown(KeyCode.Keypad2)) targetType = "cube_00000002";
        if (max > 0)
        {
            for (int i = 0; i < max; i++)
            {
                NewMeshCube(pos,targetType,Vector3.one);
                pos += Vector3.forward;
            }
        }
        
    }

    public Hashtable GetMatchListTextureWithDir(string type,out List<QuadManager.DIRECTION> defaultDir)
    {
        Hashtable table = new Hashtable();
        Hashtable ListTable = new Hashtable();
        QuadManager.SetDirListAll(out defaultDir);
        XmlElement CubeInfo = XMLFileLoader.Loader.File("Cube").GetNodeByID(type, "Cube");
        XmlNodeList list = CubeInfo.GetElementsByTagName("Texture");
        foreach (XmlElement node in list)
        {
            QuadManager.DIRECTION dir = QuadManager.GetDirByStr(node.GetAttribute("direction"));
            table.Add(dir, node.InnerText);
            if (ListTable.ContainsKey(node.InnerText) == false)
            {
                List<QuadManager.DIRECTION> dirList = new List<QuadManager.DIRECTION>();
                if (dir == QuadManager.DIRECTION.max) dirList = null;
                else
                {
                    defaultDir.Remove(dir);
                    dirList.Add(dir);
                }
                ListTable.Add(node.InnerText, dirList);
            }
            else
            {
                (ListTable[node.InnerText] as List<QuadManager.DIRECTION>).Add(dir);
            }
        }
        return ListTable;
    }

    public void NewMeshCube(Vector3 center, string type,Vector3 size)
    {
        List<QuadManager.DIRECTION> DefaultValueList;
        Hashtable ListTable = GetMatchListTextureWithDir(type, out DefaultValueList);

        foreach (string key in ListTable.Keys)
        {
            List<QuadManager.DIRECTION> dirlist = (ListTable[key] as List<QuadManager.DIRECTION>);
            if (dirlist == null) dirlist = DefaultValueList;
            if (MeshCubeManagerTable.ContainsKey(key) == false)
                NewMeshCubeToHashTable(key);
            (MeshCubeManagerTable[key] as WorldMeshCube).addBlock(center, size,dirlist);
        }
    }

    void NewMeshCubeToHashTable(string type)
    {
        WorldMeshCube c = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        MeshCubeManagerTable.Add(type, c);
        c.Init();
        c.type = type;
        c.TexturePath = type;
        /*XmlElement CubeInfo = XMLFileLoader.Loader.File("Cube").GetNodeByID(type, "Cube");
        c.TexturePath = XMLUtil.FindOneByTagIdValue(CubeInfo, "Texture").InnerText;*/
    }


    public void DeleteMeshDirFromCenter(Vector3 center, string type, Vector3 size, Vector3 dir)
    {
        List<QuadManager.DIRECTION> dirlist = GetDIRList(dir);
        (MeshCubeManagerTable[type] as WorldMeshCube).DeleteMeshDirFromCenter(center, size, dirlist);
    }


    public void NewMeshByDeleteCube(Vector3 center, string type, Vector3 size,Vector3 dir)
    {
        if (MeshCubeManagerTable.ContainsKey(type) == false)
            NewMeshCubeToHashTable(type);

        List<QuadManager.DIRECTION> dirlist = GetDIRList(dir);
        (MeshCubeManagerTable[type] as WorldMeshCube).addBlock(center, size,dirlist,-1f); 
    }

    public List<QuadManager.DIRECTION> GetDIRList(Vector3 v)
    {
        List<QuadManager.DIRECTION> dirlist = new List<QuadManager.DIRECTION>();
        if (v == Vector3.forward) dirlist.Add(QuadManager.DIRECTION.back);
        if (v == Vector3.back) dirlist.Add(QuadManager.DIRECTION.front);
        if (v == Vector3.left) dirlist.Add(QuadManager.DIRECTION.left);
        if (v == Vector3.right) dirlist.Add(QuadManager.DIRECTION.right);
        if (v == Vector3.up) dirlist.Add(QuadManager.DIRECTION.top);
        if (v == Vector3.down) dirlist.Add(QuadManager.DIRECTION.bottom);
        return dirlist;
    }
    public List<QuadManager.DIRECTION> GetDIRList(Vector3[] vArr)
    {
        List<QuadManager.DIRECTION> dirlist = new List<QuadManager.DIRECTION>();
        for(int i=0;i< vArr.Length;i++)
        {
            if (vArr[i] == Vector3.forward) dirlist.Add(QuadManager.DIRECTION.back);
            if (vArr[i] == Vector3.back) dirlist.Add(QuadManager.DIRECTION.front);
            if (vArr[i] == Vector3.left) dirlist.Add(QuadManager.DIRECTION.left);
            if (vArr[i] == Vector3.right) dirlist.Add(QuadManager.DIRECTION.right);
            if (vArr[i] == Vector3.up) dirlist.Add(QuadManager.DIRECTION.top);
            if (vArr[i] == Vector3.down) dirlist.Add(QuadManager.DIRECTION.bottom);
        }
        return dirlist;
    }
}
