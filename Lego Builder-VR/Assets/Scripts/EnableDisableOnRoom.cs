using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableOnRoom : MonoBehaviour
{
    public int enableOnRoom = 0;

    public void Awake()
    {
        GameManager.Instance.onTeleport.AddListener(OnTeleport);
    }

    public void OnTeleport(int roomIndex, GameManager.RoomData roomData)
    {
        if(roomIndex == enableOnRoom)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
