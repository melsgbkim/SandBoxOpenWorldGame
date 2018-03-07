using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSetting : MonoBehaviour {
    public string ComponentCreateCommand = "";
    // Use this for initialization
    void Start () {
        LoadComponent(gameObject);
    }
	
    void LoadComponent(GameObject obj)
    {
        string[] commands = ComponentCreateCommand.Split(';');
        for(int i=0;i< commands.Length;i++)
        {
            string line = commands[i].Trim();
            string[] SplitedLine = line.Split('=');
            string command = SplitedLine[0];
            string value = "";
            if (SplitedLine.Length > 1)
                value = SplitedLine[1];
            print(command);
            switch(command)
            {
            case "":break;
            case "HpStat":
                    obj.AddComponent<HpStat>();
                    obj.GetComponent<HpStat>().Init(30);
                    break;
            case "Skeleton":
                    obj.AddComponent<SkeletonAnimationControl>();
                    obj.AddComponent<SkeletonControl>();
                    break;
            case "Tiger":
                    obj.AddComponent<TigerAnimationControl>();
                    obj.AddComponent<TigerControl>();
                    break;
            }
        }
        Destroy(this);
    }
}

/*public class MonsterSettingScript
{
    public string command;
    public string 

}*/