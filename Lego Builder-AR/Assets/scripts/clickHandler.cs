using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class clickHandler : MonoBehaviour
{
    public GameObject canvas;
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
        
        SceneManager.UnloadSceneAsync("Menu");
        SceneManager.LoadScene("BuildingScene", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ShowNextCommand()
    {
        Controller.Instance.IncreaseStep();
    }

    public void ShowPrevCommand()
    {
        Controller.Instance.DecreaseStep();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("Card " + prefabId + " clicked!");
    }

}
