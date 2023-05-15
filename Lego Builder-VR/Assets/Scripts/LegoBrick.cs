using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LegoBrick;

public class LegoBrick : MonoBehaviour
{
    [System.Serializable]
    public enum BrickType
    {
        B1x1,
        B1x2,
        B1x3,
        B1x4,
        B1x6,
        B1x8,
        B2x2,
        B2x3,
        B2x4,
        B2x6,
        B2x8,
    }

    [System.Serializable]
    public enum BrickColor
    {
        Black,
        Blue,
        Green,
        DarkTurqoise,
        Red,
        DarkPink,
        Brown,
        LightGray,
        Yellow,
        Lime
    }

    public static GameObject Create(BrickType type, BrickColor color)
    {
        GameObject go = Instantiate(Resources.Load(BrickTypeToPrefabPath(type)) as GameObject);

        MeshRenderer rend;
        if(go && (rend = go.GetComponentInChildren<MeshRenderer>()))
        {
            rend.material = BrickColorToMaterial(color);
        }

        return go;
    }

    public static Material BrickColorToMaterial(BrickColor color)
    {
        string colorString = color.ToString();
        return Resources.Load($"Materials/Bricks/{colorString}") as Material;
    }

    public static string BrickTypeToPrefabPath(BrickType type)
    {
        string t = type.ToString();

        string brickType = t.Substring(1);

        return $"Prefabs/Bricks/{brickType}_Brick";
    }
   
}
