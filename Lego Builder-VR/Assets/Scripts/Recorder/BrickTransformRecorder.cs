using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LegoBrick))]
public class BrickTransformRecorder : MonoBehaviour
{
    public LegoBrick brick;
    public List<AnimationFrame> frames { get; private set; } = new List<AnimationFrame>(1000);
    private bool isRecording = false;
    private float frameTime = 0f;

    private const float minRecordDistance = 0.01f;
    private const float minRecordAngle = 2f;

    public void ClearRecording()
    {
        frames.Clear();
        frameTime = 0f;
    }

    public void StartRecordingTransform()
    {
        AddCurrentTransformFrame(frameTime);
        isRecording = true;
    }

    public void StopRecordingTransform()
    {
        AddCurrentTransformFrame(frameTime);
        isRecording = false;
    }

    private void AddCurrentTransformFrame(float frameTime)
    {
        frames.Add(new AnimationFrame(PositionToBuildAreaLocal(), RotationToBuildAreaLocal(), frameTime));
    }

    private Vector3 PositionToBuildAreaLocal()
    {
        return BuildArea.Instance.transform.InverseTransformPoint(transform.position);
    }

    private Quaternion RotationToBuildAreaLocal()
    {
        return transform.rotation* BuildArea.Instance.transform.rotation;
    }

    /// <summary>
    /// Has the transform from the given frame changed enough to record a new frame?
    /// </summary>
    /// <param name="previousFrame"></param>
    /// <returns></returns>
    private bool HasTransformChanged(AnimationFrame previousFrame)
    {
        return Vector3.Distance(previousFrame.position, PositionToBuildAreaLocal()) > minRecordDistance || Quaternion.Angle(previousFrame.rotation, RotationToBuildAreaLocal()) > minRecordAngle;
    }
    
    private void Update()
    {
        if (isRecording)
        {
            if (HasTransformChanged(frames[frames.Count - 1]))
            {
                AddCurrentTransformFrame(frameTime);
            }
            frameTime += Time.deltaTime;
        }
    }

    public void Reset()
    {
        brick = GetComponent<LegoBrick>();
    }

}
