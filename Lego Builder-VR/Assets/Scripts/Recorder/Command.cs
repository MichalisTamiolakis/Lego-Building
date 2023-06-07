using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Command
{
    public Command(CommandType type, int blockID, LegoBrick.BrickColor brickColor, LegoBrick.BrickType brickType)
    {
        this.type = type;
        this.blockID = blockID;
        this.brickColor = brickColor;
        this.brickType = brickType;
    }

    public Command(CommandType type, int blockID, List<AnimationFrame> frames)
    {
        this.type = type;
        this.blockID = blockID;
        this.frames = frames;
    }

    public enum CommandType
    {
        RequireBrick,
        MoveBrick,
        MoveHand
    }

    public CommandType type;

    /// <summary>
    /// The block this command refers to
    /// </summary>
    public int blockID;

    /// <summary>
    /// Used for Require Brick Command to describe the color of the brick
    /// </summary>
    public LegoBrick.BrickColor brickColor;
    /// <summary>
    /// Used for Require Brick Command to describe the type of the brick
    /// </summary>
    public LegoBrick.BrickType brickType;
   
    /// <summary>
    /// The animation frames, used for MoveBrick and MoveHand commands
    /// </summary>
    public List<AnimationFrame> frames;


    public static Command CreateRequireBrickCommand(int brickID, LegoBrick brick)
    {
        return new Command(CommandType.RequireBrick, brickID, brick.color, brick.type);
    }
}

[System.Serializable]
public class AnimationFrame
{
    /// <summary>
    /// The position of this frame
    /// </summary>
    public Vector3 position = Vector3.zero;
    /// <summary>
    /// The rotation of this frame
    /// </summary>
    public Quaternion rotation = Quaternion.identity;
    /// <summary>
    /// The frame timestamp in seconds
    /// </summary>
    public float timestamp = 0f;

    public AnimationFrame(Vector3 position, Quaternion rotation, float timestamp)
    {
        this.position = position;
        this.rotation = rotation;
        this.timestamp = timestamp;
    }
}