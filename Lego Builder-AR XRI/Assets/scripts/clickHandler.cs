using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class clickHandler : MonoBehaviour
{
    public GameObject canvas;
    public GameObject buildingScene;
    public GameObject Menu;
    public int prefabId ;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ChangeScene()
    {
        /*Debug.Log("Menu object! " + Menu);
        Destroy(Menu);
        
        Vector3 position = new Vector3(0.051f, 1.096f, -0.111f);
        Quaternion rotation = Quaternion.identity;
        GameObject instantiatedPrefab = Instantiate(buildingScene, position, rotation);*/

        /*SceneManager.UnloadSceneAsync("Menu");
        SceneManager.LoadScene("BuildingScene", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;*/

        Debug.Log("Card clicked!");

        GameObject menu = GameObject.Find("Menu");
        if (menu == null)
        {
            Debug.Log("Menu object not found!");
            GameObject menu2 = GameObject.Find("Menu(Clone)");
            Destroy(menu2);
            //Debug.Log("Menu object found! " + buildingScene);
            Vector3 position = new Vector3(0.051f, 1.096f, -0.111f);
            Quaternion rotation = Quaternion.identity;
            GameObject instantiatedPrefab = Instantiate(buildingScene, position, rotation);
        }
        else
        {
            Destroy(menu);
            Debug.Log("buildingScene found! " + buildingScene);
            Vector3 position = new Vector3(0.051f, 1.096f, -0.111f);
            Quaternion rotation = Quaternion.identity;
            GameObject instantiatedPrefab = Instantiate(buildingScene, position, rotation);
            instantiatedPrefab.name = "BuildingContainer";
            /*GameObject buildArea = instantiatedPrefab.transform.Find("BuildArea").gameObject;
            buildArea.AddComponent<Controller>();
            buildArea.AddComponent<Manager>();*/
        }
    }

    public void ShowNextCommand()
    {
        Controller.Instance.IncreaseStep();
    }

    public void ShowPrevCommand()
    {
        Controller.Instance.DecreaseStep();
    }

    public void ShowModel()
    {
        Controller.Instance.ShowFinalModel();
    }

    public void ExitBuildScene()
    {
        Destroy(buildingScene);
        // Get the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject instantiatedPrefab = Instantiate(Menu);
        //instantiatedPrefab.transform.position = Vector3.zero;
        instantiatedPrefab.name = "Menu";
        SceneManager.MoveGameObjectToScene(instantiatedPrefab, currentScene);
        //GameObject instantiatedPrefab = Instantiate(Menu, transform);
        /*SceneManager.UnloadSceneAsync("BuildingScene");
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;*/
        /*GameObject menu = GameObject.Find("BuildingContainer");
        if (menu == null)
        {
            Debug.Log("BuildingContainer object not found!");
        }
        else
        {
            Destroy(menu);
            //Debug.Log("Menu object found! " + buildingScene);
            *//*Vector3 position = new Vector3(0.051f, 1.096f, -0.111f);
            Quaternion rotation = Quaternion.identity;*//*
            GameObject instantiatedPrefab = Instantiate(Menu, transform);
        }*/
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("Card " + prefabId + " clicked!");
    }

}
