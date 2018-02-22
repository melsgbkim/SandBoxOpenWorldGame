using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TreeAble
{
    public Object obj;
    public MeshQuad mesh;
    public TreeAble(Object obj) { this.obj = obj; mesh = null; }
    public TreeAble(MeshQuad mesh) { this.mesh = mesh; obj = null; }
}

public class OctreeBasic
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
    protected OctreeBasic root = null;
    protected OctreeBasic parent;
    protected OctreeBasic[] childNodes;

    protected Vector3 start;
    protected Vector3 end;
    protected float size;
    protected int level;
    public int count;
    protected List<TreeAble> list;

    protected Vector3 firstS;
    protected Vector3 firstE;

    public OctreeBasic()
    {
        init();
    }
    public OctreeBasic(OctreeBasic Parent, Vector3 S, Vector3 E, OctreeBasic root, string name)
    {
        init();
        this.parent = Parent;

        start = S;
        end = E;
        size = E.x - S.x;
        level = parent.level + 1;
        this.root = root;
        this.name = name;
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

    
    public void PopValueAllParent(TreeAble t)
    {
        PopValue(t);
        if (parent != null)
            parent.PopValueAllParent(t);
    }

    protected bool CheckCollideRange(Vector3 s, Vector3 e)
    {
        Vector3 S = start;
        Vector3 E = end;
        return !((E.x < s.x) || (e.x < S.x) || (E.y < s.y) || (e.y < S.y) || (E.z < s.z) || (e.z < S.z));
    }

    protected List<Vector3> GetVectexList(Vector3 s, Vector3 e)
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
    protected void createChildNodes()
    {
        if (level > 9) return;
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
            childNodes[i] = createChild(this, s, e, root, name + i);
        }
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

    public virtual void init()
    {
        parent = null;
        childNodes = new OctreeBasic[8];
        for (int i = 0; i < 8; i++) childNodes[i] = null;
        list = new List<TreeAble>();
        level = 0;
        count = 0;
    }
    public virtual OctreeBasic createChild(OctreeBasic Parent, Vector3 S, Vector3 E, OctreeBasic root, string name)
    {
        return new OctreeBasic(Parent, S, E, root, name);
    }
    public virtual void PopValue(TreeAble t)
    {
        count--;
        list.Remove(t);
    }
    public virtual bool AddValue(TreeAble t, Vector3 s, Vector3 e) { return false; }
    public virtual void updateValue(TreeAble t){}
    public virtual bool checkChildNodeCanAddValue(TreeAble t, Vector3 s, Vector3 e){ return false; }
    public virtual bool tyrChildNodeCanAddValue(TreeAble t, Vector3 s, Vector3 e){ return false; }
}

