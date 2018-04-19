using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldMeshCube : MonoBehaviour {
    public float tileX = 0;
    public float tileY = 0;
    float tilePerc = 1 / 64f;

    //static QuadTreeMeshNode quadtreeMeshNode = null;
    int startTriangleIndex = 0;
    // Use this for initialization

    List<int> triIndexList = new List<int>();
    public string type;

    static QuadTreeIndexList[] TreeIndexArr = null;

    public void Init() {
        tilePerc = 1;
        this.GetComponent<MeshFilter>().mesh = new Mesh();
        if(TreeIndexArr == null)
        {
            TreeIndexArr = new QuadTreeIndexList[3];
            TreeIndexArr[0] = new QuadTreeIndexList();
            TreeIndexArr[1] = new QuadTreeIndexList();
            TreeIndexArr[2] = new QuadTreeIndexList();
        }
    }

    public string TexturePath
    {
        set { this.GetComponent<MeshRenderer>().material.mainTexture = (Texture)Resources.Load(value); }
    }

    void expend(MeshQuad c)
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length+1];
        //Destroy(this.gameObject.GetComponent<MeshCollider>());

        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            startTriangleIndex += combine[i].mesh.triangles.Length;
        }
        GameObject tmp = new GameObject();
        tmp.transform.localPosition = Vector3.zero;
        c.triIndex = startTriangleIndex;
        triIndexList.Add(startTriangleIndex);
        startTriangleIndex += 6;
        combine[meshFilters.Length].mesh = c.mesh;
        combine[meshFilters.Length].transform = tmp.transform.localToWorldMatrix;
        
        Destroy(tmp);

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
        transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        transform.gameObject.SetActive(true);
    }
    int TextureNum = 0;
    // Update is called once per frame

    public void addBlock(Vector3 center, Vector3 size)
    {
        List<MeshQuad> listQuad = MeshQuad.getQuadList(center, size, QuadManager.DirList, this);
        for (int i = 0; i < listQuad.Count; i++)
            AddQuad(listQuad[i]);
    }

    public void addBlock(Vector3 center, Vector3 size,List<QuadManager.DIRECTION> dirlist,float reverse = 1f)
    {
        List<MeshQuad> listQuad = MeshQuad.getQuadList(center, size, dirlist, this,reverse);
        for (int i = 0; i < listQuad.Count; i++)
            AddQuad(listQuad[i]);
    }

    public void AddReverseBlock(Vector3 center)
    {
        AddReverseBlock(center, Vector3.one);
    }

    public void AddReverseBlock(Vector3 center, Vector3 size)
    {
        List<MeshQuad> listQuad = MeshQuad.getQuadList(center, size, QuadManager.DirList, this,-1f);
        for (int i = 0; i < listQuad.Count; i++)
            AddQuad(listQuad[i]);
    }

    public void DeleteMeshDirFromCenter(Vector3 center, Vector3 size, List<QuadManager.DIRECTION> dirlist, float reverse = 1f)
    {
        List<MeshQuad> listQuadForDelete = MeshQuad.getQuadList(center, size, dirlist, this, reverse);
        for (int i = 0; i < listQuadForDelete.Count; i++)
            DeleteQuad(listQuadForDelete[i]);
    }

    QuadTreeMeshNode GetTree(MeshQuad quad)
    {
        if (quad.normal.x == 0 && quad.normal.y == 0) return TreeIndexArr[0].GetTree(quad);//front back
        if (quad.normal.z == 0 && quad.normal.y == 0) return TreeIndexArr[1].GetTree(quad);//right left
        if (quad.normal.x == 0 && quad.normal.z == 0) return TreeIndexArr[2].GetTree(quad);//up down
        return null;
    }

    void AddQuad(MeshQuad quad)
    {
        QuadTreeMeshNode quadtreeMeshNode = quad.tree;
        if (quadtreeMeshNode == null)
            quadtreeMeshNode = GetTree(quad);
        List<MeshQuad> list = quadtreeMeshNode.FindRangeList(quad.V2Start49, quad.V2End49);
        if (list.Count <= 0)
        {
            if (loopMarge(quad, quadtreeMeshNode) == true)
                return;
            quadtreeMeshNode.AddValue(new TreeAble(quad), quad.V2Start, quad.V2End);
            expend(quad);
        }
        else if (list.Count > 0)
        {
            Hashtable deleteTable = new Hashtable();
            List<int> deleteList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (quad.normal == -list[i].normal)
                {
                    list[i].parentMesh.deleteQuad(quad.V2Start, quad.V2End, list[i]);
                    if (deleteTable.ContainsKey(list[i].parentMesh) == false)
                        deleteTable.Add(list[i].parentMesh, new List<int>());
                    (deleteTable[list[i].parentMesh] as List<int>).Add(list[i].triIndex);
                }
            }
            if (deleteTable.Count > 0)
            {
                foreach (WorldMeshCube q in deleteTable.Keys)
                {
                    q.deleteTriangleList(deleteTable[q] as List<int>);
                }
            }
        }
    }
    void DeleteQuad(MeshQuad quad)
    {
        QuadTreeMeshNode quadtreeMeshNode = quad.tree;
        if (quadtreeMeshNode == null)
            quadtreeMeshNode = GetTree(quad);
        List<MeshQuad> list = quadtreeMeshNode.FindRangeList(quad.V2Start49, quad.V2End49);
        if (list.Count > 0)
        {
            Hashtable deleteTable = new Hashtable();
            List<int> deleteList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                deleteQuad(quad.V2Start, quad.V2End, list[i]);
                if (deleteTable.ContainsKey(list[i].parentMesh) == false)
                    deleteTable.Add(list[i].parentMesh, new List<int>());
                (deleteTable[list[i].parentMesh] as List<int>).Add(list[i].triIndex);
            }
            if (deleteTable.Count > 0)
            {
                foreach (WorldMeshCube q in deleteTable.Keys)
                {
                    q.deleteTriangleList(deleteTable[q] as List<int>);
                }
            }
        }
    }

    public bool deleteQuad(Vector2 s, Vector2 e, MeshQuad target)
    {
        List<QuadRangeData> splitList = target.getSplitCubeListbyRange(s,e);
        target.tree.PopValueAllParent(new TreeAble(target));
        //DestroyImmediate(target.gameObject);
        //print("splitList.Count : " + splitList.Count);
        int count = 0;
        foreach (QuadRangeData data in splitList)
        {
            if (data.deleteRange == false)
            {
                //print("Child(" + data.StartBlockRange + ">>" + data.EndBlockRange + ")(" + data.start + ">>" + data.end + ")");
                AddQuad(new MeshQuad(target.normal, data.uv, data.center, data.size, target.type, target.dir,this));
                count += 1;
            }
        }
        if (count == 0)
        {
            //loopMargeFromDeletedCube((s + e) / 2f);
        }
        return true;
    }

    bool loopMarge(MeshQuad t, QuadTreeMeshNode tree)
    {
        MeshQuad quad = t;
        MeshQuad MargeTarget = null;
        bool result = false;
        QuadTreeMeshNode quadtreeMeshNode = tree;
        if(quadtreeMeshNode == null)
            quadtreeMeshNode = GetTree(quad);
        

        while (true)
        {
            List<MeshQuad> searchList = quadtreeMeshNode.FindRangeList(quad.V2StartPlus1, quad.V2EndPlus1);
            foreach (MeshQuad c in searchList)
            {
                if (quad.Equals(c))
                    continue;
                //print("find[" + (c.GetStartVector3()) + ">>" + (c.GetEndVector3()) + "]");
                if (c.CheckCanMarge(quad))
                {
                    MargeTarget = c;
                    break;
                }
            }

            if (MargeTarget != null)
            {
                MargeTarget.ExpandQuadForMarge(quad);//ExpandQuad
                if(quad.tree != null)
                    quad.tree.PopValueAllParent(new TreeAble(quad));
                if (quad.triIndex != -1)
                {
                    List<int> tmp = new List<int>();tmp.Add(quad.triIndex);
                    deleteTriangleList(tmp);
                }
                MargeTarget.tree.updateValue(new TreeAble(MargeTarget));
                updateTriangleList(MargeTarget);
                quad = MargeTarget;
                MargeTarget = null;
                if (result == false)
                    result = true;
            }
            else if (MargeTarget == null)
                break;
        }
        return result;
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
    void updateTriangleList(List<MeshQuad> list)
    {
        for (int i = 0; i < list.Count; i++)
            updateTriangleList(list[i]);
    }
    void updateTriangleList(MeshQuad q)
    {
        Vector3[] origin = transform.GetComponent<MeshFilter>().mesh.vertices;
        Vector2[] uv = transform.GetComponent<MeshFilter>().mesh.uv;
        int index = triIndexList.FindIndex(FindInt(q.triIndex)) * 4;
        Vector3[] ver = q.vertices;
        Vector2[] uvs = q.UVs;
        origin[index + 0] = ver[0];
        origin[index + 1] = ver[1];
        origin[index + 2] = ver[2];
        origin[index + 3] = ver[3];

        uv[index + 0] = uvs[0];
        uv[index + 1] = uvs[1];
        uv[index + 2] = uvs[2];
        uv[index + 3] = uvs[3];


        transform.GetComponent<MeshFilter>().mesh.vertices = origin;
        transform.GetComponent<MeshFilter>().mesh.uv = uv;
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
            deleteIndexList.Add(triIndexList.FindIndex(FindInt(list[j])));
        deleteIndexList.Sort(delegate (int v1, int v2)
        {
            if (v1 > v2) return -1;
            if (v1 < v2) return +1;
            return 0;
        });
        for (int j = 0; j < deleteIndexList.Count; j++)
            triIndexList.RemoveAt(deleteIndexList[j]);

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
