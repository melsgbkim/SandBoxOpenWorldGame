using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshQuad
{
    Vector3[] vertices;
    Vector3[] normal;
    Vector2[] uv;
    Vector3 center;
    int[] tri;
    public int triIndex;
    public MeshQuad(Vector3[] vertices,Vector3[] normal,Vector2[] uv,Vector3 center, int[] tri)
    {
        this.vertices = new Vector3[vertices.Length];
        this.normal = new Vector3[normal.Length];
        this.uv = new Vector2[uv.Length];
        this.tri = new int[tri.Length];
        int i = 0; while (i < vertices.Length) { this.vertices[i] = vertices[i]; }
        int j = 0; while (j < normal.Length) { this.normal[j] = normal[j]; }
        int k = 0; while (k < uv.Length) { this.uv[k] = uv[k]; }
        int l = 0; while (l < tri.Length) { this.tri[l] = tri[l]; }
        this.center = center;

    }

}

public class MeshCube
{
    public OctreeMeshNode tree;
    public Vector3 center;
    public bool[] quadArr = new bool[(int)(QuadManager.DIRECTION.max)];
    public int[] triIndexArr = new int[(int)(QuadManager.DIRECTION.max)];
    public static List<int> triIndexList = new List<int>();
    Vector2 uvStart;
    Vector2 uvSize;
    public MeshCube(Vector3 center, Vector2 uvStart, Vector2 uvSize)
    {
        for (int i = 0; i < (int)QuadManager.DirList.Count; i++)
        {
            triIndexArr[i] = 0;
            quadArr[i] = true;
        }
        this.center = center;
        this.uvStart = uvStart;
        this.uvSize = uvSize;
    }

    public QuadManager.DIRECTION tryDeleteMeshNearMeshCube(MeshCube other)
    {
        QuadManager.DIRECTION result = QuadManager.DIRECTION.max;
        if (((other.center - center) - Vector3.back).sqrMagnitude < 0.1f)
        {
            quadArr[(int)QuadManager.DIRECTION.front] = false;
            result = QuadManager.DIRECTION.back;
        }

        if (((other.center - center) - Vector3.forward).sqrMagnitude < 0.1f)
        {
            quadArr[(int)QuadManager.DIRECTION.back] = false;
            result = QuadManager.DIRECTION.front;
        }

        if (((other.center - center) - Vector3.right).sqrMagnitude < 0.1f)
        {
            quadArr[(int)QuadManager.DIRECTION.right] = false;
            result = QuadManager.DIRECTION.left;
        }

        if (((other.center - center) - Vector3.left).sqrMagnitude < 0.1f)
        {
            quadArr[(int)QuadManager.DIRECTION.left] = false;
            result = QuadManager.DIRECTION.right;
        }

        if (((other.center - center) - Vector3.up).sqrMagnitude < 0.1f)
        {
            quadArr[(int)QuadManager.DIRECTION.top] = false;
            result = QuadManager.DIRECTION.bottom;
        }

        if (((other.center - center) - Vector3.down).sqrMagnitude < 0.1f)
        {
            quadArr[(int)QuadManager.DIRECTION.bottom] = false;
            result = QuadManager.DIRECTION.top;
        }
        return result;
    }

    public int getActiveDirCount()
    {
        int result = 0;
        for (int i = 0; i < (int)QuadManager.DIRECTION.max; i++)
        {
            if (quadArr[i])
                result++;
        }
        return result;
    }

    public List<QuadManager.DIRECTION> getActiveDirList()
    {
        List<QuadManager.DIRECTION> result = new List<QuadManager.DIRECTION>();
        for (int i = 0; i < (int)QuadManager.DirList.Count; i++)
        {
            if (quadArr[i])
                result.Add(QuadManager.DirList[i]);
        }
        return result;
    }

