using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class clickHandler : MonoBehaviour
{
    public GameObject canvas;
    public GameObject buildingScene;
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

        /*SceneManager.UnloadSceneAsync("Menu");
        SceneManager.LoadScene("BuildingScene", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;*/

        GameObject menu = GameObject.Find("Menu");
        if (menu == null)
        {
            Debug.Log("Menu object not found!");
        }
        else
        {
            Destroy(menu);
            Debug.Log("Menu object found! " + buildingScene);
            Vector3 position = new Vector3(0f, 0f, 0f);
            Quaternion rotation = Quaternion.identity;
            GameObject instantiatedPrefab = Instantiate(buildingScene, position, rotation);
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
        SceneManager.UnloadSceneAsync("BuildingScene");
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("Card " + prefabId + " clicked!");
    }

}
