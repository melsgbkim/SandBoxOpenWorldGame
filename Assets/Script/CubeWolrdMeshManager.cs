using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWolrdMeshManager : MonoBehaviour {
    public float BlockCountInWidth = 64;
    public float tileX = 0;
    public float tileY = 0;
    float tilePerc = 1 / 64f;

    public GameObject obj;

    Vector3 pos;
    OctreeMeshNode octreeMeshNode;
    int startTriangleIndex = 0;
    // Use this for initialization
    void Start () {
        pos = Vector3.zero;
        this.GetComponent<MeshFilter>().mesh.uv = getDefaultUV(new Vector2(tileX, tileY));
        octreeMeshNode = new OctreeMeshNode();
        octreeMeshNode.SetRoot();
    }

    void expend(GameObject block)
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        //Destroy(this.gameObject.GetComponent<MeshCollider>());

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
        transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        transform.gameObject.SetActive(true);
        Destroy(block);
    }

    void expend(MeshCube c)
    {
        List<QuadManager.DIRECTION> list = c.getActiveDirList();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (pos == Vector3.zero)
            meshFilters = new MeshFilter[0];
        CombineInstance[] combine = new CombineInstance[meshFilters.Length+ list.Count];
        //Destroy(this.gameObject.GetComponent<MeshCollider>());

        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            startTriangleIndex += combine[i].mesh.triangles.Length;
        }
        List<Mesh> meshList = c.getMeshList(list, new Vector2(tileX, tileY) * tilePerc, Vector2.one * tilePerc);
        for (int i=0;i< meshList.Count; i++)
        {
            GameObject tmp = new GameObject();
            tmp.transform.localPosition = c.center;
            combine[i + meshFilters.Length].mesh = meshList[i];
            combine[i + meshFilters.Length].transform = tmp.transform.localToWorldMatrix;
            c.triIndexArr[(int)list[i]] = startTriangleIndex;
            MeshCube.triIndexList.Add(startTriangleIndex);
            startTriangleIndex += 6;
            Destroy(tmp);
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
        transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        transform.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
        int max = 0;
        if (Input.GetKeyDown(KeyCode.F6))
            max = 1;
        if (Input.GetKeyDown(KeyCode.F7))
            max = 20;

        for (int i = 0; i < max; i++)
        {
            if (pos.x < -50)
                pos = new Vector3(0, 0, pos.z + 1);
            addBlock(pos);
            pos += Vector3.left;
        }
    }

    void addBlock(Vector3 center)
    {
        addBlock(center, Vector3.one);
    }

    void addBlock(Vector3 center, Vector3 size)
    {
        Vector3 s = center - size * 0.5f;
        Vector3 e = center + size * 0.5f;

        List<MeshCube> list = octreeMeshNode.FindRangeList(s, e);
        if(list.Count <= 0)
        {
            List<MeshCube> listAround = octreeMeshNode.FindRangeList(s - Vector3.one, e + Vector3.one);
            MeshCube c = new MeshCube(center, new Vector2(tileX, tileY) * tilePerc, Vector2.one * tilePerc);
            List<int> triangleList = new List<int>();
            if (listAround.Count > 0)
            {
                for (int i = 0; i < listAround.Count; i++)
                {
                    QuadManager.DIRECTION dir = c.tryDeleteMeshNearMeshCube(listAround[i]);
                    if(dir != QuadManager.DIRECTION.max)
                        triangleList.Add(listAround[i].triIndexArr[(int)dir]);
                }
            }
            if(triangleList.Count > 0)
                deleteTriangleList(triangleList);
            octreeMeshNode.AddValue(new OctreeAble(c), s, e);
            expend(c);
        }
    }

    private int findTriangleTarget = 0;
    private bool findTriangleIndex(int index)
    {
        return index == findTriangleTarget;
    }
    private Predicate<int> FindInt(int val)
    {
        findTriangleTarget = val;
        return findTriangleIndex;
    }
    void deleteTriangleList(List<int> list)
    {
        CombineInstance[] combine = new CombineInstance[1];
        //Destroy(this.gameObject.GetComponent<MeshCollider>());

        Mesh origin = transform.GetComponent<MeshFilter>().mesh;
        
        transform.gameObject.SetActive(false);
        Predicate<int> predicate = findTriangleIndex;
        List<int> deleteIndexList = new List<int>();
        for(int j=0;j<list.Count;j++)
            deleteIndexList.Add(MeshCube.triIndexList.FindIndex(FindInt(list[j])));
        deleteIndexList.Sort(delegate (int v1, int v2)
        {
            if (v1 > v2) return -1;
            if (v1 < v2) return +1;
            return 0;
        });
        for (int j = 0; j < deleteIndexList.Count; j++)
            MeshCube.triIndexList.RemoveAt(deleteIndexList[j]);

        Mesh newMesh = new Mesh();
        List<Vector3> originVer = new List<Vector3>(origin.vertices);
        for (int j = 0; j < deleteIndexList.Count; j++)
            originVer.RemoveRange(deleteIndexList[j] * 4, 4);
        newMesh.vertices = originVer.ToArray(); originVer = null;
        

        List<Vector3> originNor = new List<Vector3>(origin.normals);
        for (int j = 0; j < deleteIndexList.Count; j++)
            originNor.RemoveRange(deleteIndexList[j] * 4, 4);
        newMesh.normals = originNor.ToArray(); originNor = null;
        

        List<Vector2> originUV = new List<Vector2>(origin.uv);
        for (int j = 0; j < deleteIndexList.Count; j++)
            originUV.RemoveRange(deleteIndexList[j] * 4, 4);
        newMesh.uv = originUV.ToArray(); originUV = null;
        

        List<int> originTri = new List<int>(origin.triangles);
        for (int j = 0; j < deleteIndexList.Count; j++)
            originTri.RemoveRange(deleteIndexList[j] * 6, 6);

        int s = deleteIndexList[deleteIndexList.Count - 1];
        int e = originTri.Count / 6;

        for (int i = s; i < e; i++)
        {
            int index4 = i * 4;
            int index6 = i * 6;
            originTri[index6 + 0] = index4;
            originTri[index6 + 1] = index4 + 2;
            originTri[index6 + 2] = index4 + 1;
            originTri[index6 + 3] = index4 + 2;
            originTri[index6 + 4] = index4 + 3;
            originTri[index6 + 5] = index4 + 1;
        }

        newMesh.triangles = originTri.ToArray(); originTri = null;
        //transform.GetComponent<MeshFilter>().mesh = origin;
        
        
        
        
        
        

        combine[0].mesh = newMesh;
        combine[0].transform = transform.localToWorldMatrix;
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
        transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        transform.gameObject.SetActive(true);
    }

    void newMeshExpend()
    {

    }





    Vector2[] getDefaultUV(Vector2 tile)
    {
        Vector2[] newMeshUVs = new Vector2[24];
        
        Vector2 u = new Vector2(tile.x, tile.x + 1) * tilePerc;
        Vector2 v = new Vector2(tile.y, tile.y + 1) * tilePerc;
        newMeshUVs[0] = new Vector2(u.x, v.x);
        newMeshUVs[1] = new Vector2(u.y, v.x);
        newMeshUVs[2] = new Vector2(u.x, v.y);
        newMeshUVs[3] = new Vector2(u.y, v.y);
        newMeshUVs[4] = new Vector2(u.x, v.y);
        newMeshUVs[5] = new Vector2(u.y, v.y);
        newMeshUVs[6] = new Vector2(u.x, v.y);
        newMeshUVs[7] = new Vector2(u.y, v.y);
        newMeshUVs[8] = new Vector2(u.x, v.x);
        newMeshUVs[9] = new Vector2(u.y, v.x);
        newMeshUVs[10] = new Vector2(u.x, v.x);
        newMeshUVs[11] = new Vector2(u.y, v.x);
        newMeshUVs[12] = new Vector2(u.x, v.x);
        newMeshUVs[13] = new Vector2(u.x, v.y);
        newMeshUVs[14] = new Vector2(u.y, v.y);
        newMeshUVs[15] = new Vector2(u.y, v.x);
        newMeshUVs[16] = new Vector2(u.x, v.x);
        newMeshUVs[17] = new Vector2(u.x, v.y);
        newMeshUVs[18] = new Vector2(u.y, v.y);
        newMeshUVs[19] = new Vector2(u.y, v.x);
        newMeshUVs[20] = new Vector2(u.x, v.x);
        newMeshUVs[21] = new Vector2(u.x, v.y);
        newMeshUVs[22] = new Vector2(u.y, v.y);
        newMeshUVs[23] = new Vector2(u.y, v.x);

        return newMeshUVs;
    }
}
