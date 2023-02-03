using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] bool canMove = false;
    [SerializeField] float initialSpeed = 1f;
    [SerializeField] float accelerationPerFrame;
    [SerializeField] float maxSpeed = 13f;
    private float cntSpeed;
    private float stopLerp = 1;


    private void Start()
    {
        cntSpeed = initialSpeed;
        canMove = false;

        SoloGameManager.Instance.onGameStarted.AddListener(LetsMove);
    }

    void Update()
    {
        if (canMove)
        {
            transform.position += Vector3.up * cntSpeed * Time.deltaTime;

            if (cntSpeed < maxSpeed)
                cntSpeed += accelerationPerFrame * Time.deltaTime;
        }
    }


    public void StopCameraMovement()
    {
        stopLerp -= Time.deltaTime * 2;
        if(cntSpeed > 0)
        {
            cntSpeed = Mathf.Lerp(0.0f, cntSpeed, stopLerp);
        }
    }

    private void LetsMove()
    {
        canMove = true;
    }
}
