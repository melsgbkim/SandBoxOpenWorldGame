using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    public KeyCode KeyUp = KeyCode.UpArrow;
    public KeyCode KeyDown = KeyCode.DownArrow;
    public KeyCode KeyLeft = KeyCode.LeftArrow;
    public KeyCode KeyRight = KeyCode.RightArrow;
    public KeyCode KeyJump = KeyCode.Space;
    public KeyCode KeyRun = KeyCode.LeftShift;
    public KeyCode KeySit = KeyCode.LeftControl;
    public KeyCode KeyCancel = KeyCode.Escape;
    public KeyCode KeyMining = KeyCode.X;
    public Vector3 Forword;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        bool CancleMoveBySkill = GetComponent<PlayerSkill>().SkillUpdate();
        if (CancleMoveBySkill == false)
            GetComponent<PlayerMove>().moveUpdate(
            Input.GetKey(KeyUp), Input.GetKey(KeyDown), 
            Input.GetKey(KeyLeft), Input.GetKey(KeyRight), 
            Input.GetKey(KeyRun));
		else
			print("aaa");
		GetComponent<PlayerMove>().jumpUpdate((!CancleMoveBySkill) && Input.GetKeyDown(KeyJump));
		
		//GetComponent<PlayerMining>().actionUpdate(Input.GetKey(KeyMining));

        if(Input.GetKeyDown(KeyCode.F5))
        {
            GameObject obj = Instantiate((GameObject)Resources.Load("AttackRange/att"));
            obj.GetComponent<AttackHp>().init(10, null, 0.3f,delegate(Collider col) { });
            obj.transform.position = transform.position;
        }

    }

    
}
