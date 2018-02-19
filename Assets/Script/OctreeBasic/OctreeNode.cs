using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeNode : OctreeBasic
{
    public OctreeNode()
    {
        
    }

    public OctreeNode(OctreeNode Parent, Vector3 S, Vector3 E, OctreeNode root, string name)
    {
    }

    public override OctreeBasic createChild(OctreeBasic Parent, Vector3 S, Vector3 E, OctreeBasic root, string name)
    {
        return new OctreeNode(Parent as OctreeNode, S, E, root as OctreeNode, name);
    }

    public override bool AddValue(OctreeAble T, Vector3 s, Vector3 e)
    {
        Cube t = T.obj as Cube;
        if (t == null) return false;
        if (level > 10) return false;

        if (count == 0)
        {
            list.Add(T);
            t.myTree = this;
            t.treeName = name;
            firstS = s;
            firstE = e;
        }
        else if (count == 1)
        {
            if (childNodes[0] == null) createChildNodes();
            if (list.Count > 0 && tyrChildNodeCanAddValue(list[0], firstS, firstE) == true)
                list = new List<OctreeAble>();
            if (tyrChildNodeCanAddValue(T, s, e) == false)
            {
                //MonoBehaviour.print("list.count " + list.Count + "  count : " + count + "   level : " + level + "(" + start + " >> " + end + ")");
                list.Add(T);
                t.myTree = this;
                t.treeName = name;
            }
        }
        else
        {
            if (tyrChildNodeCanAddValue(T, s, e) == false)
            {
                //MonoBehaviour.print("list.count " + list.Count + "  count : " + count + "   level : " + level + "(" + start + " >> " + end + ")");
                list.Add(T);
                t.myTree = this;
                t.treeName = name;
            }
        }
        count++;

        return true;
        //MonoBehaviour.print("count " + count);
    }

    public override void updateValue(OctreeAble T)
    {
        Cube t = T.obj as Cube;
        Vector3 s = t.GetStartVector3();
        Vector3 e = t.GetEndVector3();
        if (ValueInMyRangeCount(s, e) != 8)
        {//bigger this node. pop t and toss to parent
            PopValueAllParent(T);
            root.AddValue(T, s, e);
        }
        else if (childNodes[0] != null && checkChildNodeCanAddValue(T, s, e) == true)
        {//add to child
            //MonoBehaviour.print(name + " : move to child");
            PopValue(T);
            AddValue(T, s, e);
        }
        //else
        //MonoBehaviour.print(name + " : nope");
        //else no update
    }

    public List<Cube> FindRangeList(Vector3 s, Vector3 e)
    {
        List<Cube> result = new List<Cube>();
        //find My List
        foreach (OctreeAble c in list)
        {
            if ((c.obj as Cube).CheckThisCubeCollideRange(s, e))
                result.Add((c.obj as Cube));
        }
        //find ChildNodes List
        foreach (OctreeNode n in childNodes)
        {
            if (n != null && n.CheckCollideRange(s, e) == true && n.count > 0)
            {
                result.AddRange(n.FindRangeList(s, e));
            }
        }
        //MonoBehaviour.print("FindRangeList level " + level + "  result "+ result.Count);
        return result;
    }

    public override bool checkChildNodeCanAddValue(OctreeAble T, Vector3 s, Vector3 e)
    {
        for (int i = 0; i < childNodes.Length; i++)
        {
            OctreeNode node = childNodes[i] as OctreeNode;
            if (node.ValueInMyRangeCount(s, e) == 8)
            {
                return true;
            }
        }
        return false;
    }
    public override bool tyrChildNodeCanAddValue(OctreeAble T, Vector3 s, Vector3 e)
    {
        for (int i = 0; i < childNodes.Length; i++)
        {
            OctreeNode node = childNodes[i] as OctreeNode;
            if (node.ValueInMyRangeCount(s, e) == 8)
            {
                return node.AddValue(T, s, e);
            }
        }
        return false;
    }
    public int ValueInMyRangeCount(Vector3 s, Vector3 e)
    {
        List<Vector3> list = GetVectexList(s, e);
        int result = 0;
        foreach (Vector3 v in list)
        {
            if (Vec3InMyRange(v))
                result++;
        }
        return result;
    }
}
