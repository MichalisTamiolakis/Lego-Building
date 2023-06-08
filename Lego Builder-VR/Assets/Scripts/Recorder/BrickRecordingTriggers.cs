using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BrickRecordingTriggers:MonoBehaviour
{
    public BrickTransformRecorder recorder;
    public LegoBrick brick;

    public void OnTriggerEnter(Collider other)
    {
        recorder.ClearRecording();
        recorder.StartRecordingTransform();
    }

    public void OnTriggerExit(Collider other)
    {
        recorder.StopRecordingTransform();
        recorder.ClearRecording();
    }

    [ContextMenu("Save Recording")]
    public void SaveRecording()
    {
        recorder.StopRecordingTransform();

        int brickID = Recorder.Instance.recording.GenerateBrickID();
        Recorder.Instance.recording.AddCommand(Command.CreateRequireBrickCommand(brickID, brick));
        Recorder.Instance.recording.AddCommand(new Command(Command.CommandType.MoveBrick, brickID, recorder.frames));
    }

    private void Reset()
    {
        recorder = GetComponent<BrickTransformRecorder>();
        brick = GetComponent<LegoBrick>();
    }
}
