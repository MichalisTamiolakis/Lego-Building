using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignIdToBrick : MonoBehaviour
{
    public int blockID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        Debug.Log("From Enable: Adding brick with ID: " + blockID);
        Manager.Instance.AddBrick(blockID, this.gameObject);
    }
    void OnDisable()
    {
        Manager.Instance.RemoveBrick(blockID);
    }
    
}
