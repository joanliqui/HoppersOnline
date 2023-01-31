using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Photon.Pun;

public class LoseManager : MonoBehaviour
{
    private class HoppersForLoseCondition
    {
        NetBaseHopper hopper;
        Transform pos;
        Collider2D col;
        public Vector2 posInView;

        public HoppersForLoseCondition(NetBaseHopper hopper)
        {
            this.hopper = hopper;
            this.pos = hopper.transform;
            this.col = hopper.transform.GetComponent<Collider2D>();
        }

        public void SetPosInView()
        {
            posInView = new Vector2(pos.position.x, pos.position.y + col.bounds.extents.y);
        }
    }

    List<HoppersForLoseCondition> hoppers = new List<HoppersForLoseCondition>();
    private List<Vector2> positionsInView = new List<Vector2>();

    private bool gameFinished;

    [SerializeField] GameObject endPanel;
    [SerializeField] TextMeshProUGUI playerNumberDisplayer;
    [SerializeField] GameObject playAgainButton;


    private PhotonView view;
    private void Start()
    {
        view = GetComponent<PhotonView>();
        foreach (NetBaseHopper item in NetGameManager.Instance.HoppersInGame)
        {
            HoppersForLoseCondition nuevo = new HoppersForLoseCondition(item);
            hoppers.Add(nuevo);
            positionsInView.Add(item.transform.position);
        }

        endPanel.SetActive(false);

        NetGameManager.Instance.onGameEnded.AddListener(SetEndPanel);

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
                    view.RPC("CallOnGameEvent", RpcTarget.All);
                }
            }
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
    private void SetEndPanelRPC()
    {
   
    }
}
