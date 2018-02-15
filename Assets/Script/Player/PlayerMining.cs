using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMining : MonoBehaviour {
    public BlockManager blockmanager;
    Vector3 forword;
	// Use this for initialization
	void Start () {
		
	}

    public void actionUpdate(bool getKeyMining)
    {
        if (getKeyMining)
        {
            //Mining(-45, new Vector3(0.5f, 1.3f, 0.05f), new Vector3(0, 0.6f, 0) / 3f);
            //Mining(-30, new Vector3(0.5f, 1.3f, 0.05f), new Vector3(0, 0.6f, 0) / 3f);
            //Mining(0, new Vector3(0.5f, 0.9f, 0.05f), new Vector3(0, 0, 0) / 3f);
            //Mining(30, new Vector3(0.5f, 1.1f, 0.05f), new Vector3(0, 1.4f, 0) / 3f);
            //Mining(45, new Vector3(0.5f, 1.1f, 0.05f), new Vector3(0, 3f, 0) / 3f);
        }
    }
    /*void OnDrawGizmos()
    {
        RaycastHit hitInfo;
        Quaternion rot = Quaternion.LookRotation(forword);
        if (Physics.BoxCast(transform.position, new Vector3(0.45f, 0.9f, 0.45f), forword, out hitInfo, rot, 100f, 1 << LayerMask.NameToLayer("Cube")))
        {
        }
    }*/

    public void Mining(float RotX,Vector3 size, Vector3 startPos)
    {
        //print("few few["+ transform.position+"] >> ["+ (transform.position + GetComponent<PlayerInput>().Forword)+"]");
        RaycastHit hitInfo;
        forword = GetComponent<PlayerInput>().Forword;
        //print("A : " + forword);
        Quaternion rot = Quaternion.LookRotation(forword);
        forword = rot * (Quaternion.Euler(RotX, 0, 0) * (Quaternion.Inverse(rot) * forword));
        rot = Quaternion.LookRotation(forword);
        //print("B : " + forword);
        
        if (Physics.BoxCast(transform.position- Vector3.ClampMagnitude(forword,0.3f)+ startPos, size, forword, out hitInfo, rot, 100f, 1 << LayerMask.NameToLayer("Cube")))
        {

            Cube c = hitInfo.transform.GetComponent<Cube>();
            Vector3 tmpVector = new Vector3(Mathf.Round(hitInfo.point.x * 3), Mathf.Round(hitInfo.point.y * 3), Mathf.Round(hitInfo.point.z * 3));
            tmpVector = c.getPositionFromCollisionPos(tmpVector);            
            Vector3 v = hitInfo.point * 3;

            List<Cube> tmpList = c.myTree.FindRangeList(tmpVector, tmpVector);

            DropItem.DropItemPosCube(c.type, 1, hitInfo.point, DropItem.RandomUpperVel());            

            if (blockmanager.deleteBlock(tmpVector- Vector3.one / 2f, tmpVector+ Vector3.one / 2f, c) == false)
            {
                print("hitInfo.transform.name : " + hitInfo.transform.name);
            }
            else
            {
                blockmanager.loopMargeFromDeletedCube(tmpVector);
            }

        }
        else
        {
            print("NO");

        }
    }

    void GetMiningTargetSimpleRange(Vector3 center, Vector3 size, out Vector3 s, out Vector3 e)
    {
        List<Vector3> list = new List<Vector3>();
        list.Add(center + new Vector3(-size.x, -size.y, -size.z));
        list.Add(center + new Vector3(+size.x, -size.y, -size.z));
        list.Add(center + new Vector3(-size.x, +size.y, -size.z));
        list.Add(center + new Vector3(+size.x, +size.y, -size.z));
        list.Add(center + new Vector3(-size.x, -size.y, +size.z));
        list.Add(center + new Vector3(+size.x, -size.y, +size.z));
        list.Add(center + new Vector3(-size.x, +size.y, +size.z));
        list.Add(center + new Vector3(+size.x, +size.y, +size.z));
        s = list[0];
        e = list[0];
        foreach(Vector3 v in list)
        {
            if (v.x < s.x) s.x = v.x;
            if (v.y < s.y) s.y = v.y;
            if (v.z < s.z) s.z = v.z;
            if (v.x > e.x) e.x = v.x;
            if (v.y > e.y) e.y = v.y;
            if (v.z > e.z) e.z = v.z;
        }
    }

    List<List<Vector3>> getBlockPosList(Vector3 center, Vector3 size, Quaternion rot, out Vector3 s, out Vector3 e)
    {
        List<List<Vector3>> result = new List<List<Vector3>>();
        Vector3 Start = (center - size)*3;
        Vector3 End = (center + size)*3;
        GetMiningTargetSimpleRange(center*3, rot*(size*3), out s, out e);
        s = new Vector3(Mathf.Floor(s.x), Mathf.Floor(s.y), Mathf.Floor(s.z));
        e = new Vector3(Mathf.Ceil(e.x), Mathf.Ceil(e.y), Mathf.Ceil(e.z));
        float height = (e.y - s.y) + 1;
        float widthX = (e.x - s.x) + 1;
        float widthZ = (e.z - s.z) + 1;
        List<Vector3> FloorList = new List<Vector3>();
        for (int x = 0; x < widthX; x++)
        {
            for (int z = 0; z < widthZ; z++)
            {
                Vector3 v = Quaternion.Inverse(rot) * (new Vector3(s.x + x - (s.x + e.x) / 2f, 0, s.z + z - (s.z + e.z) / 2f)) + (s+e)/2f;
//                print("x[" + Start.x + "<" + v.x + "<" + End.x + "]" + "    " + "z[" + Start.z + "<" + v.z + "<" + End.z + "]" + (v.x < Start.x) + " " + (v.x > End.x) + " " + (v.z < Start.z) + " " + (v.z < End.z));
                if (v.x < Start.x) continue;
                if (v.x > End.x) continue;
                if (v.z < Start.z) continue;
                if (v.z > End.z) continue;
                FloorList.Add(new Vector3(s.x + x, 0, s.z + z));
            }
        }
        print("FloorList.Count : " + FloorList.Count);

        for (int i = 0; i < height; i++)
        {
            List<Vector3> list = new List<Vector3>();
            foreach(Vector3 v in FloorList)
            {
                list.Add(new Vector3(v.x,s.y + i, v.z));
            }
            result.Add(list);
        }

        return result;
    }

    class MiningData
    {
        public Vector3 playerPos;
        public Vector3 blockPos;
        public float distance;
        public Cube cube;
        public MiningData(Vector3 player,Vector3 block, Cube c)
        {
            playerPos = player;
            blockPos = block;
            distance = (blockPos - playerPos).sqrMagnitude;
            cube = c;
        }
    }




    //----------------------------------------------------

    
}



