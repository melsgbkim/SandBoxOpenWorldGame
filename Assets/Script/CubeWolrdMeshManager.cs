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
        CombineInstance[] combine = new CombineInstance[meshFilters.Length+ list.Count];
        //Destroy(this.gameObject.GetComponent<MeshCollider>());

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        List<Mesh> meshList = c.getMeshList(list, new Vector2(tileX, tileY) * tilePerc, Vector2.one * tilePerc);
        for (int i=0;i< meshList.Count; i++)
        {
            GameObject tmp = new GameObject();
            tmp.transform.localPosition = c.center;
            combine[i + meshFilters.Length].mesh = meshList[i];
            combine[i + meshFilters.Length].transform = tmp.transform.localToWorldMatrix;
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
        if (Input.GetKey(KeyCode.F6))
        {
            for (int i = 0; i < 5; i++)
            {
                if (pos.x < -50)
                    pos = new Vector3(0, 0, pos.z + 1);
                pos += Vector3.left;
                addBlock(pos);
                //GameObject block = Instantiate(obj, pos, Quaternion.identity) as GameObject;
                //block.transform.SetParent(this.transform);
                //block.GetComponent<MeshFilter>().mesh.uv = getDefaultUV(new Vector2(tileX, tileY));
                //expend(block);
            }
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
                    //triangleList.Add(listAround[i].quadArr[(int)dir].triIndex);
                }
            }
            octreeMeshNode.AddValue(new OctreeAble(c), s, e);
            expend(c);
        }
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
