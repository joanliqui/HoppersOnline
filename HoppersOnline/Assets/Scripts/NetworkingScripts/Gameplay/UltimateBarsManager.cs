using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UltimateBarsManager : MonoBehaviour
{
    [SerializeField] NetUltBarController ultBarPrefab;
    List<NetUltBarController> ultBars = new List<NetUltBarController>();


    public void InicializeUltimateBar(NetBaseHopper hopper)
    {
        Transform p = GameObject.FindGameObjectWithTag("UltBarContainer").transform;

        GameObject g = PhotonNetwork.Instantiate(ultBarPrefab.gameObject.name, Vector3.zero, Quaternion.identity);

        NetUltBarController newBar = g.GetComponent<NetUltBarController>();
        ultBars.Add(newBar);
        newBar.SetHopper(hopper);
    } 
}


