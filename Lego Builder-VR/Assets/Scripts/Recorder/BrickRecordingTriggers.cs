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
        Debug.Log("Entered recording area");
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "BuildArea")
        {
            return;
        }

        recorder.StopRecordingTransform();
        recorder.ClearRecording();
        Debug.Log("Left recording area");
    }

    private void Reset()
    {
        recorder = GetComponent<BrickTransformRecorder>();
        brick = GetComponent<LegoBrick>();
    }
}
