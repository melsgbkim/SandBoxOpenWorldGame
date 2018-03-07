using UnityEngine;
using UnityEditor;

public class MonsterControl : MonoBehaviour
{
    public string MonsterName = "";
    public HpStat hp = null;
    // Use this for initialization
    void Start()
    {
        hp = GetComponent<HpStat>();

    }

    // Update is called once per frame
    public virtual bool isHpUnderValue(float value)
    {
        if(hp != null)
        {
            if (hp.value <= value)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void Dead()
    {
        print("Drop");
        print("Effect");
        print("Exp");
        Destroy(gameObject);
    }

    public virtual void CheckDead()
    {
        if (isHpUnderValue(0))
            Dead();
    }
}