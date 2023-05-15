using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildArea : MonoBehaviour
{
    public bool gizmos = false;

    public Vector3Int numberCells  = new Vector3Int( 48, 50, 48);
    public static Vector3 cellDimensions = new Vector3(.8f, .96f, .8f);



    public void OnDrawGizmos()
    {
        if (!gizmos)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.blue;
        for (int i= 0; i < numberCells.x; i++)
        {
            for(int j= 0; j < numberCells.y; j++)
            {
                for (int k= 0; k < numberCells.z; k++)
                {
                    Vector3Int pos = new Vector3Int(i, j, k);

                    Gizmos.DrawWireCube(Vector3.Scale(pos, cellDimensions) + cellDimensions/2f, cellDimensions);

                }
            }
        }
    }
    
    /// <summary>
    /// Gets the closest valid position in which the givn brick can be placed.
    /// In order for a position to be valid it must have enough free space to fit the brick and the brick be connected on top or bottom with a neighboring brick
    /// </summary>
    /// <param name="brick"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="maxCheckDistance"></param>
    /// <returns></returns>
    public bool GetClosestValidBrickPosition(Vector3 brickCellUnitDimensions, out Vector3 startPos, out Vector3 endPos, int maxCheckDistance= 2)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the dimensions needed for this brick in the grid, based on the xy orientation of it.
    /// </summary>
    /// <param name="brick"></param>
    /// <returns></returns>
    private Vector3Int GetBrickCellUnitDimensions(LegoBrick brick)
    {
        throw new NotImplementedException(); 
    }


}
