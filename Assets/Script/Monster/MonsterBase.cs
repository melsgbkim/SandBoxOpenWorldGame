using UnityEngine;
using System.Collections;

// 必要なコンポーネントの列記
[RequireComponent(typeof(Animator))]

public class MonsterBase : MonoBehaviour
{
    protected Animator anim;
    protected AnimatorStateInfo currentBaseState;

    void Start()
    {
        
    }

    public void init()
    {
        anim = GetComponent<Animator>();
    }

    public bool ChangeFloat(string name, float val)
    {
        //print("tryChangeFloat : [" + name + ":" + val + "]");
        try
        {
            anim.SetFloat(name, val);     // Animatorにジャンプに切り替えるフラグを送る
            return true;
        }
        catch (System.Exception e)
        {
            print(e.Message);
        }
        return false;
    }

    public bool tryChangeBool(string name, bool val)
    {
        //ステート遷移中でなかったらジャンプできる
        if (!anim.IsInTransition(0))
        {
            return ChangeBool(name, val);
        }
        else
            return false;
    }

    public bool ChangeBool(string name, bool val)
    {
        try
        {
            anim.SetBool(name, val);     // Animatorにジャンプに切り替えるフラグを送る
            return true;
        }
        catch (System.Exception e)
        {
            print(e.Message);
        }
        return false;
    }


    public bool tryChangeTrigger(string name)
    {
        //ステート遷移中でなかったらジャンプできる
        if (!anim.IsInTransition(0))
        {
            return ChangeTrigger(name);
        }
        else
            return false;
    }

    public bool ChangeTrigger(string name)
    {
        try
        {
            anim.SetTrigger(name);     // Animatorにジャンプに切り替えるフラグを送る
            return true;
        }
        catch (System.Exception e)
        {
            print(e.Message);
        }
        return false;
    }
}