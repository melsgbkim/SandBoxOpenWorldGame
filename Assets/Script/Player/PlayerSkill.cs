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
        (SkillKeyTable[KeyCode.X] as GameObject).GetComponent<SkillSlot>().setSkill(new SkillMiningFront());
        (SkillKeyTable[KeyCode.S] as GameObject).GetComponent<SkillSlot>().setSkill(new SkillMiningDown());
        (SkillKeyTable[KeyCode.W] as GameObject).GetComponent<SkillSlot>().setSkill(new SkillMiningUp());

        
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

public class Skill
{
    public static GameObject Player;
    public Texture Icon = null;
    public CoolTime coolTime;
    public bool OnlyKeyDown = false;
    public bool isStopOtherAction = false;
    public SkillSlot slot = null;
    public float needPoint = 0f;
    public void init(string path)
    {
        Icon = (Texture)Resources.Load(path);
        coolTime = new CoolTime();
    }
    public virtual void Start()
    {
        coolTime.Start();
        Activate();
        UsePoint();
    }
    public delegate bool CHECKPOINT();
    public CHECKPOINT HavePoint = delegate ()
    {
        return true;
    };
    public delegate void USEPOINT();
    public USEPOINT UsePoint= delegate (){};

    public virtual bool SkillDesappearFromSlot() { return false; }
    public virtual string GetNowCoolTimeStr() { return coolTime.GetNowCoolTimeStr(); }
    public virtual bool CanActivateNow() { return coolTime.CanActivateNow(); }
    public virtual bool isActivated() { return false; }
    public virtual void Activate() { }//start skill
    public virtual void Update() { }//update skill
    public virtual void NewAction() { }
    public virtual void UpdateTexture(SkillSlot ui)
    {
        ui.IconCoolTime.GetComponent<RawImage>().enabled = true;
        ui.Text.GetComponent<Text>().enabled = true;
        ui.Icon.GetComponent<RawImage>().texture = Icon;
    }


    public bool CheckHp()
    {
        PlayerHpStat stat = Player.GetComponent<PlayerHpStat>();
        if (stat == null) return false;
        return stat.CanChange(-needPoint);
    }
    public bool CheckSp()
    {
        PlayerSpStat stat = Player.GetComponent<PlayerSpStat>();
        if (stat == null) return false;
        return stat.CanChange(-needPoint);
    }
    public bool CheckMp()
    {
        PlayerMpStat stat = Player.GetComponent<PlayerMpStat>();
        if (stat == null) return false;
        return stat.CanChange(-needPoint);
    }

    public void UseHp()
    {
        PlayerHpStat stat = Player.GetComponent<PlayerHpStat>();
        stat.Decrease(needPoint);
    }
    public void UseSp()
    {
        PlayerSpStat stat = Player.GetComponent<PlayerSpStat>();
        stat.Decrease(needPoint);
    }
    public void UseMp()
    {
        PlayerMpStat stat = Player.GetComponent<PlayerMpStat>();
        stat.Decrease(needPoint);
    }
}

public class SkillMiningFront : Skill
{
    public SkillMiningFront()
    {
        init("SkillIcon/skill_mining_front");
        coolTime.time = 1 / 60f * 1f;
        needPoint = 1f;
        HavePoint = CheckSp;
        UsePoint = UseSp;

    }

    public override void Activate()
    {
        PlayerMining mining = Player.GetComponent<PlayerMining>();
        if (mining)
        {
            mining.Mining(0, new Vector3(0.5f, 0.9f, 0.05f), new Vector3(0, 0, 0) / 3f);
        }
    }
}

public class SkillMiningUp : Skill
{
    public SkillMiningUp()
    {
        init("SkillIcon/skill_mining_Up");
        coolTime.time = 1 / 60f * 3f;
        needPoint = 3f;
        HavePoint = CheckSp;
        UsePoint = UseSp;
    }
    public override void Activate()
    {
        PlayerMining mining = Player.GetComponent<PlayerMining>();
        if (mining)
        {
            mining.Mining(-45, new Vector3(0.5f, 1.3f, 0.05f), new Vector3(0, 0.6f, 0) / 3f);
        }
    }
}

public class SkillMiningDown : Skill
{
    public SkillMiningDown()
    {
        init("SkillIcon/skill_mining_Down");
        coolTime.time = 1 / 60f * 3f;
        needPoint = 3f;
        HavePoint = CheckSp;
        UsePoint = UseSp;
    }
    public override void Activate()
    {
        PlayerMining mining = Player.GetComponent<PlayerMining>();
        if (mining)
        {
            mining.Mining(45, new Vector3(0.5f, 1.1f, 0.05f), new Vector3(0, 3f, 0) / 3f);
        }
    }
}

public class SkillUseItem : Skill
{
    Item item;
    public Texture ItemIcon = null;
    public SkillUseItem(Item i)
    {
        OnlyKeyDown = true;
        if (i as ItemCube != null)
            OnlyKeyDown = false;
        isStopOtherAction = true;
        item = i;
        init(item.iconPath);
        coolTime.time = 0.5f;
    }

    public override bool SkillDesappearFromSlot()
    {
        return item.ItemWillDelete();
    }

    public override void Start()
    {
        Activate();
    }

    public override void Activate()
    {
        if(item as ItemStackable != null)
        {
            ItemStackable i = item as ItemStackable;
            i.ItemUse(1f);
        }
    }

    public override void NewAction()
    {
        Activate();
    }

    public override void Update()
    {
        if(item.isItemUseEnd() == false)
            item.ItemUpdate(Player);
        if(coolTime.CanActivateNow() && item.isItemUseEnd())
        {
            coolTime.Start();
        }


    }//update skill

    public override string GetNowCoolTimeStr()
    {
        if (item.isItemUseEnd() == false)
            return "";
        else
            return coolTime.GetNowCoolTimeStr();
    }

    public override bool CanActivateNow()
    {
        if (item.isItemUseEnd())
            return coolTime.CanActivateNow();
        else
            return false;
    }

    public override void UpdateTexture(SkillSlot ui)
    {
        ui.IconCoolTime.GetComponent<RawImage>().enabled = true;
        ui.Text.GetComponent<Text>().enabled = true;
        ui.IconOther.GetComponent<RawImage>().enabled = true;

        ui.Icon.GetComponent<RawImage>().texture = Icon;
        //ui.IconOther.GetComponent<RawImage>().texture = SkillSlot.SkillNone;
    }
}