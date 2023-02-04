using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class NetBoltz : NetBaseHopper
{
    [Space(20)]
    [Header("Ultimate Variables")]
    [SerializeField] GameObject rayPrefab;
    GameObject reusableRay;
    [SerializeField] Transform socket;

    [SerializeField] AudioClip ultimateClip;
    [SerializeField] AudioSource source;

    private void Start()
    {
        if (view.IsMine)
        {
            reusableRay = PhotonNetwork.Instantiate("AbilityPrefab/" + rayPrefab.name, new Vector3(0, - 20, 0), Quaternion.identity);
            view.RPC("DeactivateRay", RpcTarget.All, reusableRay.GetPhotonView().ViewID);
        }

        onUltPerformed.AddListener(UltimateSound);
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

            onUltPerformed?.Invoke();
        }
    }

    protected override void Gravity()
    {
        if (!isUlting)
        {
            base.Gravity();
        }
        else
        {
            if (!_isGrounded)
            {
                appliedMovement.y += Time.deltaTime * lowGravity;
            }
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

    private void UltimateSound()
    {
        source.clip = ultimateClip;
        source.pitch = 1;
        source.volume = 0.55f;
        source.Play();
    }
}
