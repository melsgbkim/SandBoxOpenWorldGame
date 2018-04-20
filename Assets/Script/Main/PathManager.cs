using UnityEngine;
using UnityEditor;

public class PathManager
{
    public const string iconPath = "ItemIcon/";
    public const string previewPath = "prefab/DropItemPreview/";

    public static string CubeTexturePath(string type)
    {
        switch (type)
        {
            case "item_cube_00000001": return "Texture/cube_grass"; 
            case "item_cube_00000002": return "Texture/cube_dirt"; 
            case "item_cube_00000000": return "Texture/CubeNone"; 
        }
        return "";
    }
}