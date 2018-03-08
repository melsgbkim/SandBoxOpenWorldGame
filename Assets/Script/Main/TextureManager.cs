using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager
{
    static Hashtable TextureTable = new Hashtable();
    public static Texture Load(string path)
    {
        if (TextureTable.Contains(path) == false)
        {
            Texture tmp = Resources.Load(path) as Texture;
            if (tmp == null)
            {
                MonoBehaviour.print(path + " is Not Image or notting this path");
                return null;
            }
            else
                TextureTable.Add(path, tmp);
        }
        return TextureTable[path] as Texture;
    }

}