using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeIndexList
{
    public Vector3 normal;
    public Hashtable ListByIndex;
    public QuadTreeIndexList()
    {
        ListByIndex = new Hashtable();
    }

    public QuadTreeMeshNode GetTree(MeshQuad q)
    {
        float index = getIndex(q);

        if(ListByIndex.ContainsKey(index) == false)
        {
            QuadTreeMeshNode tmp = new QuadTreeMeshNode();
            tmp.SetRoot();
            ListByIndex.Add(index, tmp);
        }
        return ListByIndex[index] as QuadTreeMeshNode;
    }

    float getIndex(MeshQuad q)
    {
        if (q.normal.x == 0 && q.normal.y == 0) return q.center.z;
        if (q.normal.x == 0 && q.normal.z == 0) return q.center.y;
        if (q.normal.z == 0 && q.normal.y == 0) return q.center.x;
        return 0f;
    }
}