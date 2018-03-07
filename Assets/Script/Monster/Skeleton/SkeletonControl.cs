using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonControl : MonsterControl
{
    MonsterBase animationControl = null;
    // Use this for initialization
    void Start () {
        hp = GetComponent<HpStat>();
        animationControl = GetComponent<SkeletonAnimationControl>();

        hp.DecreaseAction = HitDamage;

        MonsterName = "SkeletonTester";
    }
	
	// Update is called once per frame
	void Update () {
        CheckDead();

    }

    void HitDamage(float val)
    {
        if (val > 1)
            animationControl.tryChangeTrigger("Hit");
    }

    public override void Dead()
    {
        base.Dead();
        print(MonsterName + " Is Dead");
    }
}
