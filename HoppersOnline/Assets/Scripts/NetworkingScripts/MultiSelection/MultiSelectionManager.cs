using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MultiSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI roomNameDisplay;
    [SerializeField] PlayerItem[] playerItems;

    [Header("Holders")]
    [SerializeField] CharacterHolder holderPrefab;
    [SerializeField] Transform holdersContent;
    [SerializeField] List<CharacterSO> charactersList;

    //PLAYER PROPERTIES
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    [SerializeField] TextMeshProUGUI playerIdDisplay;

    //Se llama desde OnJoinedRoom callback en el LobbyManager 
    public void OnEnterRoom()
    {
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
        playerIdDisplay.text = PhotonNetwork.LocalPlayer.UserId;
        SetCharacterHolders();
        UpdatePlayerList();
        InicializePlayerProperties();
    }

    private void SetCharacterHolders()
    {
        foreach (Transform item in holdersContent)
        {
            Destroy(item.gameObject);
        }

        foreach (CharacterSO item in charactersList)
        {
            CharacterHolder newHolder = Instantiate(holderPrefab, holdersContent);
            newHolder.SetCharacterHolder(item);
            newHolder.onClickedButton += OnCharacterClick;
        }
    }

   

    public void OnExitRoomClick()
    {
        PhotonNetwork.LeaveRoom();
    }
    
   
    public void UpdatePlayerList()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        Debug.Log("UpdatePlayerList Method");
        foreach(PlayerItem item in playerItems)
        {
            item.ClearItem();
        }

        //Por cada Player que haya en el Room
        int i = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem item = playerItems[i];
            item.ActivateItem();
            item.SetPlayerInfo(player.Value);
        

            if(i < PhotonNetwork.CurrentRoom.PlayerCount)
            {
                i++;
            }
            ////En caso de que este sea nuestro player, remarcamos la casilla
            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                item.ApplyLocalChanges();
            }
        }
    }
  
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left the room");
        UpdatePlayerList();
    }


    //Setea la propiedad del sprite escogido a traves del network
    public void OnCharacterClick(CharacterSO characterSO)
    {
        for (int i = 0; i < charactersList.Count; i++)
        {
            if(charactersList[i] == characterSO)
            {
                playerProperties["playerAvatar"] = i;
                break;
            }
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void InicializePlayerProperties()
    {
        playerProperties["playerAvatar"] = -1;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("PlayerPropertiesUpdate callback");
        UpdatePlayerPropertiesOverNetwork(targetPlayer);
    }

    private void UpdatePlayerPropertiesOverNetwork(Player p)
    {
        foreach (PlayerItem item in playerItems)
        {
            if (item.MyPlayer == p)
            {
                if (p.CustomProperties.ContainsKey("playerAvatar"))
                {
                    item.SelectedCharacter(charactersList[(int)p.CustomProperties["playerAvatar"]].characterSprite);
                }
                else
                {
                    playerProperties["playerAvatar"] = -1;
                }
                break;
            }
        }
    }
}
