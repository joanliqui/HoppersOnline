using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour, IPooleable
{
    [SerializeField] float speed;
    [SerializeField] float impulseForce = 2000;

    private AudioSource source;
    private float initialVolume;
    private float lifeTime = 2f;
    private float cntLifeTime;

    private Pool pool;
    public Pool Pool 
    { 
        get { return pool; } 
        set
        {
            if (pool == null)
                pool = value;
        }
    }

    public GameObject GameObject 
    { 
        get => this.gameObject;
    }

    //HAY QUE CAMBIARLO A UN POOL SISTEMA 
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        initialVolume = source.volume;
        source.pitch = Random.Range(0.6f, 1.4f);
        source.volume = initialVolume - Random.Range(-0.4f, 0.4f);
        cntLifeTime = 0;

        source.Play();
    }

    void Update()
    {
        transform.position += Vector3.down * speed * Random.Range(1, 1.5f) * Time.deltaTime;
        if(cntLifeTime < lifeTime)
        {
            cntLifeTime += Time.deltaTime;
        }
        else
        {
            pool.ReturnToPool(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable obj))
        {
            if (!other.gameObject.TryGetComponent(out Talys t))
            {
                obj.Damaged(transform.position, impulseForce);
                pool.ReturnToPool(this);
            }
        }
    }
}
