using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    public enum CommandType
    {
        RequireBrick,
        MoveBrick,
        MoveHand
    }

    public int blockID;
    public string brickType = "";
    public List<AnimationFrame> frames;
}


public class AnimationFrame
{
    public Vector3 position;
    public Quaternion rotation;
    public float timestamp;
}