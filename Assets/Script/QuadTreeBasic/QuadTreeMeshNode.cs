using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeMeshNode : QuadTreeBasic
{
    public QuadTreeMeshNode()
    {

    }

    public QuadTreeMeshNode(QuadTreeMeshNode Parent, Vector2 S, Vector2 E, QuadTreeMeshNode root, string name)
    {
    }

    public override QuadTreeBasic createChild(QuadTreeBasic Parent, Vector2 S, Vector2 E, QuadTreeBasic root, string name)
    {
        return new QuadTreeMeshNode(Parent as QuadTreeMeshNode, S, E, root as QuadTreeMeshNode, name);
    }

    public override bool AddValue(TreeAble T, Vector2 s, Vector2 e)
    {
        MeshQuad t = T.mesh;
        if (t == null) return false;
        if (level > 10) return false;

        if (count == 0)
        {
            list.Add(T);
            t.tree = this;
            firstS = s;
            firstE = e;
        }
        else if (count == 1)
        {
            if (childNodes[0] == null) createChildNodes();
            if (list.Count > 0 && tyrChildNodeCanAddValue(list[0], firstS, firstE) == true)
                list = new List<TreeAble>();
            if (tyrChildNodeCanAddValue(T, s, e) == false)
            {
                //MonoBehaviour.print("list.count " + list.Count + "  count : " + count + "   level : " + level + "(" + start + " >> " + end + ")");
                list.Add(T);
                t.tree = this;
            }
        }
        else
        {
            if (tyrChildNodeCanAddValue(T, s, e) == false)
            {
                //MonoBehaviour.print("list.count " + list.Count + "  count : " + count + "   level : " + level + "(" + start + " >> " + end + ")");
                list.Add(T);
                t.tree = this;
            }
        }
        count++;

        return true;
        //MonoBehaviour.print("count " + count);
    }

    public override void updateValue(TreeAble T)
    {
        MeshQuad t = T.mesh;
        Vector2 s = t.V2Start;
        Vector2 e = t.V2End;
        if (ValueInMyRangeCount(s, e) != (int)QuadDir.max)
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

    public List<MeshQuad> FindRangeList(Vector2 s, Vector2 e)
    {
        List<MeshQuad> result = new List<MeshQuad>();
        //find My List
        foreach (TreeAble c in list)
        {
            if (c.mesh.CheckThisQuadCollideRange(s, e))
                result.Add(c.mesh);
        }
        //find ChildNodes List
        foreach (QuadTreeMeshNode n in childNodes)
        {
            if (n != null && n.CheckCollideRange(s, e) == true && n.count > 0)
            {
                result.AddRange(n.FindRangeList(s, e));
            }
        }
        //MonoBehaviour.print("FindRangeList level " + level + "  result "+ result.Count);
        return result;
    }

    public override bool checkChildNodeCanAddValue(TreeAble T, Vector2 s, Vector2 e)
    {
        for (int i = 0; i < childNodes.Length; i++)
        {
            QuadTreeMeshNode node = childNodes[i] as QuadTreeMeshNode;
            if (node.ValueInMyRangeCount(s, e) == (int)QuadDir.max)
            {
                return true;
            }
        }
        return false;
    }
    public override bool tyrChildNodeCanAddValue(TreeAble T, Vector2 s, Vector2 e)
    {
        for (int i = 0; i < childNodes.Length; i++)
        {
            QuadTreeMeshNode node = childNodes[i] as QuadTreeMeshNode;
            if (node.ValueInMyRangeCount(s, e) == (int)QuadDir.max)
            {
                return node.AddValue(T, s, e);
            }
        }
        return false;
    }
    public int ValueInMyRangeCount(Vector2 s, Vector2 e)
    {
        List<Vector2> list = GetVectexList(s, e);
        int result = 0;
        foreach (Vector2 v in list)
        {
            if (Vec2InMyRange(v))
                result++;
        }
        return result;
    }

    bool InRange(Vector2 s, Vector2 e, Vector2 center)
    {
        return s.x <= center.x &&
            s.y <= center.y &&
            e.x >= center.x &&
            e.y >= center.y;
    }
}
