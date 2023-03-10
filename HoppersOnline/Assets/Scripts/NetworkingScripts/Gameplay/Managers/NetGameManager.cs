using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class NetGameManager : MonoBehaviour
{
    PhotonView view;

    private static NetGameManager _instance;

    public UnityEvent onGameStarted;
    public UnityEvent onGameEnded;

    [SerializeField] float timeToStart;
    private float cntTime;
    
    //Fases del juego
    private bool gameStarted = false;
    private bool actuallyPlaying = false;
    private bool gameEnded = false;

    private float lerpTime = 0;

    List<NetBaseHopper> hoppersInGame = new List<NetBaseHopper>();
    [SerializeField] PlayerSpawner spawner;
    [SerializeField] LoseManager loseManager;
    [SerializeField] CanvasManager canvasManager;

    //NO SE QUE HACER CON ESTO
    public Dictionary<Player, NetBaseHopper> playerHopperDictionary = new Dictionary<Player, NetBaseHopper>();
    private Queue< KeyValuePair<Player, NetBaseHopper>> cola = new Queue<KeyValuePair<Player, NetBaseHopper>>();

    #region PROPIEDADES
    public static NetGameManager Instance { get => _instance; }
    public List<NetBaseHopper> HoppersInGame { get => hoppersInGame; set => hoppersInGame = value;}
    public bool GameEnded { get => gameEnded; set => gameEnded = value; }
    #endregion
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
        spawner.SpawnPlayers();

        view = GetComponent<PhotonView>();
        canvasManager.ActivateAllCanvas();

        if(MusicManager.Instance)
            MusicManager.Instance.GetActualScene();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        cntTime = 0.0f;
        actuallyPlaying = false;

        //GameStarted Events
        onGameStarted.AddListener(StartHoppersCooldowns);
        onGameStarted.AddListener(GamesFaseStart);

        //GameEnded Events
        //onGameEnded.AddListener(SetEndPanel);
        onGameEnded.AddListener(GameFaseEnded);
        onGameEnded.AddListener(DisableAllInputs);
    }

    private void Update()
    {
        if (gameEnded)
        {
            if(lerpTime < 1)
            {
                Time.timeScale = Mathf.Lerp(1, 0, lerpTime);
                lerpTime += Time.deltaTime;
            }
            else
            {
                lerpTime = 1f;
                Time.timeScale = 0f;
            }
        }
    }

    //AQUI TENGO QUE CONSEGUIR QUE DEVUELVA EL GAMEOBJECT DEL HOPPER ASIGANDO AL PLAYER QUE PASAMOS POR PARAMETRO
    public NetBaseHopper GetHopperByPlayerID(int hopperID)
    {
        foreach (NetBaseHopper item in hoppersInGame)
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
        actuallyPlaying = true;
        foreach (NetBaseHopper hopper in hoppersInGame)
        {
            hopper.StartCooldown();
        }
    }

    public void OnPlayAgainButtonClick()
    {
        Time.timeScale = 1f;
        PhotonNetwork.LoadLevel("ReloadingScene");
        //view.RPC("LoadLevelRPC", RpcTarget.All);
    }

    public void GamesFaseStart()
    {
        gameStarted = true;
        actuallyPlaying = true;
        gameEnded = false;
    }

    public void GameFaseEnded()
    {
        gameStarted = true;
        actuallyPlaying = true;
        gameEnded = true;
    }

    public void DisableAllInputs()
    {
        foreach (NetBaseHopper item in hoppersInGame)
        {
            item.DisableAllInput();
        }
    }

    public void EnableAllInput()
    {
        foreach (NetBaseHopper item in hoppersInGame)
        {
            item.EnableAllInput();
        }
    }

    public void SetHopperToList(NetBaseHopper hopper)
    {
        hoppersInGame.Add(hopper);
        loseManager.AddToHoppersList(hopper);
    }
}
