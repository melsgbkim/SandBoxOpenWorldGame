using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour {
    public GameObject skillSlot;
    public List<GameObject> skillList = new List<GameObject>();
    // Use this for initialization

    bool Initialized = false;
    public void Init()
    {
        if (Initialized == false)
        {
            int index = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    GameObject slot = Instantiate(skillSlot, Vector3.zero, Quaternion.identity);
                    slot.transform.SetParent(transform);
                    slot.transform.position = new Vector3((x - 2.5f) * 105 + 960, (y - 1f) * 105 + 210, 0);
                    slot.name = "Skill_" + (x + 1) + (y + 1);
                    skillList.Add(slot);
                }
            }
            Initialized = true;
        }
    }
    void Start()
    {
        Init();
        UIClick.add(gameObject, ClickUpdate);
    }

    public bool ClickUpdate(Vector2 pos, UIClick.ClickType type)
    {

        return false;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
