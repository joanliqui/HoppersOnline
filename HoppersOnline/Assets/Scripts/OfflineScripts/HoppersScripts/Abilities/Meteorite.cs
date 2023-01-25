using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float impulseForce = 2000;
    private AudioSource source;

    float initialVolume;

    //HAY QUE CAMBIARLO A UN POOL SISTEMA 
    private void Start()
    {
         source = GetComponent<AudioSource>();

        initialVolume = source.volume;
        source.pitch = Random.Range(0.6f, 1.4f);
        source.volume = initialVolume - Random.Range(-0.4f, 0.4f);
        source.Play();
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position += Vector3.down * speed * Random.Range(1, 1.5f) * Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable obj))
        {
            if (!other.gameObject.TryGetComponent(out Talys t))
            {
                obj.Damaged(transform.position, impulseForce);
            }
        }
    }
}
