using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MultiSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI roomNameDisplay;


    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
        }
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
