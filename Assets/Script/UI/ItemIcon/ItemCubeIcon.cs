using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCubeIcon : MonoBehaviour
{
    public List<RawImage> list;
    public void SetCubeType(string type)
    {
        Texture txt = TextureManager.Load(PathManager.CubeTexturePath(type));
        list[0].texture = txt;
        list[1].texture = txt;
        list[2].texture = txt;
    }
    public bool enabled
    {
        set
        {
            list[0].enabled = value;
            list[1].enabled = value;
            list[2].enabled = value;
        }
    }
}