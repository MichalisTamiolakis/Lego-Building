using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DisplayModel : MonoBehaviour
{
    public string fileName;
    public Transform buildArea;
    public TMPro.TMP_Text title;
    public TMPro.TMP_Text pieces;
    public TMPro.TMP_Text time;

    private Dictionary<int, GameObject> idToBrickGameObject = new Dictionary<int, GameObject>();

    private void Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath)) //Problematic part 
        {
            string dataAsJson = File.ReadAllText(filePath);
            DisplayRecordedModel(Recording.FromJson(dataAsJson));
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    public void DisplayRecordedModel(Recording recording)
    {
        // Destroy old model
        foreach(GameObject go in idToBrickGameObject.Values)
        {
            Destroy(go);
        }
        idToBrickGameObject.Clear();

        // Create new one
        foreach (Command c in recording.commands)
        {
            switch (c.type)
            {
                case Command.CommandType.RequireBrick:
                    GameObject go = LegoBrick.CreateKinematic(c.brickType, c.brickColor);
                    idToBrickGameObject[c.blockID] = go;
                    break;
                case Command.CommandType.MoveBrick:
                    if(c.frames.Count > 0)
                    {
                        GameObject go1 = idToBrickGameObject[c.blockID];
                        go1.transform.position = c.frames[c.frames.Count-1].position;
                        go1.transform.rotation = c.frames[c.frames.Count - 1].rotation;
                        go1.transform.SetParent(buildArea, false);
                    }
                    break;
            }
        }

        // Show Info with UI
        title.text = recording.name;
        pieces.text = $"{recording.pieces}";

        TimeSpan t = TimeSpan.FromSeconds(recording.time);

        string answer = string.Format("{0:D2}h:{1:D2}m",
                        t.Hours,
                        t.Minutes);
        time.text = answer;
    }
}
