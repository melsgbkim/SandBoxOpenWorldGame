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
            case "cube_00000001": return "Texture/CubeGrass"; 
            case "cube_00000002": return "Texture/CubeDirt"; 
            case "cube_00000000": return "Texture/CubeNone"; 
        }
        return "";
    }
}