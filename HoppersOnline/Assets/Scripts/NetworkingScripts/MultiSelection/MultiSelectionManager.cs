using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MultiSelectionManager : MonoBehaviourPunCallbacks
{
    public int playerNumber = 1;

    [SerializeField] TextMeshProUGUI roomNameDisplay;
    [SerializeField] PlayerItem[] playerItems;

    [Header("Holders")]
    [SerializeField] CharacterHolder holderPrefab;
    [SerializeField] Transform holdersContent;
    [SerializeField] CharactersListSO charactersList;
    bool selected = false;

    [Header("StartGame")]
    [SerializeField] GameObject playButton;
    [SerializeField] int minimumPlayers = 1;
    //PLAYER PROPERTIES
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    //Se llama desde OnJoinedRoom callback en el LobbyManager 
    public void OnEnterRoom()
    {
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
        selected = false;
        SetCharacterHolders();

        InicializePlayerProperties();
        UpdatePlayerList();
    }

    private void SetCharacterHolders()
    {
        foreach (Transform item in holdersContent)
        {
            Destroy(item.gameObject);
        }

        foreach (CharacterSO item in charactersList.charactersList)
        {
            CharacterHolder newHolder = Instantiate(holderPrefab, holdersContent);
            newHolder.SetCharacterHolder(item);
            newHolder.onClickedButton += OnCharacterClick;
            newHolder.onPointerEntered += OnCharacterPointerEntered;
            newHolder.onPointerExit += OnCharacterPointerExited;
        }
    }
    public void InicializePlayerProperties()
    {
        playerProperties["playerAvatar"] = -1;
        playerProperties["playerNumber"] = playerNumber;
        playerNumber++;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
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

        int n = 0;
        //Por cada Player que haya en el Room activamos el holder y lo seteamos con cada player
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {

            PlayerItem item = playerItems[n];
            item.ActivateItem();
            item.SetPlayerInfo(player.Value);

            n++;

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
    private void OnCharacterClick(CharacterSO characterSO)
    {
        for (int i = 0; i < charactersList.charactersList.Count; i++)
        {
            if(charactersList.charactersList[i] == characterSO)
            {
                playerProperties["playerAvatar"] = i;
                selected = true;
                break;
            }
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    private void OnCharacterPointerEntered(CharacterSO chararcterSo)
    {
        if (selected)
            return;


        foreach (PlayerItem item in playerItems)
        {
            if(item.MyPlayer == PhotonNetwork.LocalPlayer)
            {
                item.PointedCharacter(chararcterSo.characterSprite);
            }
        }
    }

    private void OnCharacterPointerExited()
    {
        if (selected)
            return;
        foreach (PlayerItem item in playerItems)
        {
            if (item.MyPlayer == PhotonNetwork.LocalPlayer)
            {
                item.ResetItemOnPoinerExit();
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerPropertiesOverNetwork(targetPlayer);
        ShowPlayButton();
    }

    private void UpdatePlayerPropertiesOverNetwork(Player p)
    {
        foreach (PlayerItem item in playerItems)
        {
            if (item.MyPlayer == p)
            {
                if (p.CustomProperties.ContainsKey("playerAvatar"))
                {
                    if((int)p.CustomProperties["playerAvatar"] != -1)
                    {
                        item.SelectedCharacter(charactersList.charactersList[(int)p.CustomProperties["playerAvatar"]].characterSprite);
                    }
                }
                return;
            }
        }
    }

    private void Update()
    {
        ShowPlayButton();
    }

    /// <summary>
    /// Solo se le muestra el boton al MasterClient cuando haya mas de 1 jugador, y cuando todos los jugadores hayan seleccionado algun personaje
    /// </summary>
    public void ShowPlayButton()
    {
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= minimumPlayers)
        {
            int playersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
            int n = 0;

            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if(player.Value.CustomProperties.ContainsKey("playerAvatar"))
                {
                    if((int)player.Value.CustomProperties["playerAvatar"] != -1)
                    {
                        n++;
                    }
                }
            }
            playButton.SetActive(n == playersInRoom);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("MultiGameplayScene");
    }
}
