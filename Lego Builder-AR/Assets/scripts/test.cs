using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
        LegoBrick.Create(LegoBrick.BrickType.B2x2, LegoBrick.BrickColor.Red);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
