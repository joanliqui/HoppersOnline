using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] bool canMove = false;
    [SerializeField] float initialVelocity = 1f;
    
    private float cameraScrollSpeed;

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            transform.position += Vector3.up * initialVelocity * Time.deltaTime;
        }
    }
}
