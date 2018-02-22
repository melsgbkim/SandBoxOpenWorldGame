using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeBasic
{
    protected enum QuadDir
    {
        DownLeft = 0,
        DownRight= 1,
        UpLeft= 2,
        UpRight= 3,
        max = 4
    }
    //2,3
    //0,1
    public string name = "";
    protected QuadTreeBasic root = null;
    protected QuadTreeBasic parent;
    protected QuadTreeBasic[] childNodes;

    public float index;
    protected Vector2 start;
    protected Vector2 end;
    protected float size;
    protected int level;
    public int count;
    protected List<TreeAble> list;

    protected Vector2 firstS;
    protected Vector2 firstE;

    public QuadTreeBasic()
    {
        init();
    }
    public QuadTreeBasic(QuadTreeBasic Parent, Vector2 S, Vector2 E, QuadTreeBasic root, string name)
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
        start = Vector2.one * -512;//dir == 0
        end = Vector2.one * 512;//dir == 7
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

    protected bool CheckCollideRange(Vector2 s, Vector2 e)
    {
        Vector2 S = start;
        Vector2 E = end;
        return !((E.x < s.x) || (e.x < S.x) || (E.y < s.y) || (e.y < S.y));
    }

    protected List<Vector2> GetVectexList(Vector2 s, Vector2 e)
    {
        List<Vector2> list = new List<Vector2>();
        list.Add(new Vector3(s.x, s.y));
        list.Add(new Vector3(e.x, s.y));
        list.Add(new Vector3(s.x, e.y));
        list.Add(new Vector3(e.x, e.y));
        return list;
    }
    protected void createChildNodes()
    {
        if (level > 9) return;
        for (int i = 0; i < (int)QuadDir.max; i++)
        {
            Vector2 s = start;
            Vector2 e = end;
            Vector2 c = (s + e) * 0.5f;
            switch ((QuadDir)i)
            {
                case QuadDir.DownLeft:  e.y = c.y; e.x = c.x; break;
                case QuadDir.DownRight: e.y = c.y; s.x = c.x; break;
                case QuadDir.UpLeft:    s.y = c.y; e.x = c.x; break;
                case QuadDir.UpRight:   s.y = c.y; s.x = c.x; break;
            }
            childNodes[i] = createChild(this, s, e, root, name + i);
        }
    }

    public bool Vec2InMyRange(Vector2 p)
    {
        return
        start.x <= p.x &&
        start.y <= p.y &&
        end.x > p.x &&
        end.y > p.y;
    }

    public virtual void init()
    {
        parent = null;
        childNodes = new QuadTreeBasic[(int)QuadDir.max];
        for (int i = 0; i < (int)QuadDir.max; i++) childNodes[i] = null;
        list = new List<TreeAble>();
        level = 0;
        count = 0;
    }
    public virtual QuadTreeBasic createChild(QuadTreeBasic Parent, Vector2 S, Vector2 E, QuadTreeBasic root, string name)
    {
        return new QuadTreeBasic(Parent, S, E, root, name);
    }
    public virtual void PopValue(TreeAble t)
    {
        count--;
        list.Remove(t);
    }
    public virtual bool AddValue(TreeAble t, Vector2 s, Vector2 e) { return false; }
    public virtual void updateValue(TreeAble t) { }
    public virtual bool checkChildNodeCanAddValue(TreeAble t, Vector2 s, Vector2 e) { return false; }
    public virtual bool tyrChildNodeCanAddValue(TreeAble t, Vector2 s, Vector2 e) { return false; }
}

