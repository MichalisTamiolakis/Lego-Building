using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }
    public bool setActive = false;
    public static Recording r;
    public int step = 0;
    public static GameObject instantiatedPopup = null;
    public GameObject popup;
    public GameObject finalModel;
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

        string fileData = "";
        string fileName = Path.Combine(Application.streamingAssetsPath, "TestRecording.json");
        byte[] bytes = UnityEngine.Windows.File.ReadAllBytes(fileName);
        fileData = System.Text.Encoding.ASCII.GetString(bytes);
        r = Recording.FromJson(fileData);


        /*string path = Application.dataPath + "/json/TestRecording.json";

        string jsonString = File.ReadAllText(path);
        //nextButton = GetComponent<Button>();
        r = Recording.FromJson(jsonString);*/
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
                //AnimationPlayer animation = Manager.Instance.GetBrick(c.blockID).GetComponent<AnimationPlayer>();

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
                    Vector3 targetScale = new Vector3(1f, 1f, 1f);
                    
                    MeshRenderer childMeshRenderer = brick.GetComponentInChildren<MeshRenderer>();
                    //Vector3 offset = new Vector3(1f, 0.243f, 0.13f);
                    //Vector3 offset = new Vector3(0f, 0f, 0f);
                    //instantiatedPopup = Instantiate(popup, transform.position + offset, transform.rotation, transform);
                    Vector3 position = new Vector3(1.109985f, 0.1700134f, 0.082f);
                    Quaternion rotation = Quaternion.identity;
                    instantiatedPopup = Instantiate(popup, transform);
                    Debug.Log("popup: " + instantiatedPopup);
                    GameObject brickContainer = instantiatedPopup.transform.Find("Panel/brickContainer/rotationPivot").gameObject;
                    Debug.Log("brickContainer: " + brickContainer);
                    brick.transform.SetParent(brickContainer.transform);
                    brick.transform.localScale = targetScale;
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
                    //AnimationPlayer player = movingBrick.AddComponent<AnimationPlayer>();
                    //player.animation = c.frames;
                    //player.StartAnimation();

                }
                else
                {
                    AnimationExecution animation = block.AddComponent<AnimationExecution>();
                    animation.command = c;
                    //AnimationPlayer player = block.AddComponent<AnimationPlayer>();
                    //player.animation = c.frames;
                    //player.StartAnimation();
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
        //GameObject finalModel = new GameObject("Final Model");
        GameObject instantiatedModel = Instantiate(finalModel, transform);        
        //instantiatedModel.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        instantiatedModel.transform.localPosition = new Vector3(0.449999988f, 0.0500000007f, -0.970000029f);
        //instantiatedModel.transform.localPosition = Vector3.Scale(new Vector3(-0.008f / 2f, -0.0096f / 2f, -0.008f / 2f), instantiatedModel.GetComponent<LegoBrick>().CellUnitDimensions);
        instantiatedModel.name = "FinalModel";
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
                brick.transform.SetParent(instantiatedModel.transform,false);
            }
        }
        finalModel.SetActive(setActive);


        /*MeshRenderer renderer = finalModel.AddComponent<MeshRenderer>();
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
        }*/
        return finalModel;
    }

    public void ShowFinalModel()
    {
        setActive = !setActive;
        GameObject model = this.transform.Find("FinalModel").gameObject;
        model.SetActive(setActive);
        // Find the scene with the name "BuildingScene"
        /*Scene targetScene = SceneManager.GetSceneByName("BuildingScene");

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
        }*/
    }

    // Update is called once per frame
    void Update()
    {

    }
}

// x: 0.008, y: 0.0096, z: 0.008    1X1 
