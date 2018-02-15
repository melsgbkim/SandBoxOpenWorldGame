using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
		if (item as ItemStackable != null)
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
		if (item.isItemUseEnd() == false)
			item.ItemUpdate(Player);
		if (coolTime.CanActivateNow() && item.isItemUseEnd())
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