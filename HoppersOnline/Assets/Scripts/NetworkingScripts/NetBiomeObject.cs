using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetBiomeObject : MonoBehaviour
{
    [SerializeField] TypeZone typeZone;
    [SerializeField] Transform connectorPoint;

    [Header("Enemies Space")]
    [SerializeField] Transform[] enemySpawnPoints;

    private Collider2D col;
    float ps;

    bool addZone = false;
    PhotonView view;
    private void Start()
    {
        col = GetComponent<Collider2D>();
        view = GetComponent<PhotonView>();
    }

    public Transform GetConnectorPoint()
    {
        if (connectorPoint != null)
            return connectorPoint;
        else
            Debug.LogError(string.Format("Connector Point of {0} is not set", gameObject.name));
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainCamera"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                col.enabled = false;
                NetMapBuilder.Instance.OnCamEnterEventCall(typeZone);
            }
            
        }
    }
}
