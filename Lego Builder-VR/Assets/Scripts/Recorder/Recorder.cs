using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = LegoBrick.Create(LegoBrick.BrickType.B2x2, LegoBrick.BrickColor.Blue);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
