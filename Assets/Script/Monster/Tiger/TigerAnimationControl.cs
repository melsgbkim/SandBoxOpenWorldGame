using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerAnimationControl : MonsterBase
{
    float v = 0f;
    public float speed = 10f;

    static int IdleState = Animator.StringToHash("Base Layer.Idle");
    static int WalkState = Animator.StringToHash("Base Layer.Walk");
    static int RunState =  Animator.StringToHash("Base Layer.Run");
    static int AttState = Animator.StringToHash("Base Layer.Attack");
    static int SoundState = Animator.StringToHash("Base Layer.Sound");
    // Use this for initialization
    void Start () {
        base.init();
    }
	
	// Update is called once per frame
	void Update () {
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        if (Input.GetKey(KeyCode.F11)) v += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.F12)) v -= speed * Time.deltaTime;
        ChangeFloat("Speed", v);
        // to do 입력키 정리
    }
}