class idontknow
{
    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
    {
        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
        DrawBox(origin, halfExtents, orientation, color);
    }

    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
    {
        direction.Normalize();
        Box bottomBox = new Box(origin, halfExtents, orientation);
        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

        DrawBox(bottomBox, color);
        DrawBox(topBox, color);
    }

    public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        DrawBox(new Box(origin, halfExtents, orientation), color);
    }
    public static void DrawBox(Box box, Color color)
    {
        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
        Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
        Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
    }

    public struct Box
    {
        public Vector3 localFrontTopLeft { get; private set; }
        public Vector3 localFrontTopRight { get; private set; }
        public Vector3 localFrontBottomLeft { get; private set; }
        public Vector3 localFrontBottomRight { get; private set; }
        public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
        public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
        public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
        public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

        public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
        public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
        public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
        public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
        public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
        public Vector3 backTopRight { get { return localBackTopRight + origin; } }
        public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
        public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

        public Vector3 origin { get; private set; }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
        }
        public Box(Vector3 origin, Vector3 halfExtents)
        {
            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }


        public void Rotate(Quaternion orientation)
        {
            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }

    //This should work for all cast types
    static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
    {
        return origin + (direction.normalized * hitInfoDistance);
    }

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        Vector3 direction = point - pivot;
        return pivot + rotation * direction;
    }

}