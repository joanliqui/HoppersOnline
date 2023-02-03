using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltzRay : MonoBehaviour
{
    [SerializeField] float impulseForce = 1000;

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

                obj.Damaged();
            }
        }
    }

    public void TurnOffRay()
    {
        gameObject.SetActive(false);
    }
}
