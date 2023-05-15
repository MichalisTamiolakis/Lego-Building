using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public int blockID;
    public List<AnimationFrame> frames;
}


public class AnimationFrame
{
    public Vector3 position;
    public Quaternion rotation;
    public float timestamp;
}