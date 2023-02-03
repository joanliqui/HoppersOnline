using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class SoloLoseManager : MonoBehaviour
{
    [System.Serializable]
    private class HoppersForLoseCondition
    {
        BaseHopper hopper;
        public bool hasLost;
        Transform pos;
        Collider2D col;
        public Vector2 posInView;

        public BaseHopper Hopper { get => hopper; set => hopper = value; }

        public HoppersForLoseCondition(BaseHopper hopper)
        {
            this.hopper = hopper;
            this.pos = hopper.transform;
            this.col = hopper.transform.GetComponent<Collider2D>();
            hasLost = false;
        }

        public void SetPosInView()
        {
            posInView = new Vector2(pos.position.x, pos.position.y + col.bounds.extents.y);
        }
    }

    private HoppersForLoseCondition hopperLose;
    private Vector2 posInView;

    private bool gameFinished;

    [SerializeField] GameObject endPanel;

    private AudioSource source;
    UnityEvent onPlayerDead = new UnityEvent();


    private void Start()
    {
        source = GetComponent<AudioSource>();

        endPanel.SetActive(false);

        //PlayerDead Event
        onPlayerDead.AddListener(PlayerDead);

        //GameEnd Event
        SoloGameManager.Instance.onGameEnded.AddListener(SetEndPanel);
        SoloGameManager.Instance.onGameEnded.AddListener(source.Play);
        if (SoloGameManager.Instance.SoloHopper)
        {
            hopperLose = new HoppersForLoseCondition(SoloGameManager.Instance.SoloHopper);
        }
    }

    private void Update()
    {
        if (!SoloGameManager.Instance.GameEnded && hopperLose != null)
        {
            hopperLose.SetPosInView();
            posInView = Camera.main.WorldToViewportPoint(hopperLose.posInView);

            if (posInView.y < 0)
            {
                if (!hopperLose.hasLost)
                {
                    hopperLose.hasLost = true;
                    onPlayerDead?.Invoke();
                }
            }
        }
    }

    private void PlayerDead()
    {
        SoloGameManager.Instance.onGameEnded?.Invoke();
    }

    private void SetEndPanel()
    {
        endPanel.SetActive(true);
    }
}
