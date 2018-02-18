using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWolrdMeshManager : MonoBehaviour {
    public float BlockCountInWidth = 64;
    public float tileX = 0;
    public float tileY = 0;

    public GameObject obj;

    Vector3 pos;
    // Use this for initialization
    void Start () {
        pos = transform.position;
        float tilePerc = 1 / BlockCountInWidth;

        Vector2 u = new Vector2(tileX, tileX + 1) * tilePerc;
        Vector2 v = new Vector2(tileY, tileY + 1) * tilePerc;

        Vector2[] blockUVs = new Vector2[24];
        blockUVs[0] =  new Vector2(u.x, v.x);
        blockUVs[1] =  new Vector2(u.y, v.x);
        blockUVs[2] =  new Vector2(u.x, v.y);
        blockUVs[3] =  new Vector2(u.y, v.y);
        blockUVs[4] =  new Vector2(u.x, v.y);
        blockUVs[5] =  new Vector2(u.y, v.y);
        blockUVs[6] =  new Vector2(u.x, v.y);
        blockUVs[7] =  new Vector2(u.y, v.y);
        blockUVs[8] =  new Vector2(u.x, v.x);
        blockUVs[9] =  new Vector2(u.y, v.x);
        blockUVs[10] = new Vector2(u.x, v.x);
        blockUVs[11] = new Vector2(u.y, v.x);
        blockUVs[12] = new Vector2(u.x, v.x);
        blockUVs[13] = new Vector2(u.x, v.y);
        blockUVs[14] = new Vector2(u.y, v.y);
        blockUVs[15] = new Vector2(u.y, v.x);
        blockUVs[16] = new Vector2(u.x, v.x);
        blockUVs[17] = new Vector2(u.x, v.y);
        blockUVs[18] = new Vector2(u.y, v.y);
        blockUVs[19] = new Vector2(u.y, v.x);
        blockUVs[20] = new Vector2(u.x, v.x);
        blockUVs[21] = new Vector2(u.x, v.y);
        blockUVs[22] = new Vector2(u.y, v.y);
        blockUVs[23] = new Vector2(u.y, v.x);

        this.GetComponent<MeshFilter>().mesh.uv = blockUVs;

    }

    void expend(GameObject block)
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        //Destroy(this.gameObject.GetComponent<MeshCollider>());

        Vector2[] oldMeshUVs = transform.GetComponent<MeshFilter>().mesh.uv;

        for(int i=0;i<meshFilters.Length;i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);

        Vector2[] newMeshUVs = new Vector2[oldMeshUVs.Length + 24];
        for (int i = 0; i < oldMeshUVs.Length; i++)
            newMeshUVs[i] = oldMeshUVs[i];

        float tilePerc = 1 / BlockCountInWidth;
        Vector2 u = new Vector2(tileX, tileX + 1) * tilePerc;
        Vector2 v = new Vector2(tileY, tileY + 1) * tilePerc;
        newMeshUVs[oldMeshUVs.Length + 0 ] = new Vector2(u.x, v.x);
        newMeshUVs[oldMeshUVs.Length + 1 ] = new Vector2(u.y, v.x);
        newMeshUVs[oldMeshUVs.Length + 2 ] = new Vector2(u.x, v.y);
        newMeshUVs[oldMeshUVs.Length + 3 ] = new Vector2(u.y, v.y);
        newMeshUVs[oldMeshUVs.Length + 4 ] = new Vector2(u.x, v.y);
        newMeshUVs[oldMeshUVs.Length + 5 ] = new Vector2(u.y, v.y);
        newMeshUVs[oldMeshUVs.Length + 6 ] = new Vector2(u.x, v.y);
        newMeshUVs[oldMeshUVs.Length + 7 ] = new Vector2(u.y, v.y);
        newMeshUVs[oldMeshUVs.Length + 8 ] = new Vector2(u.x, v.x);
        newMeshUVs[oldMeshUVs.Length + 9 ] = new Vector2(u.y, v.x);
        newMeshUVs[oldMeshUVs.Length + 10] = new Vector2(u.x, v.x);
        newMeshUVs[oldMeshUVs.Length + 11] = new Vector2(u.y, v.x);
        newMeshUVs[oldMeshUVs.Length + 12] = new Vector2(u.x, v.x);
        newMeshUVs[oldMeshUVs.Length + 13] = new Vector2(u.x, v.y);
        newMeshUVs[oldMeshUVs.Length + 14] = new Vector2(u.y, v.y);
        newMeshUVs[oldMeshUVs.Length + 15] = new Vector2(u.y, v.x);
        newMeshUVs[oldMeshUVs.Length + 16] = new Vector2(u.x, v.x);
        newMeshUVs[oldMeshUVs.Length + 17] = new Vector2(u.x, v.y);
        newMeshUVs[oldMeshUVs.Length + 18] = new Vector2(u.y, v.y);
        newMeshUVs[oldMeshUVs.Length + 19] = new Vector2(u.y, v.x);
        newMeshUVs[oldMeshUVs.Length + 20] = new Vector2(u.x, v.x);
        newMeshUVs[oldMeshUVs.Length + 21] = new Vector2(u.x, v.y);
        newMeshUVs[oldMeshUVs.Length + 22] = new Vector2(u.y, v.y);
        newMeshUVs[oldMeshUVs.Length + 23] = new Vector2(u.y, v.x);

        transform.GetComponent<MeshFilter>().mesh.uv = newMeshUVs;
        transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        transform.gameObject.SetActive(true);
        Destroy(block);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            for (int i = 0; i < 250; i++)
            {
                if (pos.x < -50)
                    pos = new Vector3(0, 0, pos.z + 1);
                pos += Vector3.left;
                GameObject block = Instantiate(obj, pos, Quaternion.identity) as GameObject;
                block.transform.SetParent(this.transform);
                expend(block);
            }
        }
	}
}
