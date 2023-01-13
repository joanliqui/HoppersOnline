using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RoomItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomName;
    LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void OnClickItem()
    {
        lobbyManager.JoinRoom(roomName.text);
    }
}
