using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class clickHandler : MonoBehaviour, IPointerClickHandler
{

    public int prefabId;
    //public sceneNam
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Card " + prefabId + " clicked!");
        SceneManager.LoadScene("BuildingScene", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ChangeScene()
    {
        Debug.Log("Card " + prefabId + " clicked!");
        SceneManager.UnloadSceneAsync("Menu");
        SceneManager.LoadScene("BuildingScene", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.GetActiveScene().GetRootGameObjects()[0].GetComponent<LoadModel>().SetPrefabId(prefabId);
    }
}
