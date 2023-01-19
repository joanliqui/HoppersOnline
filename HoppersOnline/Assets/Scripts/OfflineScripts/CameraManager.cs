using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] bool canMove = false;
    [SerializeField] float initialSpeed = 1f;
    
    [SerializeField] private float cntSpeed;
    private float t = 1;


    private void Start()
    {
        cntSpeed = initialSpeed;
    }

    void Update()
    {
        if (canMove)
        {
            transform.position += Vector3.up * cntSpeed * Time.deltaTime;
        }
    }


    public void StopCameraMovement()
    {
        t -= Time.deltaTime * 2;
        if(cntSpeed > 0)
        {
            cntSpeed = Mathf.Lerp(0.0f, cntSpeed, t);
        }
    }
}
