using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockManager : MonoBehaviour {
    public Transform BlockPrefab;
    public List<Transform> BlockInTheworld;
    public OctreeNode OctreeCube;
    public static BlockManager manager = null;

    bool firstUpdate = true;
    // Use this for initialization
    void Start () {
        //AddBlockRange(BlockPrefab, new Vector3(-10, 0, -10), new Vector3(60, 3, 60));
        OctreeCube = new OctreeNode();
        OctreeCube.SetRoot();
        print(OctreeCube);

        deleteIndex = new Vector3(3, 1, 0);
        if(manager == null)manager = this;
    }

    private int x = -10;
    private int y = 0;
    private int z = 0;
    private int count = 0;
    private int num = 0;

    Vector3 deleteIndex;
    // Update is called once per frame
    void Update () {
        if(firstUpdate)
        {
            firstUpdate = false;
            AddBlockRange(BlockPrefab, new Vector3(-100, -5, -100), new Vector3(100, -1, 100), "cube_00000001", true);
            AddBlockRange(BlockPrefab, new Vector3(11, 0, 0), new Vector3(100, 9, 100), "cube_00000001", true);
            
            AddBlockRange(BlockPrefab, new Vector3(0, 0, 0), new Vector3(10, 0, 10), "cube_00000001", true);
            AddBlockRange(BlockPrefab, new Vector3(0, 1, 0), new Vector3(10, 1, 10), "cube_00000002", true);
            AddBlockRange(BlockPrefab, new Vector3(0, 2, 0), new Vector3(10, 2, 10), "cube_00000003", true);
            AddBlockRange(BlockPrefab, new Vector3(0, 3, 0), new Vector3(10, 3, 10), "cube_00000004", true);
            AddBlockRange(BlockPrefab, new Vector3(0, 4, 0), new Vector3(10, 4, 10), "cube_00000005", true);
            AddBlockRange(BlockPrefab, new Vector3(0, 5, 0), new Vector3(10, 5, 10), "cube_00000002", true);
            AddBlockRange(BlockPrefab, new Vector3(0, 6, 0), new Vector3(10, 6, 10), "cube_00000003", true);
            AddBlockRange(BlockPrefab, new Vector3(0, 7, 0), new Vector3(10, 7, 10), "cube_00000004", true);
            AddBlockRange(BlockPrefab, new Vector3(0, 8, 0), new Vector3(10, 8, 10), "cube_00000005", true);
        }
	}


    void AddBlockRange(Transform obj, Vector3 Start, Vector3 End, string type, bool createMesh)
    {
        if (Start.x > End.x) { Start.x = Start.x.Swap(ref End.x); }
        if (Start.y > End.y) { Start.y = Start.y.Swap(ref End.y); }
        if (Start.z > End.z) { Start.z = Start.z.Swap(ref End.z); }
        Vector3 Count = new Vector3(Mathf.Abs(Start.x- End.x), Mathf.Abs(Start.y - End.y), Mathf.Abs(Start.z - End.z)) + Vector3.one;
        AddBlock(obj, (Start + End)/2f, Count,type, createMesh);
    }
    public bool AddBlock(Vector3 vec, Vector3 size, string type, bool createMesh)
    {
        return AddBlock(BlockPrefab, vec, size, type, createMesh);
    }
    bool AddBlock(Transform obj,Vector3 vec,Vector3 size,string type, bool createMesh)
    {
        Transform tmp = Instantiate(obj, vec/3f, Quaternion.identity);
        //BlockInTheworld.Add(tmp);
        tmp.name = "Block_" + num;
        num++;
        Cube cube = tmp.GetComponent<Cube>();
        cube.type = type;
        cube.init();
        cube.setSize(size);
        List<Cube> CollisionList = OctreeCube.FindRangeList(cube.GetStartVector3()+Vector3.one/100f, cube.GetEndVector3()- Vector3.one / 100f);
        if (CollisionList.Count != 0)
        {
            print("CollisionList.Count : " + CollisionList.Count);
            print("CollisionList[0]" + CollisionList[0].GetStartVector3() + "  size:" + CollisionList[0].GetEndVector3());
            DestroyImmediate(tmp.gameObject);
            return false;
        }
        if(createMesh)
            WorldMeshCubeManager.Get.NewMeshCube(cube.Center, cube.type, cube.Size);
        List<Cube> searchList = OctreeCube.FindRangeList(cube.GetStartVector3() - Vector3.one * 2, cube.GetEndVector3() + Vector3.one * 2);
        Cube MargeTarget = null;
        
        foreach (Cube c in searchList)
        {
            if (c.CheckCanMarge(cube))
            {
                if (MargeTarget == null) MargeTarget = c;
                else
                {
                    if(MargeTarget.Size.magnitude < c.Size.magnitude)
                    {
                        MargeTarget = c;
                    }
                }
            }
        }
        if (MargeTarget == null)
        {
            OctreeCube.AddValue(new TreeAble(cube), cube.GetStartVector3(), cube.GetEndVector3());
          
        }
        else
        {
            MargeTarget.ExpandCubeForMarge(cube);
            DestroyImmediate(tmp.gameObject);
            MargeTarget.myTree.updateValue(new TreeAble(MargeTarget));
            loopMarge(MargeTarget);
        }
        return true;
    }

    public List<Cube> FindBlockRange(Vector3 s, Vector3 e)
    {
        return OctreeCube.FindRangeList(s - Vector3.one /2f + Vector3.one / 100f, e + Vector3.one / 2f - Vector3.one / 100f);
    }
    public void deleteBlockRange(Vector3 s,Vector3 e)
    {
        if (s.x > e.x) { s.x = s.x.Swap(ref e.x); }
        if (s.y > e.y) { s.y = s.y.Swap(ref e.y); }
        if (s.z > e.z) { s.z = s.z.Swap(ref e.z); }
        Vector3 Count = new Vector3(Mathf.Abs(s.x - e.x), Mathf.Abs(s.y - e.y), Mathf.Abs(s.z - e.z)) + Vector3.one;
        for (float x = s.x; x <= e.x; x += 1f)
        {
            for (float y = s.y; y <= e.y; y += 1f)
            {
                for (float z = s.z; z <= e.z; z += 1f)
                {
                    deleteBlock(new Vector3(x, y, z));
                }
            }
        }
    }


    public bool deleteBlock(Vector3 center)
    {
        return deleteBlock(center, Vector3.one);
    }

    public bool deleteBlock(Vector3 center, Vector3 size, Cube target = null)
    {
        Vector3 s = center - size / 2f;
        Vector3 e = center + size / 2f;
        //print("Range(" + s + ">>" + e + ")");
        
        if(target == null)
        {
            List<Cube> list = OctreeCube.FindRangeList(s + Vector3.one * 0.01f, e - Vector3.one * 0.01f);
            if (list.Count > 0)
                target = list[0];
        }

        if (target != null)
        {
            //print("list.Count " + list.Count);
            List<Vector3> dirList = DirListDefault;
            List<Vector3> dirListResult = null;
            string targetType = target.type;
            deleteBlock(s, e, target, out dirListResult,dirList);

            List<Cube> list = OctreeCube.FindRangeList(s + Vector3.one * 0.01f - Vector3.one, e - Vector3.one * 0.01f + Vector3.one);
            List<Vector3> CheckList = DirListDefault;
            for (int i = 0; i < CheckList.Count; i++)
            {
                bool InList = false;
                for (int j = 0; j < dirListResult.Count; j++)
                {
                    if (CheckList[i] == dirListResult[j])
                        InList = true;
                }
                if(InList == false)
                {//checkCube
                    bool created = false;
                    for (int k = 0; k < list.Count; k++)
                    {
                        Vector3 c = CheckList[i] + center;
                        if (list[k].CheckThisCubeCollideRange(c,c))
                        {//create mesh
                            WorldMeshCubeManager.Get.NewMeshByDeleteCube(center,list[k].type,size,CheckList[i]);
                            created = true;
                            break;
                        }
                    }
                    if(created == false)
                        WorldMeshCubeManager.Get.DeleteMeshDirFromCenter(center, targetType, size, CheckList[i]);
                }
                else
                {//create mesh this type;
                    WorldMeshCubeManager.Get.NewMeshByDeleteCube(center, targetType, size, CheckList[i]);
                }
            }
            return true;
        }
        else
        {
            print("Error : There is no block : " + center);
            return false;
        }
    }
    public bool deleteBlock(Vector3 s, Vector3 e, Cube target, out List<Vector3> dirListResult,List<Vector3> dirList = null)
    {
        Vector3 c = (s + e) * 0.5f;
        dirListResult = new List<Vector3>();
        List<CubeRangeData> splitList = target.getSplitCubeListbyRange(s, e);
        target.myTree.PopValueAllParent(new TreeAble(target));
        //WorldMeshCubeManager.Get.DeleteMeshRange(target.Center, target.type, target.Size);
        Destroy(target.gameObject);
        if (dirList == null)
            dirList = new List<Vector3>();
        //print("splitList.Count : " + splitList.Count);
        int count = 0;
        foreach (CubeRangeData data in splitList)
        {
            if (data.deleteRange == false)
            {
                //print("Child(" + data.StartBlockRange + ">>" + data.EndBlockRange + ")(" + data.start + ">>" + data.end + ")");
                AddBlock(BlockPrefab, data.center, data.size, target.type, false);
                for(int i=0;i< dirList.Count;i++)
                {
                    if (data.Vec3InThisRange(c + dirList[i]))
                        dirListResult.Add(dirList[i]);
                }
                
                count += 1;
            }
        }
        if(count == 0)
        {
            loopMargeFromDeletedCube((s+e)/2f);
        }
        return true;
    }

    void splitBlock(Vector3 center, Vector3 size)
    {
        Vector3 s = center - size / 2f;
        Vector3 e = center + size / 2f;
        //print("Range(" +s + ">>" +e + ")");
        List<Cube> list = OctreeCube.FindRangeList(s + Vector3.one / 100f, e - Vector3.one / 100f);
        if (list.Count > 0)
        {
            //print("Main (" + list[0].GetStartVector3() + ">>" + list[0].GetEndVector3() + ")");
            List<CubeRangeData> splitList = list[0].getSplitCubeListbyRange(s, e);
            OctreeCube.PopValueAllParent(new TreeAble(list[0]));
            DestroyImmediate(list[0].gameObject);

            foreach (CubeRangeData data in splitList)
            {
                AddBlock(BlockPrefab, data.center, data.size, "cube_00000001", false);
            }
        }
        else
        {
            print("Error : There is no block");
        }
    }
    public void loopMargeFromDeletedCube(Vector3 pos)
    {
        List<Cube> searchList = OctreeCube.FindRangeList(pos - Vector3.one * 2, pos + Vector3.one * 2);
        if(searchList.Count > 0)
        {
            loopMarge(searchList[0]);
        }
    }


    void loopMarge(Cube t)
    {
        Cube cube = t;
        Cube MargeTarget = null;

        while(true)
        { 
            List<Cube> searchList = OctreeCube.FindRangeList(cube.GetStartVector3() - Vector3.one * 2, cube.GetEndVector3() + Vector3.one * 2);
            foreach (Cube c in searchList)
            {
                if (cube.Equals(c))
                    continue;
                //print("find[" + (c.GetStartVector3()) + ">>" + (c.GetEndVector3()) + "]");
                if (c.CheckCanMarge(cube))
                {
                    if (MargeTarget == null) MargeTarget = c;
                    else
                    {
                        if (MargeTarget.Size.magnitude < c.Size.magnitude)
                        {
                            MargeTarget = c;
                        }
                    }
                }
            }

            if (MargeTarget != null)
            {
                MargeTarget.ExpandCubeForMarge(cube);
                cube.myTree.PopValueAllParent(new TreeAble(cube));
                DestroyImmediate(cube.gameObject);
                MargeTarget.myTree.updateValue(new TreeAble(MargeTarget));
                cube = MargeTarget;
                MargeTarget = null;
            }
            else if(MargeTarget == null)
                break;
        }
    }

    public bool FullRangeWithType(Vector3 s, Vector3 e, Cube cube)
    {
        bool result = false;

        List<Cube> list = FindBlockRange(s, e);
        int count = 0;
        int max = Mathf.RoundToInt((e.x - s.x + 1) * (e.y - s.y + 1) * (e.y - s.y + 1));

        //find My List
        foreach (Cube c in list)
        {
            if (c.type != cube.type)
                return false;
            count += c.GetCollisionRangeCountOtherCube(cube);
        }
        
        return count == max;
    }

    List<Vector3> DirListDefault
    {
        get
        {
            List<Vector3> tmp = new List<Vector3>();
            tmp.Add(Vector3.forward);
            tmp.Add(Vector3.back);
            tmp.Add(Vector3.right);
            tmp.Add(Vector3.left);
            tmp.Add(Vector3.up);
            tmp.Add(Vector3.down);
            return tmp;
        }
    }
}


