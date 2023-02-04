using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MetersManager : MonoBehaviour
{
    bool startCount;
    [SerializeField] TextMeshProUGUI meterDisplayer;

    Transform playerPos;
    private Vector2 initialPos;
    private Vector2 lastPos;
    private int distance;
    private int meterConverter = 5;
    private int actualDistance;

    int frameRate = 0;

    void Start()
    {
        try
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
            lastPos = playerPos.position;
            initialPos = playerPos.position;
        }
        catch 
        {
            meterDisplayer.gameObject.SetActive(false);
        }

      
        distance = 0;
        actualDistance = 0;
        meterDisplayer.text = distance + "m";

        SoloGameManager.Instance.onGameStarted.AddListener(StartCounting);
    }

    void Update()
    {
        if (!playerPos) return;

        if (startCount)
        {
            if (frameRate % 3 == 0)
            {
                distance = (int)(playerPos.position.y - initialPos.y);

                meterDisplayer.text = distance + "m";
                frameRate = 0;
            }
            frameRate++;
        }
    }

    public void StartCounting()
    {
        startCount = true;
    }
}
