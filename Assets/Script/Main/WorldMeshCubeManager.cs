using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMeshCubeManager : MonoBehaviour {
    Hashtable MeshCubeManagerTable = new Hashtable();
    Cube.TYPE targetType = Cube.TYPE.Grass;
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

        if (Input.GetKeyDown(KeyCode.Keypad1)) targetType = Cube.TYPE.Grass;
        if (Input.GetKeyDown(KeyCode.Keypad2)) targetType = Cube.TYPE.Dirt;
        if (max > 0)
        {
            for (int i = 0; i < max; i++)
            {
                NewMeshCube(pos,targetType);
                pos += Vector3.forward;
            }
        }
        
    }

    void NewMeshCube(Vector3 center, Cube.TYPE type)
    {
        if (MeshCubeManagerTable.ContainsKey(type) == false)
            NewMeshCubeToHashTable(type);
        (MeshCubeManagerTable[type] as WorldMeshCube).addBlock(center);
    }

    public void NewMeshCube(Vector3 center, Cube.TYPE type,Vector3 size)
    {
        if (MeshCubeManagerTable.ContainsKey(type) == false)
            NewMeshCubeToHashTable(type);
        (MeshCubeManagerTable[type] as WorldMeshCube).addBlock(center, size);
    }

    void NewMeshCubeToHashTable(Cube.TYPE type)
    {
        WorldMeshCube c = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        MeshCubeManagerTable.Add(type, c);
        c.Init();
        c.type = type;
        switch (targetType)
        {
            case Cube.TYPE.Grass: c.TexturePath = "Texture/CubeGrass"; break;
            case Cube.TYPE.Dirt: c.TexturePath = "Texture/CubeDirt"; break;
            case Cube.TYPE.Air: c.TexturePath = "Texture/CubeNone"; break;
        }
    }

    public void DeleteMeshRange(Vector3 center, Cube.TYPE type, Vector3 size)
    {
        (MeshCubeManagerTable[type] as WorldMeshCube).AddReverseBlock(center, size);
    }

    public void DeleteMeshDirFromCenter(Vector3 center, Cube.TYPE type, Vector3 size, Vector3 dir)
    {
        List<QuadManager.DIRECTION> dirlist = GetDIRList(dir);
        (MeshCubeManagerTable[type] as WorldMeshCube).DeleteMeshDirFromCenter(center, size, dirlist);
    }


    public void NewMeshByDeleteCube(Vector3 center, Cube.TYPE type, Vector3 size,Vector3 dir)
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
