using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadManager : MonoBehaviour {
    public List<Transform> QuadPrefabList;
    private List<Transform> QuadInTheworld;
    //public List<Material> QuadMaterialList;
    public enum DIRECTION
    {
        front,//-z
        back,//+z
        right,//+x
        left,//-x
        top,//+y
        bottom,//-y
        max
    };
    private Vector3[] Vec3Arr = new Vector3[(int)(DIRECTION.max)];
    private Quaternion[] QuatArr = new Quaternion[(int)(DIRECTION.max)];

    public static List<QuadManager.DIRECTION> DirList = new List<QuadManager.DIRECTION>();
    public QuadManager()
    {
        if(DirList.Count == 0)
        {
            DirList.Add(DIRECTION.front);
            DirList.Add(DIRECTION.back);
            DirList.Add(DIRECTION.right);
            DirList.Add(DIRECTION.left);
            DirList.Add(DIRECTION.top);
            DirList.Add(DIRECTION.bottom);
        }
    }
    // Use this for initialization
    void Start () {
        Vec3Arr[(int)(DIRECTION.front)] = new Vector3(0, 0, -0.5f);
        QuatArr[(int)(DIRECTION.front)] = Quaternion.Euler(0, 0, 0);

        Vec3Arr[(int)(DIRECTION.back)] = new Vector3(0, 0, 0.5f);
        QuatArr[(int)(DIRECTION.back)] = Quaternion.Euler(0, 180, 0);

        Vec3Arr[(int)(DIRECTION.right)] = new Vector3(0.5f, 0, 0);
        QuatArr[(int)(DIRECTION.right)] = Quaternion.Euler(0, -90, 0);

        Vec3Arr[(int)(DIRECTION.left)] = new Vector3(-0.5f, 0, 0);
        QuatArr[(int)(DIRECTION.left)] = Quaternion.Euler(0, 90, 0);

        Vec3Arr[(int)(DIRECTION.top)] = new Vector3(0, 0.5f, 0);
        QuatArr[(int)(DIRECTION.top)] = Quaternion.Euler(90, 0, 0);

        Vec3Arr[(int)(DIRECTION.bottom)] = new Vector3(0, -0.5f, 0);
        QuatArr[(int)(DIRECTION.bottom)] = Quaternion.Euler(-90, 0, 0);

        QuadInTheworld = new List<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddQuadAllDir(Cube cube)
    {
        List<DIRECTION> dirList = new List<DIRECTION>();
        dirList.Add(DIRECTION.front);
        dirList.Add(DIRECTION.back);
        dirList.Add(DIRECTION.right);
        dirList.Add(DIRECTION.left);
        dirList.Add(DIRECTION.top);
        dirList.Add(DIRECTION.bottom);
        AddQuad(cube, dirList);
        cube.setQuadSize();
    }
    public void AddQuad(Cube cube,List<DIRECTION> dirList)
    {
        foreach(DIRECTION dir in dirList)
        {
            //print(cube);
            //print(QuadPrefabList[(int)(cube.type)]);
            //print(dir);
            Transform quad = Instantiate(QuadPrefabList[(int)(cube.type)], Vector3.zero, Quaternion.identity);
            quad.parent = cube.transform;
            quad.localPosition = Vec3Arr[(int)(dir)];
            quad.localRotation = QuatArr[(int)(dir)];
            quad.localScale = Vector3.one;
            cube.QuadList[(int)(dir)] = quad;
            QuadInTheworld.Add(quad);
        }
    }
}
