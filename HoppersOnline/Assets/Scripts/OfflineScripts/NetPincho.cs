using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetPincho : MonoBehaviour
{
    [SerializeField] float impulseForce;
    private int pointUpDir;
    [SerializeField] bool pointUp;
    private void Start()
    {
        pointUpDir = pointUp ? 1 : -1;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IDamageable>(out IDamageable player))
        {
            Vector2 dir;
            if(collision.transform.position.x > transform.position.x)
            {
                dir = new Vector2(1, pointUpDir);
            }
            else
            {
                dir = new Vector2(-1, pointUpDir);
            }
            player.Damaged(dir.x, dir.y, impulseForce, collision.GetComponent<PhotonView>().ViewID);
        }
    }
}
