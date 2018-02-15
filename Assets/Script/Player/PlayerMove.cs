using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    
    public Vector3 moveForword;
    
    public float moveSpeed = 20f;
    public float velSpeed = 100f;
    public float RunPower = 2f;
    public float jumpPower = 10f;

    float JumpVel = 0;

    public Vector3 height = new Vector3(0, 2.2f, 0);
    public Vector3 HeightStairs = new Vector3(0, 1.1f, 0) / 3f;
    public Vector3 HeightDistanceToPoint = new Vector3(0, 1f, 0) - new Vector3(0, 1, 0) * 0.5f;

    // Use this for initialization
    void Start () {
        moveForword = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update() {
        moveForword = GetComponent<PlayerInput>().Forword;
    }

    public Vector3 GetOtherVector3Dir()
    {
        Vector3 dir = new Vector3(0, 0, 0);
        PlayerInput pi = GetComponent<PlayerInput>();
        if(pi != null)
        {
            if (Input.GetKey(pi.KeyUp    )) dir += moveForword;
            if (Input.GetKey(pi.KeyDown  )) dir += Quaternion.Euler(0, 180, 0) * moveForword;
            if (Input.GetKey(pi.KeyLeft  )) dir += Quaternion.Euler(0, -90, 0) * moveForword;
            if (Input.GetKey(pi.KeyRight )) dir += Quaternion.Euler(0, 90, 0) * moveForword;
            if (Input.GetKey(pi.KeyJump  )) dir += Vector3.up;
            if (Input.GetKey(pi.KeySit   )) dir += Vector3.down;
            if (Input.GetKey(pi.KeyRun   )) dir *= RunPower;
        }
        return dir;
    }

    public void moveUpdate(bool getKeyUp, bool getKeyDown, bool getKeyLeft, bool getKeyRight, bool getKeyRun)
    {
        
        Vector3 dir = new Vector3(0, 0, 0);
        float move = moveSpeed; 
        
        if (getKeyUp) dir += moveForword;
        if (getKeyDown) dir += Quaternion.Euler(0, 180, 0) * moveForword;
        if (getKeyLeft) dir += Quaternion.Euler(0, -90, 0) * moveForword;
        if (getKeyRight) dir += Quaternion.Euler(0, 90, 0) * moveForword;

        float speedMax = moveSpeed;
        if (getKeyRun) move *= RunPower;

        int div = 5;
        for(int i=0;i< div; i++)
            moveDir(dir, move / div * Time.deltaTime);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void jumpUpdate(bool getKeyDownJump)
    {
        if (isGround() == false)
            UpdateJump();
        else if (getKeyDownJump)
        {
            JumpVel = jumpPower;
            UpdateJump();
        }
        else
            JumpVel = 0;
    }

    void moveDir(Vector3 dir,float distance)
    {
        RaycastHit hitStairs;
        
        float castDistance = 0.1f;

        List<moveData> list = new List<moveData>();

        if (dir == Vector3.zero)
            return;

        RaycastHit hit;
        moveData tmp;
        if (tryMove(dir, distance, Vector3.zero, out tmp))
            list.Add(new moveData(transform.position + dir * distance, distance, false, "1"));
        else
            list.Add(tmp);

        if (tryMove(dir, distance, HeightStairs, out tmp))
            list.Add(new moveData(transform.position + dir * distance+ HeightStairs, distance, false, "2"));
        else
            list.Add(tmp);

        list.Sort(SortMoveData);

        if (list.Count > 0)
        {
            //print(printData(list));
            transform.position = list[0].position;

        }
    }
    string printData(List<moveData> list)
    {
        string result = "console["+ list.Count+ "] : \n";
        foreach(moveData data in list)
        {
            result += data.name + " : [" + transform.position.x + "," + transform.position.y + "," + transform.position.z + "] >> [" + data.position.x + "," + data.position.y + "," + data.position.z + "]  " + (data.collision ? "TRUE" : "")+"\n";
        }
        return result;
    }

    bool checkCanMove(Vector3 dir, float distance, Vector3 stair)
    {
        float tmp = 60f;
        return Physics.CapsuleCast(transform.position - HeightDistanceToPoint - dir / tmp + stair, transform.position + HeightDistanceToPoint - dir / tmp + stair, 0.45f, dir, distance + 1 / tmp, 1 << LayerMask.NameToLayer("Cube"));
    }

    bool tryMove(Vector3 dir, float distance,Vector3 stair, out moveData data)
    {
        RaycastHit hit;
        float tmp = 60f;
        if (Physics.CapsuleCast(transform.position - HeightDistanceToPoint - dir / tmp + stair, transform.position + HeightDistanceToPoint - dir / tmp + stair, 0.45f, dir,out hit, distance + 1 / tmp, 1 << LayerMask.NameToLayer("Cube")))
        {
            data = new moveData(transform.position + dir * (hit.distance - (1 / tmp)), hit.distance - (1 / tmp), true, "3_"+stair.y);
            return false;
        }
        data = new moveData(transform.position + dir * (distance), distance, false, "4");
        return true;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position - HeightDistanceToPoint + new Vector3(0, JumpVel * Time.deltaTime,0), 0.5f);
    }


    public float jumpR = 0.55f;
    void UpdateJump()
    {
        JumpVel -= 30f * Time.deltaTime;
        
        
        RaycastHit hit;
        List<moveData> list = new List<moveData>();
        Vector3 dir = new Vector3(0, 1, 0);
        float jumpDelta = Mathf.Abs(JumpVel * Time.deltaTime);
        if (JumpVel < 0) dir *= -1;
        
        if (Physics.CapsuleCast(transform.position - HeightDistanceToPoint, transform.position + HeightDistanceToPoint, jumpR, dir , out hit, jumpDelta, 1 << LayerMask.NameToLayer("Cube")))
        {//col
            list.Add(new moveData(transform.position + dir * hit.distance, hit.distance, true, "5"));
        }
        else
        {//falling
            list.Add(new moveData(transform.position + dir * jumpDelta, jumpDelta, false, "6"));
        }
        list.Sort(SortJumpData);
        //print("J" + transform.position.x + " >" + dir + "> " + list[0].position.x);
        transform.position = list[0].position;

        if (list[0].collision)
            JumpVel = 0;
    }

    bool isGround()
    {
        Vector3 s = new Vector3(0, 1 / 60f, 0);
        return Physics.CapsuleCast(transform.position - HeightDistanceToPoint + s, transform.position + HeightDistanceToPoint+s, jumpR, Vector3.down, 2/60f, 1 << LayerMask.NameToLayer("Cube"));
    }

    class moveData
    {
        public Vector3 position;
        public float point;
        public bool collision;
        public string name;
        public moveData(Vector3 p,float point,bool col,string name)
        {
            this.position = p;
            this.point = point;
            this.collision = col;
            this.name = name;
        }
    }

    int SortMoveData(moveData a, moveData b)
    {
        if (a.point < b.point) return 1;
        else if (a.point > b.point) return -1;
        else
        {
            if (a.position.y > b.position.y) return 1;
            else if (a.position.y < b.position.y) return -1;
            else return 0;
        }
    }

    int SortJumpData(moveData a, moveData b)
    {
        if (a.point > b.point) return 1;
        else if (a.point < b.point) return -1;
        else return 0;
    }
}
