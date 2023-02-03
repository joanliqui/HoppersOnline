using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    private PhotonView view;

    private Transform[] spawnPoints = new Transform[4];
    [SerializeField] CharactersListSO listSO;
    private GameMode gameMode;

    List<NetBaseHopper> hoppersInGame = new List<NetBaseHopper>();
    BaseHopper soloHopper;
    UltimateBarsManager barManager;
   

    public UnityEvent<NetBaseHopper> onPlayerSpawn;

    public BaseHopper SoloHopper { get => soloHopper; set => soloHopper = value; }

    public void SpawnPlayers()
    {
        gameMode = GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>();

        if(gameMode.GetGameMode() == GameModeEnum.Muliplayer)
        {
            view = GetComponent<PhotonView>();
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }

        if (gameMode != null)
        {
            if (gameMode.GetGameMode() == GameModeEnum.Solo)
            {
                HopperData data = null;
                try
                {
                    data = GameObject.FindGameObjectWithTag("SoloData").GetComponent<HopperData>();
                }
                catch (System.Exception)
                {
                    Debug.Log("No hay data");
                }

                if (data != null)
                {
                    GameObject hopp = Instantiate(data.SelectedCharacter, spawnPoints[0].position, Quaternion.identity);
                    soloHopper = hopp.GetComponent<BaseHopper>();
                }
            }
            else
            {
                if (PhotonNetwork.CurrentRoom != null)
                {
                    Transform spawnPoint = spawnPoints[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerNumber"] - 1];
                    GameObject playerToSpawn = listSO.charactersList[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]].GetCharacterPrefab(true);
                    GameObject player = PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.identity);

                    view.RPC("AddHopper", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
                    NetBaseHopper hopp = player.GetComponent<NetBaseHopper>();


                    onPlayerSpawn?.Invoke(hopp);
                }
            }
        }
    }


    [PunRPC]
    private void AddHopper(int id)
    {
        PhotonView hopperView = PhotonNetwork.GetPhotonView(id);
        NetBaseHopper hopp = hopperView.GetComponent<NetBaseHopper>();
        hopp.playerNumber = (int)hopp.View.Owner.CustomProperties["playerNumber"];
        hoppersInGame.Add(hopp);
        NetGameManager.Instance.SetHopperToList(hopp);
    }

    public List<NetBaseHopper> GetHoppersInGame()
    {
        return hoppersInGame;
    }
}
