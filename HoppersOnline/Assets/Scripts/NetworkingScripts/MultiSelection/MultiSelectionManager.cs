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
    PhotonView view;
    private bool afterFirstPropertyUpdate;

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
    ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    //Se llama desde OnJoinedRoom callback en el LobbyManager 
    /// <summary>
    /// Setea la Room y llama a la InicializePlayerProperties()
    /// </summary>
    public void OnEnterRoom()
    {
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
        selected = false;
        afterFirstPropertyUpdate = false;

        SetCharacterHolders();
        InicializeRoomProperties();

        InicializePlayerProperties();
        
    }

    /// <summary>
    /// Limpia los PlayerItems del LOCAL; solo del Local
    /// </summary>
    public void OnLeaveRoom()
    {
        foreach (PlayerItem item in playerItems)
        {
            item.ClearItem();
        }
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
  
    /// <summary>
    /// Inicializa las propiedades del Player y llama al UpdateCustomProperties 
    /// </summary>
    public void InicializePlayerProperties()
    {
        playerProperties["playerAvatar"] = -1;

        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerNumber"))
        {
            playerProperties["playerNumber"] = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        }
        else
        {
            playerProperties["playerNumber"] = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerNumber"];
        }

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
   
    }

    private void InicializeRoomProperties()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("cntPlayers"))
        {
            roomProperties["cntPlayers"] = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
    }
    public void OnExitRoomClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void NewUpdatePlayerList()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        int n = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            playerItems[n].ActivateItem();
            playerItems[n].SetPlayerInfo(player.Value);

            if (playerItems[n].MyPlayer.CustomProperties.ContainsKey("playerAvatar"))
            {
                if ((int)playerItems[n].MyPlayer.CustomProperties["playerAvatar"] != -1)
                {
                    playerItems[n].SelectedCharacter(charactersList.charactersList[(int)player.Value.CustomProperties["playerAvatar"]].characterSprite); //Creo que aqui esta el bug
                }
            }

            if(playerItems[n].MyPlayer == PhotonNetwork.LocalPlayer)
            {
                playerItems[n].ApplyLocalChanges();
            }
            n++;
        }

        for (int i = n; i < playerItems.Length; i++)
        {
            playerItems[n].ClearItem();
            n++;
        }
    }

    public void UpdatePlayerList()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        
        foreach(PlayerItem item in playerItems)
        {
            if(item.MyPlayer == null)
            {
                item.ClearItem();
            }
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
  

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (PlayerItem item in playerItems)
        {
            if(item.MyPlayer == otherPlayer)
            {
                item.ClearItem();
            }
        }
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
       
        NewUpdatePlayerList();
        afterFirstPropertyUpdate = true;
        
    }

    private void UpdatePlayerPropertiesOverNetwork(Player p)
    {
        if (!PhotonNetwork.InRoom) return;

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
                    else
                    {
                        item.DeleteCharacterSprite();
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
