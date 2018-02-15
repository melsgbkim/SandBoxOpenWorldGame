using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBasicAttack : Skill
{
	GameObject att;
	Vector3 Forword;
	float timeLimit;
	float nowtime;
	public SkillBasicAttack()
	{
		init("SkillIcon/skill_basic_attack");
		coolTime.time = 0.3f;
		timeLimit = 0.4f;
		nowtime = 0f;
		needPoint = 1f;
		HavePoint = CheckSp;
		UsePoint = UseSp;
		OnlyKeyDown = true;
	}

	public override void Activate()
	{
		Forword = Player.GetComponent<PlayerMove>().moveForword;
		att = MonoBehaviour.Instantiate((GameObject)Resources.Load("AttackRange/att"));
		att.GetComponent<AttackHp>().init(10, Player, timeLimit, delegate (Collider col) { });
		att.transform.position = Forword * 1f + Player.transform.position;
		att.transform.rotation = Quaternion.LookRotation(Forword);
		att.transform.localScale = new Vector3(2, 4, 2);
		nowtime = 0f;
	}

	public override void Update()
	{
		nowtime += Time.deltaTime;
		try
		{
			att.transform.position = Forword * (1f + nowtime*10) + Player.transform.position;
		}
		catch(MissingReferenceException e)
		{

		}
		
	}//update skill
}