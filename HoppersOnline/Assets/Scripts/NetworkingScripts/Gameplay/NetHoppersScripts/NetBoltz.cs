using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class NetBoltz : NetBaseHopper
{
    [Space(20)]
    [SerializeField] GameObject rayPrefab;
    GameObject reusableRay;
    [SerializeField] Transform socket;

    private void Start()
    {
        if (view.IsMine)
        {
            reusableRay = PhotonNetwork.Instantiate("AbilityPrefab/" + rayPrefab.name, new Vector3(0, - 20, 0), Quaternion.identity);
            view.RPC("DeactivateRay", RpcTarget.All, reusableRay.GetPhotonView().ViewID);
        }
    }

    protected override void Abilitie(InputAction.CallbackContext ctx)
    {
        if (canUlt)
        {
            isUlting = true;
            canUlt = false;
            reusableRay.transform.position = socket.position;
            view.RPC("ActivateRayRPC", RpcTarget.All, reusableRay.GetPhotonView().ViewID);
            cntUltTime = 0.0f;
        }
    }

    [PunRPC]
    public void DeactivateRay(int id)
    {
        if (!reusableRay)
        {
            reusableRay = PhotonNetwork.GetPhotonView(id).gameObject;
        }
        reusableRay.SetActive(false);   
    }

    [PunRPC]
    public void ActivateRayRPC(int id)
    {
        GameObject ray = PhotonNetwork.GetPhotonView(id).gameObject;
        ray.SetActive(true);

    }
}
