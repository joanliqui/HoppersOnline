using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float openSpeed = 3f;
    [SerializeField] Transform[] doors = new Transform[2];

    private bool open = false;
    float openLerp = 0;
    private Vector2[] initialPos = new Vector2[2];
    private Vector2[] finalPos = new Vector2[2];

    void Start()
    {
        SoloGameManager.Instance.onGameStarted.AddListener(OpenDoor);
        for (int i = 0; i < doors.Length; i++)
        {
            initialPos[i] = doors[i].localPosition;
        }

        for (int i = 0; i < initialPos.Length; i++)
        {
            if(i == 0)
            {

                finalPos[i] = initialPos[i] + new Vector2(20, 0);
            }
            else
            {
                finalPos[i] = initialPos[i] + new Vector2(-20, 0);

            }
        }

    }

    void Update()
    {
        if (open)
        {
            if(openLerp < 1)
                openLerp += openSpeed * Time.deltaTime;

            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].localPosition = Vector2.Lerp(initialPos[i], finalPos[i], openLerp);
            }
        }
    }

    private void OpenDoor()
    {
        open = true;
    }
}
