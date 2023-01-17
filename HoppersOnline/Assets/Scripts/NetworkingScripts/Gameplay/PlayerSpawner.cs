using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    private Transform[] spawnPoints = new Transform[4];
    [SerializeField] CharactersListSO listSO;
    private GameMode gameMode;
    void Start()
    {
        //gameMode = GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }

        if(PhotonNetwork.CurrentRoom != null)
        {
            Transform spawnPoint = spawnPoints[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerNumber"] - 1];
            GameObject playerToSpawn = listSO.charactersList[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]].GetCharacterPrefab(true);
            PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.identity);
        }
    }
}