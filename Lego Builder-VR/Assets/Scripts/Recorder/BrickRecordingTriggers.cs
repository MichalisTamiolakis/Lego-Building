using UnityEngine;
using System.Collections;

public class BrickRecordingTriggers:MonoBehaviour
{
    public BrickTransformRecorder recorder;
    public LegoBrick brick;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "BuildArea")
        {
            return;
        }   

        recorder.ClearRecording();
        recorder.StartRecordingTransform();
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "BuildArea")
        {
            return;
        }

        recorder.StopRecordingTransform();
        recorder.ClearRecording();
    }

    private void Reset()
    {
        recorder = GetComponent<BrickTransformRecorder>();
        brick = GetComponent<LegoBrick>();
    }
}
