using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class clickHandler : MonoBehaviour
    //, IPointerClickHandler
{
    public GameObject canvas;
    public int prefabId ;
    //public Controller controller;
    public static Controller controller = new Controller();
    //public sceneNam
    // Start is called before the first frame update
    void Start()
    {
        //controller = new Controller();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Card " + prefabId + " clicked!");
        SceneManager.LoadScene("BuildingScene", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }*/

    public void ChangeScene()
    {
        
        SceneManager.UnloadSceneAsync("Menu");
        SceneManager.LoadScene("BuildingScene", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ShowNextCommand()
    {
        //Controller controller = new Controller();
        controller.IncreaseStep();
    }

    public void ShowPrevCommand()
    {
        //Controller controller = new Controller();
        controller.DecreaseStep();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("Card " + prefabId + " clicked!");
        //Debug.Log("Debug: " + SceneManager.GetActiveScene().GetRootGameObjects());
        //SceneManager.GetActiveScene().GetRootGameObjects()[0].GetComponent<LoadModel>().SetPrefabId(prefabId);
    }

}
