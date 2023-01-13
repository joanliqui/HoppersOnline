using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    ConnectToServer cts;

    [SerializeField] TMP_InputField roomInputField;
    [SerializeField] GameObject lobbyPanel;

    [SerializeField] RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    [SerializeField] Transform contentObject;

    [SerializeField] float timeBtwUpdates = 1.5f;
    float nextUpdateTime = 0;
    private void Awake()
    {
        lobbyPanel.SetActive(false);
        cts = GetComponent<ConnectToServer>();
        if (!PhotonNetwork.IsConnected)
            cts.ConnectServer();
        else Debug.Log("Ya estas conectado a un servidor");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby Callback");
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.LogWarning("No estas conectado a ningun lobby");
        }
    }

    //private void Start()
    //{
    //    PhotonNetwork.JoinLobby();
    //}

    public void OnClickCreate()
    {
        if(roomInputField.text.Length >= 4)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(roomInputField.text, roomOptions);
        }
        else
        {
            Debug.LogWarning("El nombre de la sala ha de tener minimo 4 carácteres");
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MultiHopperSelectionScene");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBtwUpdates;
        }
    }

    private void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string _roomName)
    {
        PhotonNetwork.JoinRoom(_roomName);
    }
}
