using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] float parallaxSpeed = 2;
    [SerializeField] bool canParallax = true;

    SpriteRenderer sr;
    Transform cam;
    Vector2 offset;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        offset = sr.material.mainTextureOffset;
        offset.x = 0;
        cam = Camera.main.transform;
        gameObject.transform.SetParent(cam);
        gameObject.transform.localPosition = new Vector3(0.0f,0.0f, 15f);
    }

    private void Update()
    {
        if (canParallax)
        {
            offset.y = cam.position.y * parallaxSpeed / transform.localScale.y;
            sr.material.mainTextureOffset = offset;
        }
    }

    public void ParallaxState(bool enabled)
    {
        canParallax = enabled;
        if (!enabled)
        {
            transform.parent.SetParent(null);
        }
    }
}
