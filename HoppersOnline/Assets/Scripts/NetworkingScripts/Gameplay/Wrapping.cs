using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapping : MonoBehaviour
{

    Camera wrappingCam;
    float heightCam;
    float widthCam;

    SpriteRenderer rend;
    Collider2D col;

    bool isWrapping = false;
    bool isVisible;

    Vector2 objectViewPort;
    
    private void Start()
    {
        wrappingCam = GameObject.FindGameObjectWithTag("WrappingCamera").GetComponent<Camera>();

        rend = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        heightCam = 2f * wrappingCam.orthographicSize;
        widthCam = heightCam * wrappingCam.aspect;
        isWrapping = true;
    }

    private void Update()
    {
        isVisible = RenderExtensions.IsVisibleFrom(col, wrappingCam);
        if (isVisible) //Si te esta viendo es que no estas fuera del Frustrum de la MAIN CAMERA
        {
            isWrapping = false;
            return;
        }

        Vector2 leftPos = wrappingCam.WorldToViewportPoint(new Vector3(transform.position.x + col.bounds.extents.x, transform.position.y));
        Vector2 rightPos = wrappingCam.WorldToViewportPoint(new Vector3(transform.position.x - col.bounds.extents.x, transform.position.y));

        Vector2 newPos = Vector2.zero;

        if (!isWrapping)
        {
            if(leftPos.y > 0)
            {
                if (leftPos.x < 0)
                {
                    newPos = new Vector2(-transform.position.x - col.bounds.size.x, transform.position.y);
                    isWrapping = true;
                    transform.position = newPos;
                }
                else if (rightPos.x > 1)
                {
                    newPos = new Vector2(-transform.position.x + col.bounds.size.x, transform.position.y);
                    isWrapping = true;
                    transform.position = newPos;
                }

            }
        }
    }

}
