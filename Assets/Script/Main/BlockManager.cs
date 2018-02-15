using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockManager : MonoBehaviour {
    public Transform BlockPrefab;
    public List<Transform> BlockInTheWolrd;
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

    private int x = 0;
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
            AddBlockRange(BlockPrefab, new Vector3(-100, -1, -100), new Vector3(100, 0, 100), Cube.TYPE.Grass);
            AddBlockRange(BlockPrefab, new Vector3(0, 1, 0), new Vector3(100, 15, 100), Cube.TYPE.Grass);
            AddBlockRange(BlockPrefab, new Vector3(-1, 1, 0), new Vector3(-1, 8, 3), Cube.TYPE.Grass);
            //AddBlockRange(BlockPrefab, new Vector3(10, 9, 10), new Vector3(40, 30, 40));



            //deleteBlock(new Vector3(0, 0, 0));
            //AddBlock(BlockPrefab, new Vector3(0, 1, 0), Vector3.one);
            //AddBlockRange(BlockPrefab, new Vector3(0, 3, 0), new Vector3(9, 3, 0));
            //AddBlockRange(BlockPrefab, new Vector3(0, 6, 0), new Vector3(4, 6, 0));
            //AddBlockRange(BlockPrefab, new Vector3(6, 6, 0), new Vector3(9, 6, 0));
            //deleteBlock(new Vector3(4, 4, 0), Vector3.one);



            //AddBlock(BlockPrefab, new Vector3(0, 0, 0), new Vector3(11, 3, 11));
        }
        int max = 0;
        if (Input.GetKeyDown(KeyCode.Insert)) max = 1000;
        if (Input.GetKeyDown(KeyCode.Home)) max = 1;
        if (Input.GetKeyDown(KeyCode.F1))
        {
            deleteBlock(deleteIndex);
            print(deleteIndex);
            deleteIndex.x += 1f;
            if(deleteIndex.x>=8)
            {
                deleteIndex.x = 3;
                deleteIndex.y += 1f;
                if (deleteIndex.y >= 8)
                {
                    deleteIndex.y = 1;
                    deleteIndex.z += 1f;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            for (int i = 0; i < 10; i++)
            {
                AddBlockRange(BlockPrefab, new Vector3(0, y, 0 + y), new Vector3(20, y, 2 + y), Cube.TYPE.Grass);
                y++;
            }
        }


        for (int i = 0; i < max; i++)
        {
            AddBlock(BlockPrefab, new Vector3(x, y, 1 + z + y), Vector3.one, Cube.TYPE.Grass);
            x++;
            if (x >= 30)
            {
                x = 0; z++;
            }
            if (z >= 3)
            {
                z = 0; y++;
            }
        }
        
	}
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 50, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        string text = "";
        try
        {
            text = OctreeCube.count + " blocks";
        }
        catch
        {

        }
        GUI.Label(rect, text, style);
    }

    void AddBlockRange(Transform obj, Vector3 Start, Vector3 End, Cube.TYPE type)
    {
        if (Start.x > End.x) { Start.x = Start.x.Swap(ref End.x); }
        if (Start.y > End.y) { Start.y = Start.y.Swap(ref End.y); }
        if (Start.z > End.z) { Start.z = Start.z.Swap(ref End.z); }
        Vector3 Count = new Vector3(Mathf.Abs(Start.x- End.x), Mathf.Abs(Start.y - End.y), Mathf.Abs(Start.z - End.z)) + Vector3.one;
        AddBlock(obj, (Start + End)/2f, Count,type);
    }
    public bool AddBlock(Vector3 vec, Vector3 size, Cube.TYPE type)
    {
        return AddBlock(BlockPrefab, vec, size, type);
    }
    bool AddBlock(Transform obj,Vector3 vec,Vector3 size,Cube.TYPE type)
    {
        Transform tmp = Instantiate(obj, vec/3f, Quaternion.identity);
        //BlockInTheWolrd.Add(tmp);
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

        List<Cube> searchList = OctreeCube.FindRangeList(cube.GetStartVector3() - Vector3.one * 2, cube.GetEndVector3() + Vector3.one * 2);
        //print("try [" + (cube.GetStartVector3() - Vector3.one) + ">>" + (cube.GetEndVector3() + Vector3.one) + "]");
        Cube MargeTarget = null;
        
        //print("add   (" + cube.GetStartVector3() + ">>" + cube.GetEndVector3() + ")");
        foreach (Cube c in searchList)
        {
            //print("find[" + (c.GetStartVector3()) + ">>" + (c.GetEndVector3()) + "]");
            if (c.CheckCanMarge(cube))
            {
                //print("target(" + c.GetStartVector3() + ">>" + c.GetEndVector3() + ")");
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
            OctreeCube.AddValue(cube, cube.GetStartVector3(), cube.GetEndVector3());
            GetComponent<QuadManager>().AddQuadAllDir(cube);
        }
        else
        {
            MargeTarget.ExpandCubeForMarge(cube);
            DestroyImmediate(tmp.gameObject);
            //print("b : " + MargeTarget.myTree.name);
            MargeTarget.myTree.updateValue(MargeTarget);
            loopMarge(MargeTarget);
            //print("a : " + MargeTarget.myTree.name);
            //update MargeTarget
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

    public bool deleteBlock(Vector3 center, Vector3 size)
    {
        Vector3 s = center - size / 2f;
        Vector3 e = center + size / 2f;
        //print("Range(" + s + ">>" + e + ")");
        List<Cube> list = OctreeCube.FindRangeList(s + Vector3.one / 100f, e - Vector3.one / 100f);
        if (list.Count > 0)
        {
            //print("list.Count " + list.Count);
            deleteBlock(s, e, list[0]);
            return true;
        }
        else
        {
            print("Error : There is no block : " + center);
            return false;
        }
    }
    public bool deleteBlock(Vector3 s, Vector3 e, Cube target)
    {
        List<CubeRangeData> splitList = target.getSplitCubeListbyRange(s, e);
        target.myTree.PopValueAllParent(target);
        DestroyImmediate(target.gameObject);
        //print("splitList.Count : " + splitList.Count);
        int count = 0;
        foreach (CubeRangeData data in splitList)
        {
            if (data.deleteRange == false)
            {
                //print("Child(" + data.StartBlockRange + ">>" + data.EndBlockRange + ")(" + data.start + ">>" + data.end + ")");
                AddBlock(BlockPrefab, data.center, data.size, Cube.TYPE.Grass);
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
            OctreeCube.PopValueAllParent(list[0]);
            DestroyImmediate(list[0].gameObject);

            foreach (CubeRangeData data in splitList)
            {
                AddBlock(BlockPrefab, data.center, data.size, Cube.TYPE.Grass);
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
                cube.myTree.PopValueAllParent(cube);
                DestroyImmediate(cube.gameObject);
                MargeTarget.myTree.updateValue(MargeTarget);
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
}


