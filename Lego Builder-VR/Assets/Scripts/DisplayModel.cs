using Autohand;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DisplayModels : MonoBehaviour
{
    public Transform buildArea;
    public TMPro.TMP_Text title;
    public TMPro.TMP_Text pieces;
    public TMPro.TMP_Text difficulty;
    public TMPro.TMP_Text time;
    public PhysicsGadgetButton nextModel;
    public PhysicsGadgetButton previousModel;
    public Button loadButton;


    private Dictionary<int, GameObject> idToBrickGameObject = new Dictionary<int, GameObject>();

    private List<string> files = new List<string>();
    private int currentFileIndex = 0;
    private Recording current;

    private void Start()
    {
    }

    private void OnEnable()
    {
        loadButton.interactable = false;
        FetchAllFiles();
        if (files.Count > 0)
        {
            nextModel.OnPressed.AddListener(ShowNext);
            previousModel.OnPressed.AddListener(ShowPrevious);
            DisplayModel(currentFileIndex);
            loadButton.interactable = true;
        }
        else
        {
            Debug.LogError("No Recordings found.");
        }
    }

    private void OnDisable()
    {
        DestroyModel();
        nextModel.OnPressed.RemoveListener(ShowNext);
        previousModel.OnPressed.RemoveListener(ShowPrevious);
    }

    // TODO fetch data from internet.
    public void FetchAllFiles()
    {
        files.Clear();
        if (Directory.Exists(Path.Combine(Application.persistentDataPath, GameManager.storageFolder))){
            string folderPath = Path.Combine(Application.persistentDataPath, GameManager.storageFolder);
            var info = new DirectoryInfo(folderPath);
            var fileInfo = info.GetFiles("*.json");
            foreach (var file in fileInfo)
            {
                files.Add(Path.Combine(folderPath, file.FullName));
            }
        }
    }

    public void DisplayModel(int fileIndex)
    {
        string json = File.ReadAllText(files[fileIndex]);
        current = Recording.FromJson(json);
        DisplayRecordedModel(current);
    }

    private void DisplayRecordedModel(Recording recording)
    {
        current = recording;
        // Destroy old model
        DestroyModel();

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
        difficulty.text = recording.difficulty.ToString();

        TimeSpan t = TimeSpan.FromSeconds(recording.time);

        string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                       t.Hours,
                       t.Minutes,
                       t.Seconds);
        time.text = answer;
    }

    public void DestroyModel()
    {
        foreach (GameObject go in idToBrickGameObject.Values)
        {
            Destroy(go);
        }
        idToBrickGameObject.Clear();
    }

    public void ShowNext()
    {
        FetchAllFiles();
        currentFileIndex = currentFileIndex >= files.Count - 1 ? 0 : currentFileIndex + 1;

        DisplayModel(currentFileIndex);
    }

    public void ShowPrevious()
    {
        FetchAllFiles();
        currentFileIndex = currentFileIndex <= 0 ? files.Count - 1 : currentFileIndex-1;
        DisplayModel(currentFileIndex);
    }

    public void LoadCurrent()
    {
        GameManager.Instance.LoadBuild(current);
    }
}
