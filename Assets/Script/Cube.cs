using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cube : MonoBehaviour {
    public Transform[] QuadList = new Transform[6];
    
    public Vector3 Center;
    public Vector3 Size;
    public OctreeNode myTree = null;
    public string treeName = "";
    public float CheckedTime = 0f;

    bool initialized = false;

    public enum TYPE
    {
        Air,
        Grass,
        Dirt
    };
    public TYPE type = TYPE.Air;

    public void init()
    {
        transform.localScale = Vector3.one / 3f;
        Center = transform.localPosition * 3f;
        Size = Vector3.one;
    }

    // Use this for initialization
    void Start () {
        if (initialized == false)
        {
            transform.localScale = Vector3.one / 3f;
            Size = Vector3.one;
            Center = transform.localPosition * 3f;
            initialized = true;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Cube c = obj as Cube;
        if (c == null) return false;
        else return Equals(c);
    }
    public bool Equals(Cube other)
    {
        if (other == null) return false;
        return (this.Center == other.Center && this.Size == other.Size);
    }

    public Vector3 GetStartVector3()
    {
        return Center - Size / 2f;
    }
    public Vector3 GetEndVector3()
    {
        return Center + Size / 2f;
    }

    public bool CheckThisCubeCollideRange(Vector3 s, Vector3 e)
    { 
        Vector3 S = GetStartVector3();
        Vector3 E = GetEndVector3();
        bool result = !((E.x < s.x) || (e.x < S.x) || (E.y < s.y) || (e.y < S.y) || (E.z < s.z) || (e.z < S.z));
        //print("[this S:" + S + "][this E:" + E + "]");
        //print("[OtherS:" + s + "][OtherE:" + e + "]");
        //print("result : " + (!((E.x < s.x) || (e.x < S.x) || (E.y < s.y) || (e.y < S.y) || (E.z < s.z) || (e.z < S.z))));
        return result;
    }

    public int GetCollisionRangeCountOtherCube(Cube c)
    {
        int result = 0;
        Vector3 s = GetStartVector3();
        Vector3 e = GetEndVector3();
        Vector3 S = c.GetStartVector3();
        Vector3 E = c.GetEndVector3();

        if (s.x < S.x) s.x = S.x;
        if (s.y < S.y) s.y = S.y;
        if (s.z < S.z) s.z = S.z;
        if (e.x > E.x) e.x = E.x;
        if (e.y > E.y) e.y = E.y;
        if (e.z > E.z) e.z = E.z;

        return Mathf.RoundToInt((e.x - s.x) * (e.y - s.y) * (e.z - s.z));
    }
    


    // Update is called once per frame

    public bool CheckCanMarge(Cube other)
    {
        if (other.type != this.type) return false;

        bool SameX = Center.x == other.Center.x && Size.x == other.Size.x;
        bool SameY = Center.y == other.Center.y && Size.y == other.Size.y;
        bool SameZ = Center.z == other.Center.z && Size.z == other.Size.z;
        if ( SameX &&  SameY && !SameZ) return ((GetEndVector3().z == other.GetStartVector3().z) || (GetStartVector3().z == other.GetEndVector3().z));
        if ( SameX && !SameY &&  SameZ) return ((GetEndVector3().y == other.GetStartVector3().y) || (GetStartVector3().y == other.GetEndVector3().y));
        if (!SameX &&  SameY && SameZ) return  ((GetEndVector3().x == other.GetStartVector3().x) || (GetStartVector3().x == other.GetEndVector3().x));
        if (SameX && SameY && SameZ) print("Error : CheckCanMarge AllTrue");
        //All True = bug
        return false;
    }

    public void ExpandCubeForMarge(Cube other)
    {
        bool SameX = Center.x == other.Center.x && Size.x == other.Size.x;
        bool SameY = Center.y == other.Center.y && Size.y == other.Size.y;
        bool SameZ = Center.z == other.Center.z && Size.z == other.Size.z;
        if (SameX == false)
        {
            float min = GetMinPoint(Center.x, Size.x, other.Center.x, other.Size.x);
            float max = GetMaxPoint(Center.x, Size.x, other.Center.x, other.Size.x);
            //print("min " + min + " max " + max);
            Center.x = (min + max) / 2f;
            Size.x = (max - min);
        }

        else if (SameY == false)
        {
            float min = GetMinPoint(Center.y, Size.y, other.Center.y, other.Size.y);
            float max = GetMaxPoint(Center.y, Size.y, other.Center.y, other.Size.y);
            //print("min " + min + " max " + max);
            Center.y = (min + max) / 2f;
            Size.y = (max - min);
        }

        else if (SameZ == false)
        {
            float min = GetMinPoint(Center.z, Size.z, other.Center.z, other.Size.z);
            float max = GetMaxPoint(Center.z, Size.z, other.Center.z, other.Size.z);
            //print("min " + min + " max " + max);
            Center.z = (min + max) / 2f;
            Size.z = (max - min);
        }

        transform.localPosition = Center / 3f;
        transform.localScale = Size / 3f;
        //setQuadSize();
        if (initialized == false)initialized = true;
        //print("localPosition " + transform.localPosition + "  localScale " + transform.localScale);
    }

    public List<CubeRangeData> getSplitCubeListbyRange(Vector3 s, Vector3 e)
    {
        List<CubeRangeData> result = new List<CubeRangeData>();
        List<float> x = new List<float>();
        List<float> y = new List<float>();
        List<float> z = new List<float>();
        Vector3 S = GetStartVector3();
        Vector3 E = GetEndVector3();
        /*
        if (s.x < S.x) x.Add(S.x); else x.Add(s.x);
        if (e.x > E.x) x.Add(E.x); else x.Add(e.x);
        if (s.y < S.y) y.Add(S.y); else y.Add(s.y);
        if (e.y > E.y) y.Add(E.y); else y.Add(e.y);
        if (s.z < S.z) z.Add(S.z); else z.Add(s.z);
        if (e.z > E.z) z.Add(E.z); else z.Add(e.z);
        */
        x.Add(s.x);
        x.Add(e.x);
        y.Add(s.y);
        y.Add(e.y);
        z.Add(s.z);
        z.Add(e.z);
        x.Add(S.x);
        x.Add(E.x);
        y.Add(S.y);
        y.Add(E.y);
        z.Add(S.z);
        z.Add(E.z);

        x.Sort(sortFloatASC);
        y.Sort(sortFloatASC);        
        z.Sort(sortFloatASC);
        if (false)
        {
            print("[" + x[0] + "," + x[1] + "," + x[2] + "," + x[3] + "]");
            print("[" + y[0] + "," + y[1] + "," + y[2] + "," + y[3] + "]");
            print("[" + z[0] + "," + z[1] + "," + z[2] + "," + z[3] + "]");
        }
        for (int iy = 0; iy < 3; iy++)
        {
            for (int iz = 0; iz < 3; iz++)
            {
                for (int ix = 0; ix < 3; ix++)
                {
                    if(x[ix + 1] - x[ix] >= 1f && y[iy + 1] - y[iy] >= 1f && z[iz + 1] - z[iz] >= 1f)
                    {
                        Vector3 insertS = new Vector3(x[ix], y[iy], z[iz]);
                        Vector3 insertE = new Vector3(x[ix + 1], y[iy + 1], z[iz + 1]);
                        //print("x(" + ix + ")  y(" + iy + ")  z(" + iz + ")"+"Split (" + insertS + ">>" + insertE + ")  "+ (x[ix] == s.x && y[iy] == s.y && z[iz] == s.z && x[ix + 1] == e.x && y[iy + 1] == e.y && z[iz + 1] == e.z));
                        result.Add(new CubeRangeData(this,insertS, insertE, (x[ix] == s.x && y[iy] == s.y && z[iz] == s.z && x[ix + 1] == e.x && y[iy + 1] == e.y && z[iz + 1] == e.z)));
                    }
                    else if(false)
                        print("x(" + ix + ")  y(" + iy + ")  z(" + iz + ") Failed"+ (x[ix] == s.x && y[iy] == s.y && z[iz] == s.z && x[ix + 1] == e.x && y[iy + 1] == e.y && z[iz + 1] == e.z)+
                            "  x:" + ((x[ix + 1] - x[ix])) +
                            "  y:" + ((y[iy + 1] - y[iy])) +
                            "  z:" + ((z[iz + 1] - z[iz])));

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

    public void setSize(Vector3 s)
    {
        Size = s;
        transform.localScale = Size / 3f;
        initialized = true;
    }

    public void setQuadSize()
    {
        QuadList[(int)(QuadManager.DIRECTION.front)].GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(Size.x, Size.y));
        QuadList[(int)(QuadManager.DIRECTION.back)].GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(Size.x, Size.y));
        QuadList[(int)(QuadManager.DIRECTION.right)].GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(Size.z, Size.y));
        QuadList[(int)(QuadManager.DIRECTION.left)].GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(Size.z, Size.y));
        QuadList[(int)(QuadManager.DIRECTION.top)].GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(Size.x, Size.z));
        QuadList[(int)(QuadManager.DIRECTION.bottom)].GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(Size.x, Size.z));
    }

    public Vector3 getPositionFromCollisionPos(Vector3 p)
    {
        Vector3 s = GetStartVector3() + Vector3.one / 2f;
        Vector3 e = GetEndVector3() - Vector3.one / 2f;
        if (p.x <= s.x) p.x = Mathf.Round(s.x);
        if (p.y <= s.y) p.y = Mathf.Round(s.y);
        if (p.z <= s.z) p.z = Mathf.Round(s.z);
        if (p.x >= e.x) p.x = Mathf.Round(e.x);
        if (p.y >= e.y) p.y = Mathf.Round(e.y);
        if (p.z >= e.z) p.z = Mathf.Round(e.z);
        return p;
    }

    
    float GetMinPoint(float c1, float s1, float c2, float s2)
    {
        List<float> list = getPointListForSort(c1, s1, c2, s2);
        list.Sort(delegate (float a, float b)
        {
            if (a > b) return 1;
            else if (a < b) return -1;
            return 0;
        });
        return list[0];
    }
    float GetMaxPoint(float c1, float s1, float c2, float s2)
    {
        List<float> list = getPointListForSort(c1, s1, c2, s2);
        list.Sort(delegate (float a, float b)
        {
            if (a > b) return -1;
            else if (a < b) return 1;
            return 0;
        });
        return list[0];
    }
    List<float> getPointListForSort(float c1, float s1, float c2, float s2)
    {
        List<float> list = new List<float>();
        list.Add(c1 - s1 / 2f);
        list.Add(c1 + s1 / 2f);
        list.Add(c2 - s2 / 2f);
        list.Add(c2 + s2 / 2f);
        return list;
    }

}


public class CubeRangeData
{
    Cube parent = null;
    public bool deleteRange = false;
    public CubeRangeData(Cube p, Vector3 s, Vector3 e, bool delete)
    {
        parent = p;
        start = s;
        end = e;
        deleteRange = delete;
    }
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

    public bool Vec3InThisRange(Vector3 v)
    {
        return
            start.x <= v.x &&
            start.y <= v.y &&
            start.z <= v.z &&
            end.x >= v.x &&
            end.y >= v.y &&
            end.z >= v.z;
    }
}