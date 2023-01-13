using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class MultiSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI roomNameDisplay;
    [SerializeField] PlayerItem[] playerItems;

    [Header("Holders")]
    [SerializeField] CharacterHolder holderPrefab;
    [SerializeField] Transform holdersContent;
    [SerializeField] List<CharacterSO> charactersList;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
            UpdatePlayerList();
            SetCharacterHolders();
        }
    }

    private void SetCharacterHolders()
    {
        foreach (Transform item in holdersContent)
        {
            Destroy(item);
        }

        foreach (CharacterSO item in charactersList)
        {
            CharacterHolder newHolder = Instantiate(holderPrefab, holdersContent);
            newHolder.SetCharacterHolder(item);
        }
    }

    public void InicializePlayerProperties()
    {
        playerProperties["characterPrefab"] = null;
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

   
    private void UpdatePlayerList()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
           // Debug.Log(PhotonNetwork.CurrentRoom.Players.Count);
            PlayerItem item = playerItems[player.Key - 1];
            Debug.Log(player.Key - 1);
            item.SetPlayerInfo(player.Value);
            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                item.ApplyLocalChanges();
            }
        }


        foreach (PlayerItem item in playerItems)
        {
            if(item.MyPlayer == null || item.MyPlayer.ActorNumber <= 0 )
            {
                Debug.Log(item.name);
                item.SetItemEmpty();
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnterCallback - Player:" + newPlayer.ActorNumber);
        //UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeft   Count:" + PhotonNetwork.CurrentRoom.PlayerCount);
        foreach (PlayerItem item in playerItems)
        {
            if(item.MyPlayer == otherPlayer)
            {
                item.SetItemEmpty();
            }
        }
        UpdatePlayerList();
    }
}
