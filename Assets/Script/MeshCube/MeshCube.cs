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
    public WorldMeshCube parentMesh;
    public Cube.TYPE type;

    public OctreeMeshNode tree;

    public Vector3 VStart { get { return center - size * 0.5f; } }
    public Vector3 VEnd { get { return center + size * 0.5f; } }

    public Vector3 VStart49 { get { return center - size * 0.49f; } }
    public Vector3 VEnd49 { get { return center + size * 0.49f; } }

    public Vector3 VStartPlus1 { get { return VStart - Vector3.ClampMagnitude(size, 0.5f); } }
    public Vector3 VEndPlus1 { get { return VEnd + Vector3.ClampMagnitude(size, 0.5f); } }
    public MeshQuad(Vector3 normal, Vector2 uvSize, Vector3 center, Vector3 size, Cube.TYPE type,WorldMeshCube parent)
    {
        this.normal = normal;
        this.uvSize = uvSize;
        this.center = center;
        this.size = size;
        this.type = type;
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
            mesh.uv = new Vector2[4] { new Vector2(0, 0), new Vector2(uvSize.x, 0), new Vector2(0, uvSize.y), new Vector2(uvSize.x, uvSize.y) } ;
            mesh.triangles = new int[6] { 0, 2, 1, 2, 3, 1 };
            return mesh;
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
            result[0] = center + rot * new Vector3(-0.5f * tmpSize.x, -0.5f * tmpSize.y, 0);
            result[1] = center + rot * new Vector3(+0.5f * tmpSize.x, -0.5f * tmpSize.y, 0);
            result[2] = center + rot * new Vector3(-0.5f * tmpSize.x, +0.5f * tmpSize.y, 0);
            result[3] = center + rot * new Vector3(+0.5f * tmpSize.x, +0.5f * tmpSize.y, 0);
            return result;
        }
    }


    public bool CheckOtherCenterInMyRange(Vector3 c)
    {
        Vector3 S = VStart;
        Vector3 E = VEnd;
        return !((E.x < c.x) || (c.x < S.x) || (E.y < c.y) || (c.y < S.y) || (E.z < c.z) || (c.z < S.z));
    }

    public bool CheckThisQuadCollideRange(Vector3 s, Vector3 e)
    {
        Vector3 S = VStart;
        Vector3 E = VEnd;
        return !((E.x < s.x) || (e.x < S.x) || (E.y < s.y) || (e.y < S.y) || (E.z < s.z) || (e.z < S.z));
    }

    public bool CheckCanMarge(MeshQuad q)
    {
        MonoBehaviour.print("CHECK CAN MARGE");
        return false;
    }

    public void ExpandQuadForMarge(MeshQuad other)
    {
        MonoBehaviour.print("EXPAND FOR MARGE");
    }


    public static List<MeshQuad> getQuadList(Vector3 center,Vector3 size, List<QuadManager.DIRECTION> dirList, WorldMeshCube p)
    {
        List<MeshQuad> result = new List<MeshQuad>();

        int index = 0;
        if (dirList[index] == QuadManager.DIRECTION.front)
        {
            index++;
            result.Add(new MeshQuad(Vector3.back,       
                new Vector2(size.x, size.y),
                center + new Vector3(0, 0, -size.z) * 0.5f,
                new Vector3(size.x, size.y, 0), p.type, p));
        }
        if (dirList[index] == QuadManager.DIRECTION.back)
        {
            index++;
            result.Add(new MeshQuad(Vector3.forward,    
                new Vector2(size.x, size.y),
                center + new Vector3(0, 0, +size.z) * 0.5f,
                new Vector3(size.x, size.y, 0), p.type, p));
        }
        if (dirList[index] == QuadManager.DIRECTION.right)
        {
            index++;
            result.Add(new MeshQuad(Vector3.right,      
                new Vector2(size.z, size.y),
                center + new Vector3(size.x, 0, 0) * 0.5f,
                new Vector3(0, size.y, size.z), p.type, p));
        }
        if (dirList[index] == QuadManager.DIRECTION.left)
        {
            index++;
            result.Add(new MeshQuad(Vector3.left,       
                new Vector2(size.z, size.y),
                center + new Vector3(-size.x, 0, 0) * 0.5f,
                new Vector3(0, size.y, size.z), p.type, p));
        }
        if (dirList[index] == QuadManager.DIRECTION.top)
        {
            index++;
            result.Add(new MeshQuad(Vector3.up,         
                new Vector2(size.x, size.z),
                center + new Vector3(0, size.y, 0) * 0.5f,
                new Vector3(size.x, 0, size.z), p.type, p));
        }
        if (dirList[index] == QuadManager.DIRECTION.bottom)
        {
            index++;
            result.Add(new MeshQuad(Vector3.down,       
                new Vector2(size.x, size.z),
                center + new Vector3(0,-size.y, 0) * 0.5f, 
                new Vector3(size.x, 0, size.z), p.type, p));
        }
        //MeshQuad(Vector3 normal, Vector2 uvSize, Vector3 center, Vector3 size, WorldMeshCube parent)
        return result;
    }
}
 /*
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
            Vector3[] vertices, normal; Vector2[] uv; int[] tri;
            SetDefault(out vertices, out normal, out uv, out tri);

            vertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
            vertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
            vertices[2] = new Vector3(-0.5f, 0.5f, -0.5f);
            vertices[3] = new Vector3(0.5f, 0.5f, -0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = -Vector3.forward;

            result.Add(CreateMesh(vertices, normal, uv, tri));
        }
        if (dirList[index] == QuadManager.DIRECTION.back)
        {
            index++;
            Vector3[] vertices, normal; Vector2[] uv; int[] tri;
            SetDefault(out vertices, out normal, out uv, out tri);

            vertices[0] = new Vector3(+0.5f, -0.5f, +0.5f);
            vertices[1] = new Vector3(-0.5f, -0.5f, +0.5f);
            vertices[2] = new Vector3(+0.5f, +0.5f, +0.5f);
            vertices[3] = new Vector3(-0.5f, +0.5f, +0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.forward;

            result.Add(CreateMesh(vertices, normal, uv, tri));
        }
        if (dirList[index] == QuadManager.DIRECTION.right)
        {
            index++;
            Vector3[] vertices, normal; Vector2[] uv; int[] tri;
            SetDefault(out vertices, out normal, out uv, out tri);

            vertices[0] = new Vector3(+0.5f, -0.5f, -0.5f);
            vertices[1] = new Vector3(+0.5f, -0.5f, +0.5f);
            vertices[2] = new Vector3(+0.5f, +0.5f, -0.5f);
            vertices[3] = new Vector3(+0.5f, +0.5f, +0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.right;

            result.Add(CreateMesh(vertices, normal, uv, tri));
        }
        if (dirList[index] == QuadManager.DIRECTION.left)
        {
            index++;
            Vector3[] vertices, normal; Vector2[] uv; int[] tri;
            SetDefault(out vertices, out normal, out uv, out tri);

            vertices[0] = new Vector3(-0.5f, -0.5f, +0.5f);
            vertices[1] = new Vector3(-0.5f, -0.5f, -0.5f);
            vertices[2] = new Vector3(-0.5f, +0.5f, +0.5f);
            vertices[3] = new Vector3(-0.5f, +0.5f, -0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.left;

            result.Add(CreateMesh(vertices, normal, uv, tri));
        }
        if (dirList[index] == QuadManager.DIRECTION.top)
        {
            index++;
            Vector3[] vertices, normal; Vector2[] uv; int[] tri;
            SetDefault(out vertices, out normal, out uv, out tri);

            vertices[0] = new Vector3(-0.5f, +0.5f, -0.5f);
            vertices[1] = new Vector3(+0.5f, +0.5f, -0.5f);
            vertices[2] = new Vector3(-0.5f, +0.5f, +0.5f);
            vertices[3] = new Vector3(+0.5f, +0.5f, +0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.up;

            result.Add(CreateMesh(vertices, normal, uv, tri));
        }
        if (dirList[index] == QuadManager.DIRECTION.bottom)
        {
            index++;
            Vector3[] vertices, normal; Vector2[] uv; int[] tri;
            SetDefault(out vertices, out normal, out uv, out tri);

            vertices[0] = new Vector3(-0.5f, -0.5f, +0.5f);
            vertices[1] = new Vector3(+0.5f, -0.5f, +0.5f);
            vertices[2] = new Vector3(-0.5f, -0.5f, -0.5f);
            vertices[3] = new Vector3(+0.5f, -0.5f, -0.5f);
            normal[0] = normal[1] = normal[2] = normal[3] = Vector3.down;

            result.Add(CreateMesh(vertices, normal, uv, tri));
        }

        return result;
    }
    public void SetDefault(out Vector3[] vertices, out Vector3[] normal, out Vector2[] uv, out int[] tri)
    {
        vertices = new Vector3[4];
        normal = new Vector3[4];
        uv = new Vector2[4];
        tri = new int[6];

        tri[0] = 0; tri[1] = 2; tri[2] = 1;
        tri[3] = 2; tri[4] = 3; tri[5] = 1;
        uv[0] = new Vector2(0, 0) + uvStart;
        uv[1] = new Vector2(uvSize.x, 0) + uvStart;
        uv[2] = new Vector2(0, uvSize.y) + uvStart;
        uv[3] = new Vector2(uvSize.x, uvSize.y) + uvStart;
    }

    Mesh CreateMesh(Vector3[] vertices,Vector3[] normal,Vector2[] uv,int[] tri)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normal;
        mesh.uv = uv;
        mesh.triangles = tri;
        return mesh;
    }


    public Vector3 GetStartVector3() { return center - Vector3.one / 2f; }
    public Vector3 GetEndVector3() { return center + Vector3.one / 2f; }
}*/