using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class Controller : MonoBehaviour
{
    public static Recording r;
    public int step = 0;
    public GameObject instantiatedPopup;
    //public Vector3 rotationSpeed = new Vector3(0f, 50f, 0f); // Set the rotation speed in each axis
    [SerializeField] public GameObject popup;
    //public GameObject buildArea;
    // Start is called before the first frame update
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
        Debug.Log("step="+step + ", size="+ r.commands.Count);
        /*string path = Application.dataPath + "/json/TestRecording.json";

        string jsonString = File.ReadAllText(path);

        r = Recording.FromJson(jsonString);*/
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
        Debug.Log("Before Switch:"+ c.type);
        switch (c.type)
        {
            case Command.CommandType.RequireBrick:
                Debug.Log("Require Brick");
                GameObject brick = LegoBrick.Create(c.brickType, c.brickColor);
                instantiatedPopup = Instantiate(popup, transform);
                GameObject brickContainer = instantiatedPopup.transform.Find("Panel/brickContainer/rotationPivot").gameObject;
                brick.transform.SetParent(brickContainer.transform);
                brick.transform.localPosition = Vector3.Scale(new Vector3(-0.008f/2f, -0.0096f/2f, -0.008f/2f), brick.GetComponent<LegoBrick>().CellUnitDimensions);
                break;
            case Command.CommandType.MoveBrick:
                Debug.Log("Move Brick");
                if (instantiatedPopup != null)
                {
                    Destroy(instantiatedPopup);
                }
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
