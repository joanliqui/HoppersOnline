using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    //void Start()
    //{
    //    //Conecta al servidor
    //    PhotonNetwork.ConnectUsingSettings();
    //}

    public override void OnConnectedToMaster()
    {
        //SceneManager.LoadScene("LobbyScene");
    }


    public void ConnectServer()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
}
