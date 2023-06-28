using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public enum GameState
    {
        Menu = 0,
        Playing = 1,
    }

    public GameState gameState = GameState.Menu;

    public List<RoomData> rooms = new List<RoomData>();
    public int currentRoomIndex = 0;

    public Transform player;

    public UnityEvent<int, RoomData> onTeleport = new UnityEvent<int, RoomData>();


    private void Start()
    {
        TeleportTo(currentRoomIndex);
    }

    // Loads the given recording and teleports to the main room (room 1).
    public void LoadBuild(Recording recording)
    {
        TeleportTo(1);
        Recorder.Instance.SetRecording(recording);
        Recorder.Instance.PlayRecording();
        gameState = GameState.Playing;
    }

    public void NewBuild()
    {
        TeleportTo(1);
        Recorder.Instance.CreateNewRecording("New Recording");
        gameState = GameState.Playing;
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void TeleportTo(int roomIndex)
    {
        Debug.Assert(roomIndex < rooms.Count, "Room index out of range.");

        rooms[currentRoomIndex].roomParent.SetActive(false);
        rooms[roomIndex].roomParent.SetActive(true);

        player.gameObject.SetActive(false);
        player.transform.position = rooms[roomIndex].teleportLocation.position;
        player.transform.rotation = rooms[roomIndex].teleportLocation.rotation;
        player.gameObject.SetActive(true);

        currentRoomIndex = roomIndex;
        onTeleport?.Invoke(roomIndex, rooms[roomIndex]);
    }

    public RoomData GetCurrentRoom()
    {
        Debug.Assert(currentRoomIndex < rooms.Count, "Room index out of range.");

        return rooms[currentRoomIndex];
    }

    [System.Serializable]
    public class RoomData
    {
        public Transform teleportLocation;
        [Tooltip("Used for occlusion.")]
        public GameObject roomParent;
    }
}
