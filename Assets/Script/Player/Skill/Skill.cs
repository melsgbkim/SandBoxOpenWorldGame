using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
	public USEPOINT UsePoint = delegate () { };

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



