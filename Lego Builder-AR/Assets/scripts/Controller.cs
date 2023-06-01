using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class Controller : MonoBehaviour
{
    Recording r;
    private int step = 0;
    //public Vector3 rotationSpeed = new Vector3(0f, 50f, 0f); // Set the rotation speed in each axis
    [SerializeField] public GameObject popup;
    //public GameObject buildArea;
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/json/TestRecording.json";

        string jsonString = File.ReadAllText(path);

        r = Recording.FromJson(jsonString);
        DisplayCommand();
    }

    public Command GetNextCommand()
    {
        return r.commands[step++];
    }

    public Command GetCurrentCommand()
    {
        return r.commands[step];
    }

    public void DisplayCommand()
    {
        Command c = GetCurrentCommand();
        switch (c.type)
        {
            case Command.CommandType.RequireBrick:
                Debug.Log("Require Brick");
                GameObject brick = LegoBrick.Create(c.brickType, c.brickColor);
                GameObject instantiatedPopup = Instantiate(popup, transform);
                GameObject brickContainer = instantiatedPopup.transform.Find("Panel/brickContainer/rotationPivot").gameObject;
                brick.transform.SetParent(brickContainer.transform);
                brick.transform.localPosition = Vector3.Scale(new Vector3(-0.008f/2f, -0.0096f/2f, -0.008f/2f), brick.GetComponent<LegoBrick>().CellUnitDimensions);
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
