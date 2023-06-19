using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;

public class Instatiate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject card;
    public GameObject brick_num;
    

    [System.Serializable]
    public class Model
    {
        public int id;
        public string name;
        public int time;
        public string level;
        public int bricks;
    }

    public void printModel(Model model)
    {
        Debug.Log("Name: " + model.name + ", Time: " + model.time + ", Level: " + model.level + ", Bricks: " + model.bricks + "\n");
    }

    IEnumerator GetJSONData()
    {
        string url = "http://139.91.96.111:3000/models";

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonData = request.downloadHandler.text;
            Debug.Log("JSON data: " + jsonData);
            Model[] models = JsonConvert.DeserializeObject<Model[]>(jsonData);
            for (int i = 0; i < models.Length; i++)
            {
                string duration = "";
                int minutes = models[i].time / 60;
                int seconds = models[i].time % 60;
                if (minutes != 0) duration += minutes + " minutes";
                if (seconds != 0) duration += " and " + seconds + " seconds";
                GameObject instantiatedPrefab = Instantiate(card, transform);
                GameObject time = instantiatedPrefab.transform.Find("information/content/time/time_text").gameObject;
                GameObject bricks = instantiatedPrefab.transform.Find("information/content/bricks/bricks_num").gameObject;
                GameObject level = instantiatedPrefab.transform.Find("information/content/level/level_text").gameObject;
                GameObject name = instantiatedPrefab.transform.Find("header/name").gameObject;
                GameObject state = instantiatedPrefab.transform.Find("header/state").gameObject;
                time.GetComponent<Text>().text = duration;
                bricks.GetComponent<Text>().text = models[i].bricks.ToString();
                level.GetComponent<Text>().text = models[i].level;
                name.GetComponent<Text>().text = models[i].name;
                state.GetComponent<Text>().text = "0% Completed";

                clickHandler onClickHandler = instantiatedPrefab.GetComponent<clickHandler>();
                onClickHandler.prefabId = models[i].id;
            }
        }
        else
        {
            Debug.LogError("Error retrieving JSON data: " + request.error);
        }
    }

    void Start()
    {
        //string path = Application.dataPath + "/json/Models.json";
        //string path = Path.Combine(Application.streamingAssetsPath, "/json/Models.json");


        //string path = Application.streamingAssetsPath + "/Models.json";
        //string jsonString = File.ReadAllText(path);
        //Model[] models = JsonConvert.DeserializeObject<Model[]>(jsonString);


        /*if (Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.WSAPlayerX86)
        {
            // Script is running on HoloLens
            Debug.Log("Running on HoloLens");
        }
        else
        {
            // Script is running on another platform
            Debug.Log("Not running on HoloLens");
        }
        StartCoroutine(GetJSONData());
*/


        string fileData = "";
        string fileName = Path.Combine(Application.streamingAssetsPath, "Models.json");
        byte[] bytes = UnityEngine.Windows.File.ReadAllBytes(fileName);
        fileData = System.Text.Encoding.ASCII.GetString(bytes);
        Model[] models = JsonConvert.DeserializeObject<Model[]>(fileData);

        for (int i = 0; i < models.Length; i++)
        {
            string duration = "";
            int minutes = models[i].time / 60;
            int seconds = models[i].time % 60;
            if (minutes != 0) duration += minutes + " minutes";
            if (seconds != 0) duration += " and " + seconds + " seconds";
            GameObject instantiatedPrefab = Instantiate(card, transform);
            GameObject time = instantiatedPrefab.transform.Find("information/content/time/time_text").gameObject;
            GameObject bricks = instantiatedPrefab.transform.Find("information/content/bricks/bricks_num").gameObject;
            GameObject level = instantiatedPrefab.transform.Find("information/content/level/level_text").gameObject;
            GameObject name = instantiatedPrefab.transform.Find("header/name").gameObject;
            GameObject state = instantiatedPrefab.transform.Find("header/state").gameObject;
            time.GetComponent<Text>().text = duration;
            bricks.GetComponent<Text>().text = models[i].bricks.ToString();
            level.GetComponent<Text>().text = models[i].level;
            name.GetComponent<Text>().text = models[i].name;
            state.GetComponent<Text>().text = "0% Completed";

            clickHandler onClickHandler = instantiatedPrefab.GetComponent<clickHandler>();
            onClickHandler.prefabId = models[i].id;
        }
        //GameObject instantiatedPrefab = Instantiate(card, transform);
        Debug.Log("Done");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
