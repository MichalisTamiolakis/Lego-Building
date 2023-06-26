using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    public Recording recording { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance!= this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void CreateNewRecording(string name)
    {
        recording = new Recording(name);
    }

    [ContextMenu("Export Recording")]
    public void ExportRecording()
    {
        string jsonString = Recording.ToJson(recording);

        string path = Application.dataPath + "/Recordings/" + recording.name + ".json";
        if(File.Exists(path))
        {
            Debug.LogError("File already exists!");
            return;
        }

        File.WriteAllText(path, jsonString);
    }

}
