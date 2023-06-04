using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }

    public static Recording r;
    public int step = 0;
    public static GameObject instantiatedPopup = null;
    public GameObject popup;
    GameObject movingBrick = null;
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

        r = Recording.FromJson(jsonString);
        DisplayCommand(GetCurrentCommand());
    }

    public Command GetCurrentCommand()
    {
        return r.commands[step];
    }

    public Command GetNextCommand()
    {
        step++;
        return r.commands[step];
    }

    public Command GetPreviousCommand()
    {
        step--;
        return r.commands[step];
    }

    public void IncreaseStep()
    {
        DisplayCommand(GetNextCommand());
    }

    public void DecreaseStep()
    {
        DisplayCommand(GetPreviousCommand());
    }


    public void DisplayCommand(Command c)
    {
        //Command c = GetCurrentCommand();
        //Debug.Log("Before Switch:"+ c.type);
        switch (c.type)
        {
            case Command.CommandType.RequireBrick:
                if(movingBrick != null)
                {
                    Destroy(movingBrick);
                    movingBrick = null;
                }   
                if(instantiatedPopup == null)
                {
                    Debug.Log("Require Brick");
                    GameObject brick = LegoBrick.Create(c.brickType, c.brickColor);
                    MeshRenderer childMeshRenderer = brick.GetComponentInChildren<MeshRenderer>();
                    if (childMeshRenderer != null)
                    {
                        Material childMaterial = childMeshRenderer.material;
                        Color childColor = childMaterial.color;
                        childColor.a = 1.0f; // Set the alpha value (0.0f for fully transparent, 1.0f for opaque)
                        childMaterial.color = childColor;
                    }
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
                if (movingBrick != null)
                {
                    Destroy(movingBrick);
                    movingBrick = null;
                }
                movingBrick = LegoBrick.Create(c.brickType, c.brickColor);
                movingBrick.transform.SetParent(transform);

                //Renderer brickRenderer = movingBrick.AddComponent<Renderer>();

                //if (brickRenderer != null)
                //{
                //    Material brickMaterial = brickRenderer.material;
                //    Color brickColor = brickMaterial.color;
                //    brickColor.a = 0.5f; // Set the alpha value (0.0f for fully transparent, 1.0f for opaque)
                //    brickMaterial.color = brickColor;
                //}

                AnimationExecution animation = movingBrick.AddComponent<AnimationExecution>();

                animation.command = c;
                animation.StartAnimation();
                break;
            default:
                Debug.Log("Unknown Command");
                break;
        }
        
          
    }

    // Update is called once per frame
    void Update()
    {

    }
}

// x: 0.008, y: 0.0096, z: 0.008    1X1 
