using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetCameraManager : MonoBehaviour
{
    [SerializeField] bool canMove = false;
    [SerializeField] float initialSpeed = 1f;
    
    [SerializeField] private float cntSpeed;
    private float t = 1;

    private void Start()
    {
        cntSpeed = initialSpeed;
        canMove = false;
       
        NetGameManager.Instance.onGameStarted.AddListener(LetsMove);
        
    }

    void Update()
    {
        if (!canMove) return;

        if (PhotonNetwork.IsMasterClient)
        {
            if (canMove)
            {
                transform.position += Vector3.up * cntSpeed * Time.deltaTime;
            }
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

    private void LetsMove()
    {
        canMove = true;
    }
}
