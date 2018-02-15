using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHp : Attack
{
    public void init(float damage,GameObject p,float end)
    {
        getStat = delegate (Collider other)
        {
            return other.GetComponent<HpStat>();
        };

        this.damage = damage;
        starter = p;
        SetEndTime(end);
    }
}