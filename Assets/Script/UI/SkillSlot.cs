using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour {
    public GameObject Icon;
    public GameObject IconCoolTime;
    public GameObject Text;
    public GameObject IconOther;
    public static Texture SkillNone = null;
    public Skill skill = null;
    public ActionSlot slot = null;
    public KeyCode key;

    // Use this for initialization
    void Start () {
        IconCoolTime.active = false;
        Text.active = false;
        slot = GetComponent<ActionSlot>();
        slot.updateFromOther = UpdateFromSlot;
        slot.target = skill;
        
        UIClick.add(slot);
        if(SkillNone == null) SkillNone = (Texture)Resources.Load("SkillIcon/skill_None");
    }

    public void UpdateFromSlot()
    {
        ActionSlot slot = GetComponent<ActionSlot>();
        Skill target = slot.target as Skill;
        if(target == null)
        {
            if (slot.target as Item != null) target = new SkillUseItem(slot.target as Item);
        }
        if (skill != (target))
        {
            setSkill(target);
        }
        
    }

    public void Init()
    {
        if(skill != null)
        {
            skill.UpdateTexture(this);
            print(skill.Icon);
        }
        else
        {
            IconCoolTime.GetComponent<RawImage>().enabled = false;
            Text.GetComponent<Text>().enabled = false;
            IconOther.GetComponent<RawImage>().enabled = false;
            Icon.GetComponent<RawImage>().texture = SkillNone;
        }
    }

    public void setSkill(Skill s)
    {
        skill = s;
        slot = GetComponent<ActionSlot>();
        slot.target = skill;
        if(skill != null)
            skill.slot = this;
        Init();
    }
	
	// Update is called once per frame
	void Update () {
        if (skill != null)
        {
            if (skill.CanActivateNow())
            {
                if (IconCoolTime.active == true) IconCoolTime.active = false;
                if (Text.active == true) Text.active = false;
                Icon.GetComponent<RawImage>().color = Color.white;
            }
            else
            {
                if (IconCoolTime.active == false) IconCoolTime.active = true;
                if (Text.active == false) Text.active = true;
                Text.GetComponent<Text>().text = skill.GetNowCoolTimeStr();
                Icon.GetComponent<RawImage>().color = Color.gray;
            }
        }
        else
        {
            if (IconCoolTime.active == true) IconCoolTime.active = false;
            if (Text.active == true) Text.active = false;
            Icon.GetComponent<RawImage>().color = Color.white;
        }
	}
}
