using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UltimateBarsManager : MonoBehaviour
{
    [SerializeField] NetUltBarController ultBarPrefab;
    List<NetUltBarController> ultBars = new List<NetUltBarController>();

    [SerializeField] GameNetworkManager netManager;


    private void Start()
    {
        netManager.onPlayerDisconect.AddListener(DeleteUltimateBar);
    }
    public void InicializeUltimateBar(NetBaseHopper hopper)
    {
        Transform p = GameObject.FindGameObjectWithTag("UltBarContainer").transform;

        GameObject g = PhotonNetwork.Instantiate(ultBarPrefab.gameObject.name, Vector3.zero, Quaternion.identity);

        NetUltBarController newBar = g.GetComponent<NetUltBarController>();
        ultBars.Add(newBar);
        newBar.SetHopper(hopper);
    } 

    public void DeleteUltimateBar(NetBaseHopper hopper)
    {
        foreach (NetUltBarController item in ultBars)
        {
            if (item.Hopper.View.ViewID == hopper.View.ViewID)
            {
                PhotonNetwork.Destroy(item.gameObject);
                //Destroy(item.gameObject);
                return;
            }
        }
        
    }
}


