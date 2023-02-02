using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetCameraManager : MonoBehaviour
{
    [SerializeField] bool canMove = false;
    [SerializeField] float initialSpeed = 1f;
    [SerializeField] float accelerationPerFrame;
    [SerializeField] float maxSpeed = 30f;
    private float cntSpeed;
    private float stopLerp = 1;

    private void Start()
    {
        cntSpeed = initialSpeed;
        canMove = false;
       
        NetGameManager.Instance.onGameStarted.AddListener(LetsMove);
        
    }

    void Update()
    {
        if (!canMove) return;

        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            if (canMove)
            {
                transform.position += Vector3.up * cntSpeed * Time.deltaTime;

                if(cntSpeed < maxSpeed)
                    cntSpeed += accelerationPerFrame * Time.deltaTime;
            }
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
