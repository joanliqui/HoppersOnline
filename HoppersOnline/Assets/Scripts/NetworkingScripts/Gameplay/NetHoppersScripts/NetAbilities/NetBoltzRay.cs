using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetBoltzRay : MonoBehaviour
{
    PhotonView view;
    [SerializeField] float impulseForce = 1000;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (collision.TryGetComponent<IDamageable>(out IDamageable obj))
        {
            if (!collision.gameObject.TryGetComponent(out Talys t))
            {
                Vector2 dir;
                if (collision.transform.position.x > transform.position.x)
                {
                    dir = new Vector2(2, 1);
                }
                else
                {
                    dir = new Vector2(-2, 1);
                }

                obj.Damaged(dir.x, dir.y, impulseForce, collision.GetComponent<PhotonView>().ViewID);
            }
        }
    }

    public void TurnOffRayRPC()
    {
        gameObject.SetActive(false);
    }
}
