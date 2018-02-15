using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeNode
{
    enum OctDir
    {
        DownLeftFront = 0,
        DownLeftBack = 1,
        DownRightFront = 2,
        DownRightBack = 3,
        UpLeftFront = 4,
        UpLeftBack = 5,
        UpRightFront = 6,
        UpRightBack = 7
    }
    //Down = 0, Up = 4
    //Left = 0, Right = 2
    //Front= 0, Back = 1
    public string name = "";
    OctreeNode root = null;
    OctreeNode parent;
    OctreeNode[] childNodes;
    List<Cube> list;
    Vector3 start;
    Vector3 end;
    float size;
    int level;
    public int count;

    Vector3 firstS;
    Vector3 firstE;

    public OctreeNode()
    {
        parent = null;
        childNodes = new OctreeNode[8];
        for (int i = 0; i < 8; i++) childNodes[i] = null;
        list = new List<Cube>();
        level = 0;
        count = 0;
    }

    public OctreeNode(OctreeNode Parent, Vector3 S, Vector3 E, OctreeNode root, string name)
    {
        this.parent = Parent;
        childNodes = new OctreeNode[8];
        for (int i = 0; i < 8; i++) childNodes[i] = null;
        start = S;
        end = E;
        size = E.x - S.x;
        level = parent.level + 1;
        list = new List<Cube>();
        this.root = root;
        this.name = name;
    }

    public bool AddValue(Cube t, Vector3 s, Vector3 e)
    {

        //MonoBehaviour.print("this " + this);
        //MonoBehaviour.print("list " + list);
        //MonoBehaviour.print("childNodes[0] " + childNodes[0]);
        if (level > 10) return false;

        if (count == 0)
        {
            list.Add(t);
            t.myTree = this;
            t.treeName = name;
            firstS = s;
            firstE = e;
            //MonoBehaviour.print("list.count " + list.Count + "  count : "+ count+"   level : " + level + "("+start+" >> "+end+")");
        }
        else if (count == 1)
        {
            if (childNodes[0] == null) createChildNodes();
            if (list.Count > 0 && tyrChildNodeCanAddValue(list[0], firstS, firstE) == true)
                list = new List<Cube>();
            if (tyrChildNodeCanAddValue(t, s, e) == false)
            {
                //MonoBehaviour.print("list.count " + list.Count + "  count : " + count + "   level : " + level + "(" + start + " >> " + end + ")");
                list.Add(t);
                t.myTree = this;
                t.treeName = name;
            }
        }
        else
        {
            if (tyrChildNodeCanAddValue(t, s, e) == false)
            {
                //MonoBehaviour.print("list.count " + list.Count + "  count : " + count + "   level : " + level + "(" + start + " >> " + end + ")");
                list.Add(t);
                t.myTree = this;
                t.treeName = name;
            }
        }
        count++;

        return true;
        //MonoBehaviour.print("count " + count);
    }

    public void PopValue(Cube t)
    {
        count--;
        list.Remove(t);
        //MonoBehaviour.print("count " + count + "   list.Count " + list.Count);
    }
    public void PopValueAllParent(Cube t)
    {
        PopValue(t);
        if (parent != null)
            parent.PopValueAllParent(t);
    }

    public void updateValue(Cube t)
    {
        Vector3 s = t.GetStartVector3();
        Vector3 e = t.GetEndVector3();
        //MonoBehaviour.print(name + " : ValueInMyRangeCount(s, e) = " + ValueInMyRangeCount(s, e));
        if (ValueInMyRangeCount(s, e) != 8)
        {//bigger this node. pop t and toss to parent
            PopValueAllParent(t);
            root.AddValue(t, s, e);
        }
        else if (childNodes[0] != null && checkChildNodeCanAddValue(t, s, e) == true)
        {//add to child
            //MonoBehaviour.print(name + " : move to child");
            PopValue(t);
            AddValue(t, s, e);
        }
        //else
        //MonoBehaviour.print(name + " : nope");
        //else no update
    }

    public List<Cube> FindRangeList(Vector3 s, Vector3 e)
    {

        List<Cube> result = new List<Cube>();
        //find My List
        foreach (Cube c in list)
        {
            if (c.CheckThisCubeCollideRange(s, e))
                result.Add(c);
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
    

    bool CheckCollideRange(Vector3 s, Vector3 e)
    {
        Vector3 S = start;
        Vector3 E = end;
        return !((E.x < s.x) || (e.x < S.x) || (E.y < s.y) || (e.y < S.y) || (E.z < s.z) || (e.z < S.z));
        /*if (E.x < s.x) return false;
        if (e.x < S.x) return false;
        if (E.y < s.y) return false;
        if (e.y < S.y) return false;
        if (E.z < s.z) return false;
        if (e.z < S.z) return false;
        return true;*/
    }
    public bool checkChildNodeCanAddValue(Cube t, Vector3 s, Vector3 e)
    {
        for (int i = 0; i < childNodes.Length; i++)
        {
            OctreeNode node = childNodes[i];
            if (node.ValueInMyRangeCount(s, e) == 8)
            {
                return true;
            }
        }
        return false;
    }
    public bool tyrChildNodeCanAddValue(Cube t, Vector3 s, Vector3 e)
    {
        for (int i = 0; i < childNodes.Length; i++)
        {
            OctreeNode node = childNodes[i];
            if (node.ValueInMyRangeCount(s, e) == 8)
            {
                return node.AddValue(t, s, e);
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
    List<Vector3> GetVectexList(Vector3 s, Vector3 e)
    {
        List<Vector3> list = new List<Vector3>();
        list.Add(new Vector3(s.x, s.y, s.z));
        list.Add(new Vector3(s.x, s.y, e.z));
        list.Add(new Vector3(e.x, s.y, s.z));
        list.Add(new Vector3(e.x, s.y, e.z));
        list.Add(new Vector3(s.x, e.y, s.z));
        list.Add(new Vector3(s.x, e.y, e.z));
        list.Add(new Vector3(e.x, e.y, s.z));
        list.Add(new Vector3(e.x, e.y, e.z));
        return list;
    }


    public bool Vec3InMyRange(Vector3 p)
    {
        return
        start.x <= p.x &&
        start.y <= p.y &&
        start.z <= p.z &&
        end.x > p.x &&
        end.y > p.y &&
        end.z > p.z;
    }

    public void SetRoot()
    {
        start = Vector3.one * -512;//dir == 0
        end = Vector3.one * 512;//dir == 7
        size = 1024;
        root = this;
        name = "root";
        createChildNodes();
    }

    void createChildNodes()
    {
        //MonoBehaviour.print("createChildNodes " + level);
        if (level > 9) return;
        //MonoBehaviour.print("createChildNodes " + level + " length:" + childNodes.Length);
        for (int i = 0; i < 8; i++)
        {
            Vector3 s = start;
            Vector3 e = end;
            Vector3 c = (s + e) / 2f;
            switch ((OctDir)i)
            {
                case OctDir.DownLeftFront: e.y = c.y; e.x = c.x; e.z = c.z; break;
                case OctDir.DownLeftBack: e.y = c.y; e.x = c.x; s.z = c.z; break;
                case OctDir.DownRightFront: e.y = c.y; s.x = c.x; e.z = c.z; break;
                case OctDir.DownRightBack: e.y = c.y; s.x = c.x; s.z = c.z; break;
                case OctDir.UpLeftFront: s.y = c.y; e.x = c.x; e.z = c.z; break;
                case OctDir.UpLeftBack: s.y = c.y; e.x = c.x; s.z = c.z; break;
                case OctDir.UpRightFront: s.y = c.y; s.x = c.x; e.z = c.z; break;
                case OctDir.UpRightBack: s.y = c.y; s.x = c.x; s.z = c.z; break;
            }
            childNodes[i] = new OctreeNode(this, s, e, root, name + i);
        }
    }
}
