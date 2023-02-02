using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pincho : MonoBehaviour
{
    [SerializeField] float impulseForce;
    private int pointUpDir;

    private void Start()
    {
        if(transform.localRotation.z > 90 || transform.localRotation.z < -90)
        {
            pointUpDir = -1;
        }
        else
        {
            pointUpDir = 1;
        }
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
            player.Damaged(dir, impulseForce);
        }
    }
}