    public List<Mesh> getMeshList(List<QuadManager.DIRECTION> dirList, Vector2 uvStart, Vector2 uvSize)
    {
        List<Mesh> result = new List<Mesh>();

        int index = 0;
        if (dirList[index] == QuadManager.DIRECTION.front)
        {
            index++;
            Vector3[] vertices = new Vector3[4];
            Vector3[] normal = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] tri = new int[6];

            vertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
            vertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
            vertices[2] = new Vector3(-0.5f, 0.5f, -0.5f);
            vertices[3] = new Vector3(0.5f, 0.5f, -0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = -Vector3.forward;
            tri[0] = 0; tri[1] = 2; tri[2] = 1;
            tri[3] = 2; tri[4] = 3; tri[5] = 1;
            uv[0] = new Vector2(0, 0) + uvStart;
            uv[1] = new Vector2(uvSize.x, 0) + uvStart;
            uv[2] = new Vector2(0, uvSize.y) + uvStart;
            uv[3] = new Vector2(uvSize.x, uvSize.y) + uvStart;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normal;
            mesh.uv = uv;
            mesh.triangles = tri;
            result.Add(mesh);
        }
        if (dirList[index] == QuadManager.DIRECTION.back)
        {
            index++;
            Vector3[] vertices = new Vector3[4];
            Vector3[] normal = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] tri = new int[6];

            vertices[0] = new Vector3(+0.5f, -0.5f, +0.5f);
            vertices[1] = new Vector3(-0.5f, -0.5f, +0.5f);
            vertices[2] = new Vector3(+0.5f, +0.5f, +0.5f);
            vertices[3] = new Vector3(-0.5f, +0.5f, +0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.forward;
            tri[0] = 0; tri[1] = 2; tri[2] = 1;
            tri[3] = 2; tri[4] = 3; tri[5] = 1;
            uv[0] = new Vector2(0, 0) + uvStart;
            uv[1] = new Vector2(uvSize.x, 0) + uvStart;
            uv[2] = new Vector2(0, uvSize.y) + uvStart;
            uv[3] = new Vector2(uvSize.x, uvSize.y) + uvStart;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normal;
            mesh.uv = uv;
            mesh.triangles = tri;
            result.Add(mesh);
        }
        if (dirList[index] == QuadManager.DIRECTION.right)
        {
            index++;
            Vector3[] vertices = new Vector3[4];
            Vector3[] normal = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] tri = new int[6];

            vertices[0] = new Vector3(+0.5f, -0.5f, -0.5f);
            vertices[1] = new Vector3(+0.5f, -0.5f, +0.5f);
            vertices[2] = new Vector3(+0.5f, +0.5f, -0.5f);
            vertices[3] = new Vector3(+0.5f, +0.5f, +0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.right;
            tri[0] = 0; tri[1] = 2; tri[2] = 1;
            tri[3] = 2; tri[4] = 3; tri[5] = 1;
            uv[0] = new Vector2(0, 0) + uvStart;
            uv[1] = new Vector2(uvSize.x, 0) + uvStart;
            uv[2] = new Vector2(0, uvSize.y) + uvStart;
            uv[3] = new Vector2(uvSize.x, uvSize.y) + uvStart;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normal;
            mesh.uv = uv;
            mesh.triangles = tri;
            result.Add(mesh);
        }
        if (dirList[index] == QuadManager.DIRECTION.left)
        {
            index++;
            Vector3[] vertices = new Vector3[4];
            Vector3[] normal = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] tri = new int[6];

            vertices[0] = new Vector3(-0.5f, -0.5f, +0.5f);
            vertices[1] = new Vector3(-0.5f, -0.5f, -0.5f);
            vertices[2] = new Vector3(-0.5f, +0.5f, +0.5f);
            vertices[3] = new Vector3(-0.5f, +0.5f, -0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.left;
            tri[0] = 0; tri[1] = 2; tri[2] = 1;
            tri[3] = 2; tri[4] = 3; tri[5] = 1;
            uv[0] = new Vector2(0, 0) + uvStart;
            uv[1] = new Vector2(uvSize.x, 0) + uvStart;
            uv[2] = new Vector2(0, uvSize.y) + uvStart;
            uv[3] = new Vector2(uvSize.x, uvSize.y) + uvStart;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normal;
            mesh.uv = uv;
            mesh.triangles = tri;
            result.Add(mesh);
        }
        if (dirList[index] == QuadManager.DIRECTION.top)
        {
            index++;
            Vector3[] vertices = new Vector3[4];
            Vector3[] normal = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] tri = new int[6];

            vertices[0] = new Vector3(-0.5f, +0.5f, -0.5f);
            vertices[1] = new Vector3(+0.5f, +0.5f, -0.5f);
            vertices[2] = new Vector3(-0.5f, +0.5f, +0.5f);
            vertices[3] = new Vector3(+0.5f, +0.5f, +0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.up;
            tri[0] = 0; tri[1] = 2; tri[2] = 1;
            tri[3] = 2; tri[4] = 3; tri[5] = 1;
            uv[0] = new Vector2(0, 0) + uvStart;
            uv[1] = new Vector2(uvSize.x, 0) + uvStart;
            uv[2] = new Vector2(0, uvSize.y) + uvStart;
            uv[3] = new Vector2(uvSize.x, uvSize.y) + uvStart;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normal;
            mesh.uv = uv;
            mesh.triangles = tri;
            result.Add(mesh);
        }
        if (dirList[index] == QuadManager.DIRECTION.bottom)
        {
            index++;
            Vector3[] vertices = new Vector3[4];
            Vector3[] normal = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] tri = new int[6];

            vertices[0] = new Vector3(-0.5f, -0.5f, +0.5f);
            vertices[1] = new Vector3(+0.5f, -0.5f, +0.5f);
            vertices[2] = new Vector3(-0.5f, -0.5f, -0.5f);
            vertices[3] = new Vector3(+0.5f, -0.5f, -0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.down;
            tri[0] = 0; tri[1] = 2; tri[2] = 1;
            tri[3] = 2; tri[4] = 3; tri[5] = 1;
            uv[0] = new Vector2(0, 0) + uvStart;
            uv[1] = new Vector2(uvSize.x, 0) + uvStart;
            uv[2] = new Vector2(0, uvSize.y) + uvStart;
            uv[3] = new Vector2(uvSize.x, uvSize.y) + uvStart;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normal;
            mesh.uv = uv;
            mesh.triangles = tri;
            result.Add(mesh);
        }

        return result;
    }


