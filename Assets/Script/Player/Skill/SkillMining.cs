using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMiningFront : Skill
{
	public SkillMiningFront()
	{
		init("SkillIcon/skill_mining_front");
        //coolTime.time = 1 / 60f * 1f;
        //needPoint = 1f;
        coolTime.time = 0f;
        needPoint = 0f;
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
        //coolTime.time = 1 / 60f * 3f;
        //needPoint = 3f;
        coolTime.time = 0f;
        needPoint = 0f;
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
        //coolTime.time = 1 / 60f * 3f;
        //needPoint = 3f;
        coolTime.time = 0f;
        needPoint = 0f;
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