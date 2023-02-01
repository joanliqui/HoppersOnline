using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NetUltBarController : MonoBehaviour
{
    public float progress;
    [SerializeField] Image progressImage;
    [SerializeField] Color utlimateColor;
    [SerializeField] Image hopperImage;
    NetBaseHopper hopper;
    PhotonView view;

    //Component References
    AudioSource source;

    public NetBaseHopper Hopper { get => hopper; set => hopper = value; }
    public PhotonView View { get => view; set => view = value; }

    private void Awake()
    {
        progress = 0;
        progressImage.fillAmount = 0;

        source = GetComponent<AudioSource>();
        view = GetComponent<PhotonView>();
        hopper = null;
    }

    public void SetHopper(NetBaseHopper hop)
    {
        PhotonView v = hop.GetComponent<PhotonView>();
        view.RPC("SetHopperRPC", RpcTarget.All, v.ViewID);
    }

    public void UpdateBar(float p)
    {
        view.RPC("UpdateBarRPC", RpcTarget.All, p);
    }

    private void OnDisable()
    {
        if (hopper != null) hopper.OnUltCharging -= UpdateBar;
    }


    #region RPCs
    [PunRPC]
    private void SetHopperRPC(int id)
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("UltBarContainer").transform);
        transform.localPosition = Vector2.zero;

        NetBaseHopper hop = PhotonNetwork.GetPhotonView(id).gameObject.GetComponent<NetBaseHopper>();
        hopperImage.sprite = hop.HopperdCard.characterSprite;
        if (hopper != null)
        {
            hopper.OnUltCharging -= UpdateBar;
        }

        hopper = hop;
        hopper.OnUltCharging += UpdateBar;
    }

    [PunRPC]
    private void UpdateBarRPC(float p)
    {
        progressImage.color = Color.white;
        progress = Mathf.InverseLerp(0, hopper.Cooldown, p);
        progressImage.fillAmount = progress;

        if (progress >= 1)
        {
            progressImage.color = utlimateColor;
        }
    }

    #endregion
}
