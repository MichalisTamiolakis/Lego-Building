using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }
    public bool setActive = false;
    public static Recording r;
    public int step = 0;
    public static GameObject instantiatedPopup = null;
    public GameObject popup;
    //GameObject movingBrick = null;
    public Button nextButton;
    public Button previousButton;
    LegoBrick.BrickColor currentColor;
    LegoBrick.BrickType currentType;
    // Start is called before the first frame update

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        string path = Application.dataPath + "/json/TestRecording.json";

        string jsonString = File.ReadAllText(path);
        //nextButton = GetComponent<Button>();
        r = Recording.FromJson(jsonString);
        UpdateButtonInteractivity();
        DisplayCommand(GetCurrentCommand());
        CreateFinalModel();
    }

    public Command GetCurrentCommand()
    {
        return r.commands[step];
    }

    public Command GetNextCommand()
    {
        step++;
        Debug.Log("Step: " + step + ", Length: " + r.commands.Count);
        UpdateButtonInteractivity();
        return r.commands[step];
    }

    public Command GetPreviousCommand()
    {
        step--;
        UpdateButtonInteractivity();
        return r.commands[step];
    }

    public void IncreaseStep()
    {
        Command c = GetCurrentCommand();
        if (c.type == Command.CommandType.MoveBrick)
        {
            GameObject ret = Manager.Instance.GetBrick(c.blockID);
            if (ret != null)
            {
                AnimationExecution animation = Manager.Instance.GetBrick(c.blockID).GetComponent<AnimationExecution>();

                if (animation != null)
                {
                    Destroy(animation);
                }
            }
            
            GameObject brick = PlaceBrickToFinalPosition(c);
            brick.transform.SetParent(transform,false);
        }
        DisplayCommand(GetNextCommand());
    }

    public void DecreaseStep()
    {
        Command c = GetCurrentCommand();
        if (c.type == Command.CommandType.MoveBrick)
        {
            Destroy(Manager.Instance.GetBrick(c.blockID));
        }
        DisplayCommand(GetPreviousCommand());
    }

    private void UpdateButtonInteractivity()
    {
        Debug.Log("Step: " + step + ", Length: " + r.commands.Count);
        if (step >= r.commands.Count-1)
        {
            nextButton.interactable = false; // Disable the button
        }
        else
        {
            nextButton.interactable = true; // Enable the button
        }

        if(step == 0) // If we are at the first step
        {
            previousButton.interactable = false; // Disable the button
        }
        else
        {
            previousButton.interactable = true; // Enable the button
        }
    }


    public void DisplayCommand(Command c)
    {
        switch (c.type)
        {
            case Command.CommandType.RequireBrick:
                if(instantiatedPopup == null)
                {
                    Debug.Log("Require Brick");
                    currentColor = c.brickColor;
                    currentType = c.brickType;
                    GameObject brick = LegoBrick.Create(c.brickType, c.brickColor);
                    
                    MeshRenderer childMeshRenderer = brick.GetComponentInChildren<MeshRenderer>();
                    Vector3 offset = new Vector3(1f, 0.243f, 0.13f);
                    instantiatedPopup = Instantiate(popup, transform.position + offset, transform.rotation, transform);
                    GameObject brickContainer = instantiatedPopup.transform.Find("Panel/brickContainer/rotationPivot").gameObject;
                    brick.transform.SetParent(brickContainer.transform);
                    brick.transform.localPosition = Vector3.Scale(new Vector3(-0.008f/2f, -0.0096f/2f, -0.008f/2f), brick.GetComponent<LegoBrick>().CellUnitDimensions);
                }

                break;
            case Command.CommandType.MoveBrick:
                Debug.Log("Move Brick");
                if (instantiatedPopup != null)
                {
                    Destroy(instantiatedPopup);
                    instantiatedPopup = null;                    
                }
                GameObject block = Manager.Instance.GetBrick(c.blockID);
                if (block == null)
                {
                    GameObject movingBrick = LegoBrick.Create(currentType, LegoBrick.BrickColorToTransparent(currentColor));
                    AssignIdToBrick assignIdToBrick = movingBrick.AddComponent<AssignIdToBrick>();
                    assignIdToBrick.blockID = c.blockID;
                    assignIdToBrick.OnEnable();
                    movingBrick.transform.SetParent(transform, false);

                    AnimationExecution animation = movingBrick.AddComponent<AnimationExecution>();
                    animation.command = c;
                    animation.StartAnimation();

                }
                else
                {
                    AnimationExecution animation = block.AddComponent<AnimationExecution>();
                    animation.command = c;
                    animation.StartAnimation();
                }
                
                break;
            default:
                Debug.Log("Unknown Command");
                break;
        }
        
          
    }

    GameObject PlaceBrickToFinalPosition(Command c)
    {
        AnimationFrame lastFrame = c.frames[c.frames.Count - 1];
        GameObject brick = Manager.Instance.GetBrick(c.blockID);
        brick.transform.localPosition = lastFrame.position;
        brick.transform.localRotation = lastFrame.rotation;
        return brick;
    }

    public GameObject CreateFinalModel()
    {
        GameObject finalModel = new GameObject("Final Model");
        MeshRenderer renderer = finalModel.AddComponent<MeshRenderer>();
        // Find the scene with the name "BuildingScene"
        Scene targetScene = SceneManager.GetSceneByName("BuildingScene");

        // Check if the scene is valid and loaded
        if (targetScene.IsValid() && targetScene.isLoaded)
        {

            SceneManager.MoveGameObjectToScene(finalModel, targetScene);

            GameObject brick = null;
            foreach (Command c in r.commands)
            {
                
                if (c.type == Command.CommandType.RequireBrick)
                {
                    brick = LegoBrick.Create(c.brickType, c.brickColor);
                }
                else
                {
                    AnimationFrame lastFrame = c.frames[c.frames.Count - 1];
                    brick.transform.localPosition = lastFrame.position;
                    brick.transform.localRotation = lastFrame.rotation;
                    brick.transform.SetParent(finalModel.transform);
                }
            }
            finalModel.SetActive(setActive);
        }
        else
        {
            Debug.Log("Scene 'BuildingScene' is not valid or not loaded.");
        }
        return finalModel;
    }

    public void ShowFinalModel()
    {
        // Find the scene with the name "BuildingScene"
        Scene targetScene = SceneManager.GetSceneByName("BuildingScene");

        // Check if the scene is valid and loaded
        if (targetScene.IsValid() && targetScene.isLoaded)
        {
            // Get the root GameObjects of the scene
            GameObject[] rootObjects = targetScene.GetRootGameObjects();

            // Find the "BuildArea" GameObject in the scene
            GameObject buildArea = null;
            for (int i = 0; i < rootObjects.Length; i++)
            {
                if (rootObjects[i].name == "Final Model")
                {
                    buildArea = rootObjects[i];
                    break;
                }
            }

            // Check if the BuildArea GameObject is found
            if (buildArea != null)
            {
                setActive = !setActive;
                buildArea.SetActive(setActive);
            }
            else
            {
                Debug.LogWarning("Failed to add finalModel to the scene. BuildArea GameObject not found.");
            }
        }
        else
        {
            Debug.LogWarning("Scene 'BuildingScene' is not valid or not loaded.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

// x: 0.008, y: 0.0096, z: 0.008    1X1 
