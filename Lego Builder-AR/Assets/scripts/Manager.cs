using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    public Dictionary<int, GameObject> activeBricks = new Dictionary<int, GameObject>();
    
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBrick(int blockID, GameObject brick)
    {
        if (!activeBricks.ContainsKey(blockID))
        {
            activeBricks.Add(blockID, brick);
        }
        
        foreach (var kvp in activeBricks)
        {
            Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value.name);
        }
        
    }

    public GameObject GetBrick(int blockID)
    {

        if (activeBricks.ContainsKey(blockID))
        {
            Debug.Log("Brick with ID: " + blockID + " found!");
            return activeBricks[blockID];
        }
        return null;    
    }

    public void RemoveBrick(int blockID)
    {
        activeBricks.Remove(blockID);
    }
}
