using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager2 : MonoBehaviourPunCallbacks
{
    ConnectToServer cts;

    [SerializeField] TMP_InputField roomInputField;
    [SerializeField] TMP_InputField joinInputField;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject roomPanel;

    MultiSelectionManager selectionManager;

    private void Awake()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);

        selectionManager = GameObject.FindGameObjectWithTag("SelectionManager").GetComponent<MultiSelectionManager>();
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
    }

    public void OnClickCreate()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.PublishUserId = true;
        roomOptions.PlayerTtl = 0;
        PhotonNetwork.CreateRoom(roomInputField.text, roomOptions);
    }

    public void OnClickJoinButton()
    {
        PhotonNetwork.JoinRoom(joinInputField.text);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinInputField.text = string.Empty;
        Debug.LogError("No has podido entrar a la room");
        Debug.LogError(message + " ----ReturnCode:" + returnCode);
    }

    public override void OnJoinedRoom()
    {
        joinInputField.text = string.Empty;
        roomInputField.text = string.Empty;
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        selectionManager.OnEnterRoom();
    }

    public override void OnLeftRoom()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        selectionManager.UpdatePlayerList();
    }
}
