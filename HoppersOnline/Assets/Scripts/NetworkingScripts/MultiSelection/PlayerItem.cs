using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] Image bg;
    [SerializeField] Color localColor;
    [SerializeField] Color emptyColor;
    private Image border;

    private Player myPlayer;

    PhotonView view;

    private void Awake()
    {
        border = GetComponent<Image>();
        view = GetComponent<PhotonView>();
    }
    public Player MyPlayer { get => myPlayer;}

    public void SetPlayerInfo(Player _player)
    {
        view.RPC("SetPlayerInfoRPC", RpcTarget.All, _player);
    }

    [PunRPC]
    private void SetPlayerInfoRPC(Player _player)
    {
        myPlayer = _player;
    }

    public void ApplyLocalChanges()
    {
        bg.color = localColor;   
    }

    public void SetItemEmpty()
    {
        view.RPC("SetItemEmptyRPC", RpcTarget.All);
    }

    [PunRPC]
    private void SetItemEmptyRPC()
    {
        border.color = emptyColor;
        bg.color = emptyColor;
        myPlayer = null;
    }
}
