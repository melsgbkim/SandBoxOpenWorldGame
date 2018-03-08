using UnityEngine;
using UnityEditor;

public class PathManager
{
    public const string iconPath = "ItemIcon/";
    public const string previewPath = "prefab/DropItemPreview/";

    public static string CubeTexturePath(Cube.TYPE type)
    {
        switch (type)
        {
            case Cube.TYPE.Grass: return "Texture/CubeGrass"; 
            case Cube.TYPE.Dirt: return "Texture/CubeDirt"; 
            case Cube.TYPE.Air: return "Texture/CubeNone"; 
        }
        return "";
    }
}