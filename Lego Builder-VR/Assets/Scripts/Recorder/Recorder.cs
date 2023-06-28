using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEditor;

public class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    [field: SerializeField]
    private Recording recording;

    public RecordingInfoDisplay infoDisplay;

    public BuildArea buildArea;

    private Dictionary<int, LegoBrick> idToBrick = new Dictionary<int, LegoBrick>();
    private int largestID = int.MinValue;

    private Stack<Command> commandHistory = new Stack<Command>();

    private bool hasSaved = true;

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
        hasSaved = true;
        infoDisplay.save.interactable = false;

        string jsonString = Recording.ToJson(recording);

        string path = Path.Combine(Application.streamingAssetsPath, "Recordings", $"{recording.name}.json");
        if(File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllText(path, jsonString);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public void SetRecording(Recording r)
    {
        foreach (LegoBrick b in idToBrick.Values)
        {
            Destroy(b.gameObject);
        }

        this.recording = r;
    }


    [System.Serializable]
    public class RecordingInfoDisplay
    {
        public GameObject defaultUI;
        public GameObject exitNoSaveOptionsUI;
        public TMPro.TMP_InputField title;
        public Dropdown difficulty;
        public TMPro.TMP_Text pieces;
        public TMPro.TMP_Text time;
        public Button save;
        public Button exit;
        public Button exitNoSave;
        public Button cancelExit;
    }

    // Update display info every second
    private IEnumerator Start()
    {
        NonNativeKeyboard.Instance.OnTextSubmitted += (_, _) => 
        {
            if (NonNativeKeyboard.Instance.InputField.text != recording.name)
            {
                string newName = NonNativeKeyboard.Instance.InputField.text;
                recording.name = newName.Substring(0, Mathf.Min(newName.Length, 20));
                hasSaved = false;
                infoDisplay.save.interactable = true;
            }
        };
        
        infoDisplay.title.text = recording.name;
        infoDisplay.title.onSelect.AddListener((string val) =>
        {
            NonNativeKeyboard.Instance.PresentKeyboard(recording.name);
        });
        
        infoDisplay.difficulty.value = (int)recording.difficulty;
        infoDisplay.difficulty.onValueChanged.AddListener((int val) =>
        {
            recording.difficulty = (Recording.Difficulty)val;
            hasSaved = false;
            infoDisplay.save.interactable = true;
        });

        // UI logic
        infoDisplay.defaultUI.SetActive(true);
        infoDisplay.exitNoSaveOptionsUI.SetActive(false);
        infoDisplay.save.onClick.AddListener(() =>
        {
            ExportRecording();
        });
        infoDisplay.exit.onClick.AddListener(() =>
        {
            if (hasSaved)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Simply restart the scene
            }
            else
            {
                // Show exit without saving UI
                infoDisplay.defaultUI.SetActive(false);
                infoDisplay.exitNoSaveOptionsUI.SetActive(true);
            }
        });
        infoDisplay.exitNoSave.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Simply restart the scene
        });
        infoDisplay.cancelExit.onClick.AddListener(() =>
        {
            infoDisplay.defaultUI.SetActive(true);
            infoDisplay.exitNoSaveOptionsUI.SetActive(false);
        });

        while (true)
        {
            UpdateDisplayInfo();
            yield return new WaitForSeconds(.2f);
        }
    }

    private void UpdateDisplayInfo()
    {
        if (infoDisplay.title.text != recording.name)
        {
            infoDisplay.title.text = recording.name;
        }

        if (infoDisplay.difficulty.value != (int)recording.difficulty)
        {
            infoDisplay.difficulty.SetValueWithoutNotify((int)recording.difficulty);
        }

        infoDisplay.pieces.text = recording.pieces.ToString();

        TimeSpan t = TimeSpan.FromSeconds(recording.time);

        string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);

        infoDisplay.time.text = answer;
    }

    /// <summary>
    /// Adds a command to the current recording
    /// </summary>
    /// <param name="c">The command to add.</param>
    /// <param name="play">Wether to play the current command.</param>
    public void AddCommand(Command c, bool play=false)
    {
        hasSaved = false;
        infoDisplay.save.interactable = true;

        // Clear the command history
        commandHistory.Clear();

        AddCommandInternal(c, play);
    }

    private void AddCommandInternal(Command c, bool play = false)
    {

        recording.AddCommand(c);

        if (play)
            PlayCommand(c);
    }

    public int GenerateBlockID()
    {
        return ++largestID;
    }

    public void PlayRecording()
    {
        foreach (Command c in recording.commands)
        {
            PlayCommand(c);
        }
        hasSaved = true;
        infoDisplay.save.interactable = false;
    }
    
    public void PlayCommand(Command c)
    {
        switch (c.type)
        {
            case Command.CommandType.RequireBrick:
                // Spawn brick
                LegoBrick brick = LegoBrick.CreateKinematic(c.brickType, c.brickColor).GetComponent<LegoBrick>();
                idToBrick.Add(c.blockID, brick);
                if (largestID < c.blockID)
                {
                    largestID = c.blockID;
                }
                break;
            case Command.CommandType.MoveBrick:
                // Move brick
                if (c.frames.Count > 0)
                {
                    LegoBrick legoBrick = idToBrick[c.blockID];
                    GameObject go = legoBrick.gameObject;
                    go.transform.position = c.frames[c.frames.Count - 1].position;
                    go.transform.rotation = c.frames[c.frames.Count - 1].rotation;
                    go.transform.SetParent(buildArea.transform, false);
                    buildArea.AllocatePosition(legoBrick);
                }
                break;
        }
    }

    public void Undo()
    {
        hasSaved = false;
        infoDisplay.save.interactable = true;

        UndoInternal();
        UndoInternal();
    }

    public bool CanUndo()
    {
        return recording.commands.Count > 0;
    }

    private void UndoInternal()
    {
        if(!CanUndo())
        {
            return;
        }

        Command c = recording.commands[recording.commands.Count - 1];

        if (c.type == Command.CommandType.RequireBrick)
        {
            if(idToBrick.TryGetValue(c.blockID, out LegoBrick brick))
            {
                buildArea.DeallocatePosition(brick);
                idToBrick.Remove(c.blockID);
                Destroy(brick.gameObject);
            }
        }

        commandHistory.Push(c);

        recording.RemoveCommand(recording.commands.Count - 1);
    }

    public void Redo()
    {
        hasSaved = false;
        infoDisplay.save.interactable = true;

        RedoInternal();
        RedoInternal();
    }

    public bool CanRedo()
    {
        return commandHistory.Count > 0;
    }

    private void RedoInternal()
    {
        if(!CanRedo())
        {
            return;
        }

        Command c = commandHistory.Pop();
        AddCommandInternal(c, play: true);
    }

    private LegoBrick replayBrick = null;

    public void ReplayLastCommand()
    {
        if (!CanReplay())
            return;
        
        Command lastCommand = recording.commands[recording.commands.Count - 1];

        switch (lastCommand.type)
        {
            case Command.CommandType.MoveBrick:
                if(lastCommand.frames.Count <= 1)
                {
                    return;
                }
                LegoBrick brick = idToBrick[lastCommand.blockID];
                replayBrick = LegoBrick.CreateKinematic(brick.type, LegoBrick.BrickColorToTransparent(brick.color)).GetComponent<LegoBrick>();
                replayBrick.transform.SetParent(buildArea.transform, false);
                AnimationPlayer player = replayBrick.AddComponent<AnimationPlayer>();
                player.animation = lastCommand.frames;
                player.onLooped.AddListener(() => Destroy(replayBrick.gameObject));
                player.loop = false;
                player.StartAnimation();
                break;
        }
    }

    public bool CanReplay()
    {
        return replayBrick == null && recording.commands.Count > 0;
    }
}
