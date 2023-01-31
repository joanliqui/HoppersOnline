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
    [SerializeField] ErrorDisplayer errorPanel;

    MultiSelectionManager selectionManager;

    private void Awake()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        Time.timeScale = 1f;

        selectionManager = GameObject.FindGameObjectWithTag("SelectionManager").GetComponent<MultiSelectionManager>();
        cts = GetComponent<ConnectToServer>();

        if (!PhotonNetwork.IsConnected)
        { 
            cts.ConnectServer();
        }
        else
        {
            if (PhotonNetwork.InRoom) //En este caso venimos de la escena de gameplay tras darle al boton de salir;
            {
                lobbyPanel.SetActive(false);
                roomPanel.SetActive(true);

                selectionManager.OnEnterRoom();
            }
            else
            {
                lobbyPanel.SetActive(true);
                roomPanel.SetActive(false);
            }
        }
    }

    public override void OnJoinedLobby()
    {
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

    #region FAILS
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomInputField.text = string.Empty;
        string m = "";
        switch (returnCode)
        {
            case 32766: // GameAlreadyExists
                m = "La sala " + joinInputField.text + " ya existe";
                break;
        }

        Debug.Log(returnCode + " ----- " + message);
        errorPanel.DisplayMessage(m);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinInputField.text = string.Empty;
        string m = "";
        switch (returnCode)
        {
            case 32765: //JOINAL - GameFull 
                m = "La sala " + joinInputField.text + " esta llena";
                break;
            case 32758: //JOINAL - GameDoesNotExist 
                m = "La sala " + joinInputField.text + " no existe";
                break;
        }
        Debug.Log(returnCode + " ----- " + message);
        errorPanel.DisplayMessage(m);
    }
    #endregion
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

        selectionManager.OnLeaveRoom();
    }
}
