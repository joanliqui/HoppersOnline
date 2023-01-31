using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] NetMapBuilder mapBuilder;
    public UnityEvent<NetBaseHopper> onPlayerDisconect;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //NetBaseHopper disconected = NetGameManager.Instance.GetHopperByPlayerID((int)otherPlayer.ActorNumber);
        //onPlayerDisconect?.Invoke(disconected);

        
        if ((int)otherPlayer.CustomProperties["playerNumber"] == 1 || PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
