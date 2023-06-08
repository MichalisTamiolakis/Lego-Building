using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BrickSpawner : MonoBehaviour
{
    [SerializeField]
    LegoBrick.BrickColor color;
    [SerializeField]
    LegoBrick.BrickType type;

    [SerializeField]
    bool randomize = true;

    private GameObject spawnedObject = null;

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == spawnedObject)
        {
            SpawnPrefab();
        }
    }

    public void SetColor(LegoBrick.BrickColor color)
    {
        this.color = color;
        Destroy(spawnedObject);
        SpawnPrefab();
    }

    private void Start()
    {
        SpawnPrefab();
    }

    public void SpawnPrefab()
    {
        spawnedObject = LegoBrick.Create(type, color);
        spawnedObject.transform.position = transform.position;
        spawnedObject.transform.rotation = transform.rotation;
        if(randomize)
        {
            spawnedObject.transform.Rotate(Vector3.up, Random.Range(0, 4) * 90);
        }
    }

}
