using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraChild : MonoBehaviour
{
    Camera cam;
    int cullingPlayerMask;
    float posX;
    Vector3 pos;

    float heightCam;
    float widthCam;

    [SerializeField] bool isLeftCamera;

    void Start()
    {
        pos = new Vector3();
        cullingPlayerMask = LayerMask.NameToLayer("Player");
        cam = GetComponent<Camera>();


        cam.orthographicSize = Camera.main.orthographicSize;
        heightCam = 2f * cam.orthographicSize;
        widthCam = heightCam * cam.aspect;

        posX = isLeftCamera ? widthCam * -1 : widthCam; //Si es la Cam de la izquierda te pones a la izq

        pos.x = posX;
        pos.y = 0;
        pos.z = 0;
        transform.localPosition = pos;
    }
}
