using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetPool : BasePool
{
    [SerializeField] GameObject prefab;
    [SerializeField] int initialPoolSize = 8;

    private Queue<GameObject> objectPool;
    private PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            view.RPC("InicializePoolRPC", RpcTarget.All);
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            AddObject();
        }
    }
    public GameObject Get()
    {
        if (objectPool.Count == 0)
        {
            AddObject();
        }

        return objectPool.Dequeue();
    }

    private void AddObject()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject newObject = PhotonNetwork.Instantiate("AbilityPrefab/"+prefab.name, Vector3.zero, Quaternion.identity);
            PhotonView v = newObject.GetComponent<PhotonView>();
            view.RPC("AddObjectRPC", RpcTarget.All, v.ViewID);
        }
    }

    public override void ReturnToPool(IPooleable objectToReturn)
    {
        PhotonView v = objectToReturn.GameObject.GetComponent<PhotonView>();
        view.RPC("ReturnToPoolRPC", RpcTarget.All, v.ViewID);   
    }

    #region RPC FUNCTIONS
    [PunRPC]
    private void ReturnToPoolRPC(int id)
    {
        IPooleable objectToReturn = PhotonNetwork.GetPhotonView(id).gameObject.GetComponent<IPooleable>();
        objectToReturn.GameObject.SetActive(false);
        objectPool.Enqueue(objectToReturn.GameObject);
    }

    [PunRPC]
    private void AddObjectRPC(int id)
    {
        GameObject newObject = PhotonNetwork.GetPhotonView(id).gameObject;
        newObject.SetActive(false);
        newObject.GetComponent<IPooleable>().Pool = this;
        objectPool.Enqueue(newObject);

    }

    [PunRPC]
    private void InicializePoolRPC()
    {
        objectPool = new Queue<GameObject>();
    } 
    #endregion
}
