using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
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
    }

    private void Update()
    {
        if (canParallax)
        {
            offset.y += Time.deltaTime * parallaxSpeed ;

            sr.material.mainTextureOffset = offset;
        }
    }

    public void ParallaxState(bool enabled)
    {
        canParallax = enabled;
    }
}
