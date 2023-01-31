using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetGameManager : MonoBehaviour
{
    private static NetGameManager _instance;

    public UnityEvent onGameStarted;
    public UnityEvent onGameEnded;

    [SerializeField] float timeToStart;
    private bool started = false;
    private float cntTime;
    List<NetBaseHopper> hoppers;
    [SerializeField] PlayerSpawner spawner;

    [Space(20)]
    [Header("End Variables")]
    [SerializeField] GameObject endPanel;
    [SerializeField] TextMeshProUGUI playerNumberDisplayer;
    [SerializeField] GameObject playAgainButton;

    //NO SE QUE HACER CON ESTO
    public Dictionary<Player, NetBaseHopper> playerHopperDictionary = new Dictionary<Player, NetBaseHopper>();
    private Queue< KeyValuePair<Player, NetBaseHopper>> cola = new Queue<KeyValuePair<Player, NetBaseHopper>>();

    public static NetGameManager Instance { get => _instance; }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else if(_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cntTime = 0.0f;
        started = false;
        hoppers = spawner.GetHoppersInGame();

        endPanel.SetActive(false);

        //GameStarted Events
        onGameStarted.AddListener(StartHoppersCooldowns);
        //GameEnded Events
        onGameEnded.AddListener(SetEndPanel);
    }

    //AQUI TENGO QUE CONSEGUIR QUE DEVUELVA EL GAMEOBJECT DEL HOPPER ASIGANDO AL PLAYER QUE PASAMOS POR PARAMETRO
    public NetBaseHopper GetHopperByPlayerID(int hopperID)
    {
        foreach (NetBaseHopper item in hoppers)
        {
            if (item.View.OwnerActorNr == hopperID)
            {
                return item;
            }
        }        
        return null;
    }

    public void StartHoppersCooldowns()
    {
        started = true;
        foreach (NetBaseHopper hopper in hoppers)
        {
            hopper.StartCooldown();
        }
    }

    public void SetEndPanel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playAgainButton.SetActive(true);
        }
        else
        {
            playAgainButton.SetActive(false);
        }

        playerNumberDisplayer.text = "x";
    }

    public void OnPlayAgainButtonClick()
    {
        PhotonNetwork.LoadLevel("MultiGameplayScene");
    }

}
