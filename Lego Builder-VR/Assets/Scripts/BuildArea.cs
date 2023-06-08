using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildArea : MonoBehaviour
{
    public static BuildArea Instance { get; private set; }

    public void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        cells = new Cell[numberCells.x, numberCells.y, numberCells.z];

        for(int i= 0; i < numberCells.x; i++)
        {
            for (int j = 0; j < numberCells.y; j++)
            {
                for (int k = 0; k < numberCells.z; k++)
                {
                    cells[i, j, k] = new Cell();
                }
            }
        }   
    }

    private void Start()
    {
        LegoBrick.Create(LegoBrick.BrickType.B2x2, LegoBrick.BrickColor.DarkTurquoise).GetComponent<LegoBrick>();
    }

    public bool gizmos = false;

    public Vector3Int numberCells  = new Vector3Int( 48, 50, 48);
    public static Vector3 cellDimensions = new Vector3(.008f * 4f, .0096f * 4f, .008f * 4f);

    public Vector3 Dimensions
    {
        get=> Vector3.Scale(cellDimensions, numberCells);
    }

    public Cell[,,] cells;

    public void OnDrawGizmos()
    {
        if (!gizmos)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i= 0; i < numberCells.x; i++)
        {
            for(int j= 0; j < numberCells.y; j++)
            {
                for (int k= 0; k < numberCells.z; k++)
                {
                    Vector3Int pos = new Vector3Int(i, j, k);

                    if(cells == null || cells[i, j, k].isEmpty)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireCube(Vector3.Scale(pos, cellDimensions) + cellDimensions/2f, cellDimensions);
                    }
                    else
                    {
                        Gizmos.color = new Color(1f, 0f, 0f, .3f);
                        Gizmos.DrawCube(Vector3.Scale(pos, cellDimensions) + cellDimensions/2f, cellDimensions);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the closest valid position in which the given brick can be placed.
    /// In order for a position to be valid it must have enough free space to fit the brick and the brick be connected on top or bottom with a neighboring brick
    /// </summary>
    /// <param name="brick"></param>
    /// <param name="position">The closest aligned position</param>
    /// <param name="rotation">The closest aligned rotation</param>
    /// <returns></returns>
    public void GetClosestAlignedBrickPosition(LegoBrick brick, out Vector3 position, out Quaternion rotation)
    {
        Vector3 newForward = NearestWorldAxis(brick.transform.forward);
        rotation = Quaternion.LookRotation(newForward, Vector3.up);

        Vector3 brickCellCenter = transform.InverseTransformPoint(brick.transform.position);
        position = GetClosestVertexPosition(brickCellCenter);
    }

    /// <summary>
    /// Does brick fit in the given position and rotation? (Is the area there empty?)
    /// </summary>
    /// <param name="brick"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public bool DoesBrickFit(LegoBrick brick)
    {
        GetBrickCells(brick, out Vector3Int startCell, out Vector3Int endCell);

        for (int x = Mathf.Min(startCell.x, endCell.x); x< Mathf.Max(startCell.x, endCell.x); x++)
        {
            for(int y = Mathf.Min(startCell.y, endCell.y); y< Mathf.Max(startCell.y, endCell.y); y++)
            {
                for(int z = Mathf.Min(startCell.z, endCell.z); z< Mathf.Max(startCell.z, endCell.z); z++)
                {
                    if(!IsCellValid(new Vector3Int(x, y, z)))
                    {
                        return false;
                    }

                    if (!cells[x,y,z].isEmpty)
                    {
                        return false;
                    }
                }
            }   
        }

        return true;
    }

    /// <summary>
    /// Is the given brick in a position where it is supported?
    /// </summary>
    /// <param name="brick">The brick to check</param>
    /// <returns>True if it is supported, false otherwise</returns>
    public bool IsBrickSupported(LegoBrick brick)
    {
        GetBrickCells(brick, out Vector3Int startCell, out Vector3Int endCell);

        // Check if it is attached to any other brick in y direction
        for (int x = Mathf.Min(startCell.x, endCell.x); x < Mathf.Max(startCell.x, endCell.x); x++)
        {
            for (int z = Mathf.Min(startCell.z, endCell.z); z < Mathf.Max(startCell.z, endCell.z); z++)
            {
                Vector3Int pos = new Vector3Int(x, startCell.y, z);
                if (!IsCellValid(pos))
                    continue;

                if(startCell.y == 0) // If it is the bottom row allow connection
                {
                    return true;
                }

                if(startCell.y< numberCells.y-1) // If it is not the top row, check if it is connected to a brick on top
                {
                    if (!cells[x, startCell.y + 1, z].isEmpty)
                    {
                        return true;
                    }
                }
                if(startCell.y> 0) // If it is not the bottom row, check if it is connected to a brick on bottom
                {
                    if (!cells[x, startCell.y - 1, z].isEmpty)
                    {
                        return true;
                    }
                }
            }
        }

        return false;

    }

    public bool AllocatePosition(LegoBrick brick)
    {
        GetBrickCells(brick, out Vector3Int startCell, out Vector3Int endCell);

        for (int x = Mathf.Min(startCell.x, endCell.x); x < Mathf.Max(startCell.x, endCell.x); x++)
        {
            for (int y = Mathf.Min(startCell.y, endCell.y); y < Mathf.Max(startCell.y, endCell.y); y++)
            {
                for (int z = Mathf.Min(startCell.z, endCell.z); z < Mathf.Max(startCell.z, endCell.z); z++)
                {
                    if (!cells[x, y, z].isEmpty)
                    {
                        return false;
                    }
                    cells[x, y, z].isEmpty = false;
                }
            }
        }

        return true;
    }

    public void GetBrickCells(LegoBrick brick, out Vector3Int startCell, out Vector3Int endCell)
    {
        Vector3Int xAxis = Vector3Int.RoundToInt(brick.transform.rotation * new Vector3(brick.CellUnitDimensions.x, 0, 0));
        Vector3Int yAxis = Vector3Int.RoundToInt(brick.transform.rotation * new Vector3(0, brick.CellUnitDimensions.y, 0));
        Vector3Int zAxis = Vector3Int.RoundToInt(brick.transform.rotation * new Vector3(0, 0, brick.CellUnitDimensions.z));
        Vector3Int offset = xAxis+yAxis+zAxis;

        startCell = GetClosestCell(transform.InverseTransformPoint(brick.transform.TransformPoint(cellDimensions / 2f)));
        endCell = startCell + offset; // The sum of all 3 previous axis is the end cell

        if (offset.x < 0)
        {
            startCell += Vector3Int.right;
            endCell += Vector3Int.right;
        }

        if (offset.y < 0)
        {
            startCell += Vector3Int.up;
            endCell += Vector3Int.up;
        }

        if (offset.z < 0)
        {
            startCell += Vector3Int.forward;
            endCell += Vector3Int.forward;
        }

    }

    private Vector3 NearestWorldAxis(Vector3 v)
    {
        if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
        {
            v.x = 0;
            if (Mathf.Abs(v.y) < Mathf.Abs(v.z))
                v.y = 0;
            else
                v.z = 0;
        }
        else
        {
            v.y = 0;
            if (Mathf.Abs(v.x) < Mathf.Abs(v.z))
                v.x = 0;
            else
                v.z = 0;
        }
        return v;
    }
    

    /// <summary>
    /// Gets the cell in which the given point is located. If point is not inside any cell, returns the closest cell.
    /// </summary>
    /// <param name="point">The point in local coordinates</param>
    /// <returns>The cell in which the given point is located or the closest cell to it.</returns>
    public Vector3Int GetClosestCell(Vector3 point)
    {
        return  new Vector3Int(
                    Mathf.FloorToInt(Mathf.Clamp(point.x, 0f, Dimensions.x) / cellDimensions.x),
                    Mathf.FloorToInt(Mathf.Clamp(point.y, 0f, Dimensions.y) / cellDimensions.y),
                    Mathf.FloorToInt(Mathf.Clamp(point.z, 0f, Dimensions.z) / cellDimensions.z)
                );
    }

    /// <summary>
    /// Gets the closest cell vertex position to the given point.
    /// </summary>
    /// <param name="point">The point in local coordinates</param>
    /// <returns></returns>
    public Vector3 GetClosestVertexPosition(Vector3 point)
    {

        return Vector3.Scale(GetClosestVertex(point), cellDimensions);
    }

    /// <summary>
    /// Gets the closest cell vertex index at the position given.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3Int GetClosestVertex(Vector3 point)
    {
        return new Vector3Int(
                    Mathf.RoundToInt(Mathf.Clamp(point.x, 0f, Dimensions.x) / cellDimensions.x),
                    Mathf.RoundToInt(Mathf.Clamp(point.y, 0f, Dimensions.y) / cellDimensions.y),
                    Mathf.RoundToInt(Mathf.Clamp(point.z, 0f, Dimensions.z) / cellDimensions.z)
                );
    }

    /// <summary>
    /// Gets if the given point is inside a cell of the build area.
    /// </summary>
    /// <param name="point">The point in local coordinates</param>
    /// <returns></returns>
    public bool IsPointInsideBuildArea(Vector3 point)
    {
        return point.x >= 0 && point.x <= numberCells.x * cellDimensions.x &&
            point.y >= 0 && point.y <= numberCells.y * cellDimensions.y &&
            point.z >= 0 && point.z <= numberCells.z * cellDimensions.z;
    }

    /// <summary>
    /// Is the given cell coords inside the build area?
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public bool IsCellValid(Vector3Int cell)
    {
        return cell.x >= 0 && cell.x < numberCells.x &&
            cell.y >= 0 && cell.y < numberCells.y &&
            cell.z >= 0 && cell.z < numberCells.z;
    }
}

[SerializeField]
public class Cell
{
    public bool isEmpty = true;
}