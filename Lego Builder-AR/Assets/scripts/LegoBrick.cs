using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using static LegoBrick;

public class LegoBrick : MonoBehaviour
{
    [System.Serializable]
    public enum BrickType:int
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

    public BrickType type { get; private set; } = BrickType.B1x1;

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
        Lime,
        BlackTransparent,
        BlueTransparent,
        GreenTransparent,
        DarkTurqoiseTransparent,
        RedTransparent,
        DarkPinkTransparent,
        BrownTransparent,
        LightGrayTransparent,
        YellowTransparent,
        LimeTransparent,
        ErrorColor
    }

    public BrickColor color
    {
        get=>_color;
        set {
            _color = value;

            MeshRenderer rend;
            if (rend = GetComponentInChildren<MeshRenderer>())
            {
                rend.material = BrickColorToMaterial(color);
            }
        }
    }

    [SerializeField]
    private BrickColor _color;

    private Vector3Int[] typeToCells = new Vector3Int[]{
        new Vector3Int(1,1,1),
        new Vector3Int(2,1,1),
        new Vector3Int(3,1,1),
        new Vector3Int(4,1,1),
        new Vector3Int(6,1,1),
        new Vector3Int(8,1,1),
        new Vector3Int(2,1,2),
        new Vector3Int(3,1,2),
        new Vector3Int(4,1,2),
        new Vector3Int(6,1,2),
        new Vector3Int(8,1,2),
    };

    public Vector3Int CellUnitDimensions
    {
        get => typeToCells[(int)type];
    }

    //public Vector3 Dimensions
    //{
    //    get => Vector3.Scale(BuildArea.cellDimensions, CellUnitDimensions);
    //}

    //public Vector3 Center
    //{
    //    get => Dimensions/2f;
    //}

    public void EnableErrorColor(bool enable)
    {
        MeshRenderer rend;
        if (rend = GetComponentInChildren<MeshRenderer>())
        {
            rend.material = enable ? BrickColorToMaterial(BrickColor.ErrorColor) : BrickColorToMaterial(color);
        }
    } 

    public static GameObject Create(BrickType type, BrickColor color)
    {
        GameObject go = Instantiate(Resources.Load(BrickTypeToPrefabPath(type)) as GameObject);

        LegoBrick brick = go.GetComponent<LegoBrick>() ?? go.AddComponent<LegoBrick>();
        brick.color = color;
        brick.type = type;

        return go;
    }

    public static Material BrickColorToMaterial(BrickColor color)
    {
        string colorString = color.ToString();
        return Resources.Load($"Materials/Bricks/{colorString}") as Material;
    }

    public static BrickColor BrickColorToTransparent(BrickColor color)
    {
        Debug.Assert((int)color <= 9);
        BrickColor transColor = (BrickColor)((int)color + 10);
        return transColor;
    }

    public static string BrickTypeToPrefabPath(BrickType type)
    {
        string t = type.ToString();

        string brickType = t.Substring(1);

        return $"Prefabs/Bricks/{brickType}_Brick";
    }


}
