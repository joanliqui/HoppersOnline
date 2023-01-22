using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetBackgroundController : MonoBehaviour
{
    Parallax parallax;
    Parallax[] parallaxes = new Parallax[2];
    SpriteRenderer sr;
    PhotonView view;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            parallaxes[i] = transform.GetChild(i).GetComponent<Parallax>();
        }

        view = GetComponent<PhotonView>();
        parallax = GetComponent<Parallax>();
        sr = GetComponent<SpriteRenderer>();
    }

    //[PunRPC]
    public void InitiateBackground(Transform parent, int orderInLayer)
    {
        gameObject.transform.SetParent(parent);
        gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 15f);
        sr.sortingOrder = orderInLayer;
        Debug.Log(orderInLayer);
    }

    public void InitiateBackgroundRPC(Transform p, int ord)
    {
        view.RPC("InitiateBackground", RpcTarget.All, p, ord);
    }

    /// <summary>
    /// Set parent to null and stop the parallaxes
    /// </summary>
    [PunRPC]
    public void DetachBackground()
    {
        gameObject.transform.parent = null;
        parallax.ParallaxState(false);
        foreach (Parallax item in parallaxes)
        {
            item.ParallaxState(false);
        }
    }

    
    public void DetachBackgroundRPC()
    {
        view.RPC("DetachBackground", RpcTarget.All);
    }
}
