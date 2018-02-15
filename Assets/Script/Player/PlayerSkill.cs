using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour {
    public Hashtable SkillKeyTable = new Hashtable();
    public SkillUI SkillUI;
    public List<KeyCode> keyList = new List<KeyCode>();
    public Skill nowSkill = null;
    // Use this for initialization

    void SkillSetting()
    {
        
        keyList.Add(KeyCode.Z);
        keyList.Add(KeyCode.X);
        keyList.Add(KeyCode.C);
        keyList.Add(KeyCode.V);
        keyList.Add(KeyCode.B);
        keyList.Add(KeyCode.N);

        keyList.Add(KeyCode.A);
        keyList.Add(KeyCode.S);
        keyList.Add(KeyCode.D);
        keyList.Add(KeyCode.F);
        keyList.Add(KeyCode.G);
        keyList.Add(KeyCode.H);

        keyList.Add(KeyCode.Q);
        keyList.Add(KeyCode.W);
        keyList.Add(KeyCode.E);
        keyList.Add(KeyCode.R);
        keyList.Add(KeyCode.T);
        keyList.Add(KeyCode.Y);

        SkillUI.Init();
        int min = keyList.Count;
        for (int i = 0; i < min; i++)
        {
            SkillKeyTable.Add(keyList[i], SkillUI.skillList[i]);
            SkillUI.skillList[i].GetComponent<SkillSlot>().key = keyList[i];
        }

        Skill.Player = gameObject;
        (SkillKeyTable[KeyCode.S] as GameObject).GetComponent<SkillSlot>().setSkill(new SkillMiningFront());
        (SkillKeyTable[KeyCode.A] as GameObject).GetComponent<SkillSlot>().setSkill(new SkillMiningDown());
		(SkillKeyTable[KeyCode.D] as GameObject).GetComponent<SkillSlot>().setSkill(new SkillMiningUp());
		(SkillKeyTable[KeyCode.X] as GameObject).GetComponent<SkillSlot>().setSkill(new SkillBasicAttack());


	}
	void Start () {
        SkillSetting();
    }
	
	// Update is called once per frame
	public bool SkillUpdate () {
        if (nowSkill == null)
        {
            foreach (KeyCode key in keyList)
            {
                Skill skill = (SkillKeyTable[key] as GameObject).GetComponent<SkillSlot>().skill;
                if (skill != null &&
                    ((skill.OnlyKeyDown == true && Input.GetKeyDown(key)) ||
                    (skill.OnlyKeyDown == false && Input.GetKey(key))))
                {
                    if (tryInputSkill(skill))
                    {
                        nowSkill = skill;
                        return skill.isStopOtherAction;
                    }
                }

            }
        }
        else
        {
            if (Input.GetKeyDown(nowSkill.slot.key))
                nowSkill.NewAction();
            if (nowSkill.CanActivateNow())
            {
                if(nowSkill.SkillDesappearFromSlot())
                {
                    nowSkill.slot.setSkill(null);
                }
                nowSkill = null;
            }
            else
            {
                nowSkill.Update();
                return nowSkill.isStopOtherAction;
            }
        }
        return false;
	}

    bool tryInputSkill(Skill s)
    {
        if (s == null) return false;
        if (s.HavePoint() == false) return false;
        if (s.CanActivateNow())
        {
            s.Start();
            return true;
        }
        else
            return false;
    }
}

public class CoolTime
{
    public float time = 0f;
    public float StartTime = 0f;
    public float WaitToTime = 0f;

    public void Start()
    {
        StartTime = Time.time;
        WaitToTime = StartTime + time;
    }

    public bool CanActivateNow()
    {
        return (StartTime <= Time.time && Time.time < WaitToTime) == false;
    }
    public string GetNowCoolTimeStr()
    {
        return "" + (Mathf.Round((WaitToTime - Time.time) * 100) / 100f);
    }
    public float GetNowCoolTimePercent()
    {
        return (WaitToTime - Time.time) / time;
    }
}

