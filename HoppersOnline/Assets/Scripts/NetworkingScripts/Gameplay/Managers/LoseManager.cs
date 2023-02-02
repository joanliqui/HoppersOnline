using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Photon.Pun;

public class LoseManager : MonoBehaviour
{
    [System.Serializable]
    private class HoppersForLoseCondition
    {
        NetBaseHopper hopper;
        public bool hasLost;
        Transform pos;
        Collider2D col;
        public Vector2 posInView;

        public NetBaseHopper Hopper { get => hopper; set => hopper = value; }

        public HoppersForLoseCondition(NetBaseHopper hopper)
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




    private PhotonView view;
    List<HoppersForLoseCondition> hoppers = new List<HoppersForLoseCondition>();
    private List<Vector2> positionsInView = new List<Vector2>();

    private bool gameFinished;

    [SerializeField] GameObject endPanel;
    [SerializeField] TextMeshProUGUI playerNumberDisplayer;
    [SerializeField] GameObject playAgainButton;


    private int nPlayersLost = 0;
    UnityEvent<NetBaseHopper> onPlayerDead = new UnityEvent<NetBaseHopper>();
    private AudioSource source;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        source = GetComponent<AudioSource>();

        endPanel.SetActive(false);

        //PlayerDead Event
        onPlayerDead.AddListener(PlayerDead);

        //GameEnd Event
        NetGameManager.Instance.onGameEnded.AddListener(SetEndPanel);
        NetGameManager.Instance.onGameEnded.AddListener(source.Play);


    }
    private void Update()
    {
        if (!NetGameManager.Instance.GameEnded)
        {
            for (int i = 0; i < hoppers.Count; i++)
            {
                hoppers[i].SetPosInView();
                positionsInView[i] = Camera.main.WorldToViewportPoint(hoppers[i].posInView);

                if(positionsInView[i].y < 0)
                {
                    if (!hoppers[i].hasLost)
                    {
                        nPlayersLost++;
                        hoppers[i].hasLost = true;

                        onPlayerDead?.Invoke(hoppers[i].Hopper);
                    }
                    
                }
            }
        }
    }

    private void PlayerDead(NetBaseHopper hop)
    {
        if(nPlayersLost == PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            view.RPC("CallOnGameEvent", RpcTarget.All);
        }
    }

    [PunRPC]
    private void CallOnGameEvent()
    {
        NetGameManager.Instance.onGameEnded?.Invoke(); //GameEnded = true;
    }

    private void SetEndPanel()
    {
        endPanel.SetActive(true);
        int winner = -1;
        foreach (HoppersForLoseCondition item in hoppers)
        {
            if (!item.hasLost)
            {
                winner = item.Hopper.playerNumber;
            }
        }
        if(PhotonNetwork.IsMasterClient)
            view.RPC("SyncWinnerNumber", RpcTarget.All, winner);

        if (PhotonNetwork.IsMasterClient)
        {
            playAgainButton.SetActive(true);
        }
        else
        {
            playAgainButton.SetActive(false);
        }
    }

    [PunRPC]
    private void SyncWinnerNumber(int winner)
    {
        playerNumberDisplayer.text = winner.ToString();
    }

    public void AddToHoppersList(NetBaseHopper hop)
    {
        HoppersForLoseCondition nuevo = new HoppersForLoseCondition(hop);
        hoppers.Add(nuevo);
        positionsInView.Add(hop.transform.position);
    }
}
