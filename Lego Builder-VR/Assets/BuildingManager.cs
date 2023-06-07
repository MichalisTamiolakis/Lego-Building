using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BuildingManager : MonoBehaviour
{
    public BuildArea buildArea;

    Dictionary<LegoBrick, LegoBrick> actualToTransparent = new Dictionary<LegoBrick, LegoBrick>(); 

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"EnteredoBJECT {other.name}");
        if (other.gameObject.layer == LayerMask.NameToLayer("UnplacedBricks") && other.gameObject.TryGetComponent(out LegoBrick brick))
        {
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
}
