using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshQuad
{
    public Vector3 normal;
    public Vector2 uvSize;
    public Vector3 center;
    public Vector3 size;
    public int triIndex;
    public QuadManager.DIRECTION dir;
    public WorldMeshCube parentMesh;
    public string type;

    public QuadTreeMeshNode tree;

    public Vector3 VStart { get { return center - size * 0.5f; } }
    public Vector3 VEnd { get { return center + size * 0.5f; } }

    public Vector3 VStart49 { get { return center - size * 0.49f; } }
    public Vector3 VEnd49 { get { return center + size * 0.49f; } }

    public Vector3 VStartPlus1 { get { return VStart - Vector3.ClampMagnitude(size, 0.5f); } }
    public Vector3 VEndPlus1 { get { return VEnd + Vector3.ClampMagnitude(size, 0.5f); } }

    public Vector2 V2Start { get { return V2Center - V2size * 0.5f; } }
    public Vector2 V2End { get { return V2Center + V2size * 0.5f; } }

    public Vector2 V2Start49 { get { return V2Center - V2size * 0.49f; } }
    public Vector2 V2End49 { get { return V2Center + V2size * 0.49f; } }

    public Vector2 V2StartPlus1 { get { return V2Start - Vector2.ClampMagnitude(V2size, 0.5f); } }
    public Vector2 V2EndPlus1 { get { return V2End + Vector2.ClampMagnitude(V2size, 0.5f); } }

    public Vector3 V2toV3Center(Vector2 v)
    {
        if (normal.x == 0 && normal.y == 0) { return new Vector3(v.x, v.y, center.z); }
        if (normal.x == 0 && normal.z == 0) { return new Vector3(v.x, center.y, v.y); }
        if (normal.z == 0 && normal.y == 0) { return new Vector3(center.x, v.y, v.x); }
        return Vector3.zero;
    }

    public Vector3 V2toV3Size(Vector2 v)
    {
        if (normal.x == 0 && normal.y == 0) { return new Vector3(v.x, v.y, 0); }
        if (normal.x == 0 && normal.z == 0) { return new Vector3(v.x, 0, v.y); }
        if (normal.z == 0 && normal.y == 0) { return new Vector3(0, v.y, v.x); }
        return Vector3.zero;
    }

    public Vector2 V3toV2(Vector3 v)
    {
        if (normal.x == 0 && normal.y == 0) { return new Vector2(v.x, v.y); }
        if (normal.x == 0 && normal.z == 0) { return new Vector2(v.x, v.z); }
        if (normal.z == 0 && normal.y == 0) { return new Vector2(v.z, v.y); }
        return Vector2.zero;
    }

    public Vector2 V2Center
    {
        get
        {
            if (normal.x == 0 && normal.y == 0) return new Vector2(center.x, center.y);
            if (normal.x == 0 && normal.z == 0) return new Vector2(center.x, center.z);
            if (normal.z == 0 && normal.y == 0) return new Vector2(center.z, center.y);
            return Vector2.zero;
        }
        set
        {
            if (normal.x == 0 && normal.y == 0) {center.x = value.x; center.y = value.y;}
            if (normal.x == 0 && normal.z == 0) {center.x = value.x; center.z = value.y;}
            if (normal.z == 0 && normal.y == 0) {center.z = value.x; center.y = value.y;}
        }
    }

    public Vector2 V2size
    {
        get
        {
            if (normal.x == 0 && normal.y == 0) return new Vector2(size.x, size.y);
            if (normal.x == 0 && normal.z == 0) return new Vector2(size.x, size.z);
            if (normal.z == 0 && normal.y == 0) return new Vector2(size.z, size.y);
            return Vector2.zero;
        }
        set
        {
            if (normal.x == 0 && normal.y == 0) { size.x = value.x; size.y = value.y; }
            if (normal.x == 0 && normal.z == 0) { size.x = value.x; size.z = value.y; }
            if (normal.z == 0 && normal.y == 0) { size.z = value.x; size.y = value.y; }
        }
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        MeshQuad c = obj as MeshQuad;
        if (c == null) return false;
        else return Equals(c);
    }
    public bool Equals(MeshQuad other)
    {
        if (other == null) return false;
        return (this.center == other.center && this.size == other.size);
    }

    public MeshQuad(Vector3 normal, Vector2 uvSize, Vector3 center, Vector3 size, string type,QuadManager.DIRECTION dir,WorldMeshCube parent)
    {
        this.normal = normal;
        this.uvSize = uvSize;
        this.center = center;
        this.size = size;
        this.type = type;
        this.dir = dir;
        this.parentMesh = parent;

        this.triIndex = -1;
        tree = null;
    }

    public Mesh mesh
    {
        get
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = new Vector3[4] { normal, normal, normal, normal };
            mesh.uv = UVs;
            mesh.triangles = new int[6] { 0, 2, 1, 2, 3, 1 };
            return mesh;
        }
    }

    public Vector2[] UVs
    {
        get
        {
            return new Vector2[4] 
            {
                new Vector2(0, 0),
                new Vector2(uvSize.x, 0),
                new Vector2(0, uvSize.y),
                new Vector2(uvSize.x, uvSize.y)
            };
        }
    }
    public Vector3[] vertices
    {
        get
        {
            Vector3[] result = new Vector3[4];
            Quaternion rot = Quaternion.identity;
            Vector2 tmpSize = new Vector2(size.x, size.y);
            if (normal == Vector3.back)     {rot = Quaternion.identity;         tmpSize = new Vector2(size.x, size.y);}
            if (normal == Vector3.forward)  {rot = Quaternion.Euler(0, 180, 0); tmpSize = new Vector2(size.x, size.y);}
            if (normal == Vector3.right)    {rot = Quaternion.Euler(0, -90, 0);  tmpSize = new Vector2(size.z, size.y);}
            if (normal == Vector3.left)     {rot = Quaternion.Euler(0, 90, 0); tmpSize = new Vector2(size.z, size.y);}
            if (normal == Vector3.up)       {rot = Quaternion.Euler(90, 0, 0);  tmpSize = new Vector2(size.x, size.z);}
            if (normal == Vector3.down)     {rot = Quaternion.Euler(-90, 0, 0); tmpSize = new Vector2(size.x, size.z);}
            result[0] = (center + rot * new Vector3(-0.5f * tmpSize.x, -0.5f * tmpSize.y, 0)) / 3f;
            result[1] = (center + rot * new Vector3(+0.5f * tmpSize.x, -0.5f * tmpSize.y, 0)) / 3f;
            result[2] = (center + rot * new Vector3(-0.5f * tmpSize.x, +0.5f * tmpSize.y, 0)) / 3f;
            result[3] = (center + rot * new Vector3(+0.5f * tmpSize.x, +0.5f * tmpSize.y, 0)) / 3f;
            return result;
        }
    }


    public bool CheckOtherCenterInMyRange(Vector2 c)
    {
        Vector2 S = V2Start;
        Vector2 E = V2End;
        return !((E.x < c.x) || (c.x < S.x) || (E.y < c.y) || (c.y < S.y));
    }

    public bool CheckThisQuadCollideRange(Vector2 s, Vector2 e)
    {
        Vector2 S = V2Start;
        Vector2 E = V2End;
        return !((E.x < s.x) || (e.x < S.x) || (E.y < s.y) || (e.y < S.y));
    }

    /*public bool CheckCanMarge(MeshQuad q)
    {
        if (q.normal != this.normal)
            return false;
        if (q.type != this.type)
            return false;
        Vector2 c = q.V2Center;
        Vector2 s = q.V2size * 0.5f;

        Vector2 C = V2Center;
        Vector2 S = V2size * 0.5f;
            return (
            (c + new Vector2(0, s.y) == C - new Vector2(0, S.y) || (c - new Vector2(0, s.y) == C + new Vector2(0, S.y)) && s.x == S.x) ||
            (c + new Vector2(s.x, 0) == C - new Vector2(S.x, 0) || (c - new Vector2(s.x, 0) == C + new Vector2(S.x, 0)) && s.y == S.y)
            );
    }*/
    public bool CheckCanMarge(MeshQuad other)
    {
        if (other.type != this.type) return false;
        if (other.normal != this.normal) return false;

        bool SameX = Mathf.Abs(V2Center.x - other.V2Center.x) < 0.01f && Mathf.Abs(V2size.x - other.V2size.x) < 0.01f;
        bool SameY = Mathf.Abs(V2Center.y - other.V2Center.y) < 0.01f && Mathf.Abs(V2size.y - other.V2size.y) < 0.01f;
        if (SameX && !SameY) return (Mathf.Abs(V2End.y - other.V2Start.y)<0.01f || Mathf.Abs(V2Start.y - other.V2End.y)<0.01f);
        if (!SameX && SameY) return (Mathf.Abs(V2End.x - other.V2Start.x)<0.01f || Mathf.Abs(V2Start.x - other.V2End.x)<0.01f);
        if (SameX && SameY) MonoBehaviour.print("Error : CheckCanMarge AllTrue");
        //All True = bug
        return false;
    }

    public void ExpandQuadForMarge(MeshQuad other)
    {
        Vector2 c = other.V2Center;
        Vector2 s = other.V2size * 0.5f;

        Vector2 C = V2Center;
        Vector2 S = V2size * 0.5f;

        Vector2 center = Vector2.zero; 
        Vector2 Size = Vector2.zero; 
        if (c + new Vector2(0, s.y) == C - new Vector2(0, S.y) || (c - new Vector2(0, s.y) == C + new Vector2(0, S.y)) && s.x == S.x)
        {
                 if (c + new Vector2(0, s.y) == C - new Vector2(0, S.y)) V2Center = new Vector2(C.x, ((c.y - s.y) + (C.y + S.y)) * 0.5f);
            else if (c - new Vector2(0, s.y) == C + new Vector2(0, S.y)) V2Center = new Vector2(C.x, ((c.y + s.y) + (C.y - S.y)) * 0.5f);
            V2size = V2size + new Vector2(0, other.V2size.y);
        }
        else if (c + new Vector2(s.x, 0) == C - new Vector2(S.x, 0) || (c - new Vector2(s.x, 0) == C + new Vector2(S.x, 0)) && s.y == S.y)
        {
                 if (c + new Vector2(s.x, 0) == C - new Vector2(S.x, 0)) V2Center = new Vector2(((c.x - s.x) + (C.x + S.x)) * 0.5f, C.y);
            else if (c - new Vector2(s.x, 0) == C + new Vector2(S.x, 0)) V2Center = new Vector2(((c.x + s.x) + (C.x - S.x)) * 0.5f, C.y);
            V2size = V2size + new Vector2(other.V2size.x, 0);
        }

        uvSize = V2size;

        //MonoBehaviour.print("EXPAND FOR MARGE");
    }


    public static List<MeshQuad> getQuadList(Vector3 center,Vector3 size, List<QuadManager.DIRECTION> dirList, WorldMeshCube p,float reverse = 1f)
    {
        List<MeshQuad> result = new List<MeshQuad>();

        int index = 0;
        if (index < dirList.Count && dirList[index] == QuadManager.DIRECTION.front)
        {
           result.Add(new MeshQuad(Vector3.back * reverse,       
                new Vector2(size.x, size.y),
                center + new Vector3(0, 0, -size.z) * 0.5f,
                new Vector3(size.x, size.y, 0), p.type, dirList[index++], p));
        }
        if (index < dirList.Count && dirList[index] == QuadManager.DIRECTION.back)
        {
            result.Add(new MeshQuad(Vector3.forward * reverse,    
                new Vector2(size.x, size.y),
                center + new Vector3(0, 0, +size.z) * 0.5f,
                new Vector3(size.x, size.y, 0), p.type, dirList[index++], p));
        }
        if (index < dirList.Count && dirList[index] == QuadManager.DIRECTION.right)
        {
            result.Add(new MeshQuad(Vector3.right * reverse,      
                new Vector2(size.z, size.y),
                center + new Vector3(size.x, 0, 0) * 0.5f,
                new Vector3(0, size.y, size.z), p.type, dirList[index++], p));
        }
        if (index < dirList.Count && dirList[index] == QuadManager.DIRECTION.left)
        {
            result.Add(new MeshQuad(Vector3.left * reverse,       
                new Vector2(size.z, size.y),
                center + new Vector3(-size.x, 0, 0) * 0.5f,
                new Vector3(0, size.y, size.z), p.type, dirList[index++], p));
        }
        if (index < dirList.Count && dirList[index] == QuadManager.DIRECTION.top)
        {
            result.Add(new MeshQuad(Vector3.up * reverse,         
                new Vector2(size.x, size.z),
                center + new Vector3(0, size.y, 0) * 0.5f,
                new Vector3(size.x, 0, size.z), p.type, dirList[index++], p));
        }
        if (index < dirList.Count && dirList[index] == QuadManager.DIRECTION.bottom)
        {
            result.Add(new MeshQuad(Vector3.down * reverse,       
                new Vector2(size.x, size.z),
                center + new Vector3(0,-size.y, 0) * 0.5f, 
                new Vector3(size.x, 0, size.z), p.type, dirList[index++], p));
        }
        //MeshQuad(Vector3 normal, Vector2 uvSize, Vector3 center, Vector3 size, WorldMeshCube parent)
        return result;
    }
    public List<QuadRangeData> getSplitCubeListbyRange(Vector3 s, Vector3 e)
    {
        return getSplitCubeListbyRange(V3toV2(s), V3toV2(e));
    }
    public List<QuadRangeData> getSplitCubeListbyRange(Vector2 s, Vector2 e)
    {
        List<QuadRangeData> result = new List<QuadRangeData>();
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        Vector3 S = V2Start;
        Vector3 E = V2End;
        /*
        if (s.x < S.x) x.Add(S.x); else x.Add(s.x);
        if (e.x > E.x) x.Add(E.x); else x.Add(e.x);
        if (s.y < S.y) y.Add(S.y); else y.Add(s.y);
        if (e.y > E.y) y.Add(E.y); else y.Add(e.y);
        if (s.z < S.z) z.Add(S.z); else z.Add(s.z);
        if (e.z > E.z) z.Add(E.z); else z.Add(e.z);
        */
        if(S.x < s.x) x.Add(s.x);else x.Add(S.x);
        if(E.x > e.x) x.Add(e.x);else x.Add(E.x);
        if(S.y < s.y) y.Add(s.y);else y.Add(S.y);
        if(E.y > e.y) y.Add(e.y);else y.Add(E.y);
        x.Add(S.x);
        x.Add(E.x);
        y.Add(S.y);
        y.Add(E.y);

        x.Sort(sortFloatASC);
        y.Sort(sortFloatASC);
        for (int iy = 0; iy < 3; iy++)
        {
            for (int ix = 0; ix < 3; ix++)
            {
                if (x[ix + 1] - x[ix] >= 1f && y[iy + 1] - y[iy] >= 1f)
                {
                    Vector2 insertS = new Vector2(x[ix], y[iy]);
                    Vector2 insertE = new Vector2(x[ix + 1], y[iy + 1]);
                    //print("x(" + ix + ")  y(" + iy + ")  z(" + iz + ")"+"Split (" + insertS + ">>" + insertE + ")  "+ (x[ix] == s.x && y[iy] == s.y && z[iz] == s.z && x[ix + 1] == e.x && y[iy + 1] == e.y && z[iz + 1] == e.z));
                    result.Add(new QuadRangeData(this, V2toV3Center(insertS), V2toV3Center(insertE), insertE - insertS, (x[ix] == s.x && y[iy] == s.y &&x[ix + 1] == e.x && y[iy + 1] == e.y)));
                }
            }
        }
        return result;
    }
    int sortFloatASC(float a, float b)
    {
        if (a > b) return 1;
        if (a == b) return 0;
        return -1;
    }
}

public class QuadRangeData
{
    MeshQuad parent = null;
    public bool deleteRange = false;
    public QuadRangeData(MeshQuad p, Vector3 s, Vector3 e, Vector2 uv ,bool delete)
    {
        this.uv = uv;
        parent = p;
        start = s;
        end = e;
        deleteRange = delete;
    }
    public Vector2 uv;
    public Vector3 start;
    public Vector3 end;
    public Vector3 center
    {
        get { return (start + end) / 2f; }
    }
    public Vector3 size
    {
        get { return end - start; }
    }
    public Vector3 StartBlockRange
    {
        get { return start + Vector3.one / 2f; }
    }
    public Vector3 EndBlockRange
    {
        get { return end - Vector3.one / 2f; }
    }
}