    public Vector3 GetStartVector3() { return center - Vector3.one / 2f; }
    public Vector3 GetEndVector3() { return center + Vector3.one / 2f; }
}


public class OctreeMeshNode : OctreeBasic
{
    public OctreeMeshNode()
    {

    }

    public OctreeMeshNode(OctreeMeshNode Parent, Vector3 S, Vector3 E, OctreeMeshNode root, string name)
    {
    }

    public override OctreeBasic createChild(OctreeBasic Parent, Vector3 S, Vector3 E, OctreeBasic root, string name)
    {
        return new OctreeMeshNode(Parent as OctreeMeshNode, S, E, root as OctreeMeshNode, name);
    }

    public override bool AddValue(OctreeAble T, Vector3 s, Vector3 e)
    {
        MeshCube t = T.mesh;
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
                list = new List<OctreeAble>();
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

    public override void updateValue(OctreeAble T)
    {
        MeshCube t = T.mesh;
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

    public List<MeshCube> FindRangeList(Vector3 s, Vector3 e)
    {
        List<MeshCube> result = new List<MeshCube>();
        //find My List
        foreach (OctreeAble c in list)
        {
            if (InRange(s,e, c.mesh.center))
                result.Add(c.mesh);
        }
        //find ChildNodes List
        foreach (OctreeMeshNode n in childNodes)
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
            OctreeMeshNode node = childNodes[i] as OctreeMeshNode;
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
            OctreeMeshNode node = childNodes[i] as OctreeMeshNode;
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

    bool InRange(Vector3 s, Vector3 e, Vector3 center)
    {
        return s.x <= center.x &&
            s.y <= center.y &&
            s.z <= center.z &&
            e.x >= center.x &&
            e.y >= center.y &&
            e.z >= center.z;
    }
}
