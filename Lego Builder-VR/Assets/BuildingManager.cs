using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(BoxCollider))]
public class BuildingManager : MonoBehaviour
{
    public BuildArea buildArea;

    Dictionary<LegoBrick, LegoBrick> actualToTransparent = new Dictionary<LegoBrick, LegoBrick>(); 

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("UnplacedBricks") && other.gameObject.TryGetComponent(out LegoBrick brick))
        {
            Debug.Log($"Brick Entered Area {other.name}");
            SpawnTransparentForBrick(brick);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("UnplacedBricks") && other.TryGetComponent(out LegoBrick brick))
        {
            DestroyTransparentOfBrick(brick);
        }
    }

    /// <summary>
    /// Spawns a transparent brick to follow the initial brick
    /// </summary>
    /// <param name="brick">The initial Brick</param>
    private void SpawnTransparentForBrick(LegoBrick brick)
    {
        if(actualToTransparent.ContainsKey(brick))
        {
            return;
        }
        LegoBrick brickTrans = LegoBrick.Create(brick.type, LegoBrick.BrickColorToTransparent(brick.color)).GetComponent<LegoBrick>();
        brickTrans.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Holograms"));
        actualToTransparent.Add(brick, brickTrans);
    }

    private void DestroyTransparentOfBrick(LegoBrick brick)
    {
        Destroy(actualToTransparent[brick].gameObject);
        actualToTransparent.Remove(brick);
    }

    private void SetActiveOfTransparentForBrick(LegoBrick brick, bool active)
    {
        if (actualToTransparent[brick].gameObject.activeSelf != active)
            actualToTransparent[brick].gameObject.SetActive(active);
    }


    List<LegoBrick> toRemove= new List<LegoBrick>(1);
    
    public void Update()
    {
        foreach(var pair in actualToTransparent)
        {
            // Check if brick is at least facing up as it should
            if(Vector3.Dot(pair.Key.transform.up, Vector3.up) <= 0)
            {
                SetActiveOfTransparentForBrick(pair.Key, false);
            }
            else
            {
                buildArea.GetClosestAlignedBrickPosition(pair.Key, out Vector3 pos, out Quaternion rot);
                pair.Value.transform.position = buildArea.transform.TransformPoint(pos);
                pair.Value.transform.rotation = buildArea.transform.rotation * rot;

                SetActiveOfTransparentForBrick(pair.Key, false);

                if (buildArea.IsBrickSupported(pair.Value))
                {
                    SetActiveOfTransparentForBrick(pair.Key, true);

                    if (buildArea.DoesBrickFit(pair.Value))
                    {
                        pair.Value.EnableErrorColor(false);

                        if (Input.GetMouseButtonDown(0) && buildArea.AllocatePosition(pair.Value))
                        {
                            pair.Key.transform.position = pair.Value.transform.position;
                            pair.Key.transform.rotation = pair.Value.transform.rotation;

                            toRemove.Add(pair.Key);

                        }
                    }
                    else
                    {
                        pair.Value.EnableErrorColor(true);
                    }

                }
                else
                {
                    SetActiveOfTransparentForBrick(pair.Key, false);
                }
            }
        }

        foreach(var brick in toRemove)
        {
            brick.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Bricks"));
            DestroyTransparentOfBrick(brick);
            actualToTransparent.Remove(brick);
        }
        toRemove.Clear();
    }


    public void DebugDrawCells(LegoBrick brick)
    {
        buildArea.GetBrickCells(brick, out Vector3Int startCell, out Vector3Int endCell);

        Debug.Log($"{startCell} : {endCell}");

        Gizmos.matrix = transform.localToWorldMatrix;

        for (int x = Mathf.Min(startCell.x, endCell.x); x < Mathf.Max(startCell.x, endCell.x); x++)
        {
            for (int y = Mathf.Min(startCell.y, endCell.y); y < Mathf.Max(startCell.y, endCell.y); y++)
            {
                for (int z = Mathf.Min(startCell.z, endCell.z); z < Mathf.Max(startCell.z, endCell.z); z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    DrawBox(Vector3.Scale(pos, BuildArea.cellDimensions) + BuildArea.cellDimensions / 2f, Quaternion.identity, BuildArea.cellDimensions, buildArea.transform.localToWorldMatrix, Color.red);
                }
            }
        }

    }

    public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Matrix4x4 mat, Color c)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        m = mat * m;

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, c);
        Debug.DrawLine(point2, point3, c);
        Debug.DrawLine(point3, point4, c);
        Debug.DrawLine(point4, point1, c);

        Debug.DrawLine(point5, point6, c);
        Debug.DrawLine(point6, point7, c);
        Debug.DrawLine(point7, point8, c);
        Debug.DrawLine(point8, point5, c);

        Debug.DrawLine(point1, point5, c);
        Debug.DrawLine(point2, point6, c);
        Debug.DrawLine(point3, point7, c);
        Debug.DrawLine(point4, point8, c);

        //// optional axis display
        //Debug.DrawRay(m.GetPosition(), m.GetForward(), Color.magenta);
        //Debug.DrawRay(m.GetPosition(), m.GetUp(), Color.yellow);
        //Debug.DrawRay(m.GetPosition(), m.GetRight(), Color.red);
    }

}
