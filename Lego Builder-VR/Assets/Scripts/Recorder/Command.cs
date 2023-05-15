using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Command
{
    public enum CommandType
    {
        RequireBrick,
        MoveBrick,
        MoveHand
    }

    public LegoBrick.BrickColor brickColor;
    public LegoBrick.BrickType brickType;
   
    public int blockID;
    public List<AnimationFrame> frames;
}

[System.Serializable]
public class AnimationFrame
{
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public float timestamp = 0f;
}