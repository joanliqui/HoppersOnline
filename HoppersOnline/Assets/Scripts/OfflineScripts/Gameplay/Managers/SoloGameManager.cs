using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SoloGameManager : MonoBehaviour
{
    private static SoloGameManager _instance;

    public UnityEvent onGameStarted;
    public UnityEvent onGameEnded;

    [SerializeField] float timeToStart;
    private float cntTime;

    //Fases del juego
    private bool gameStarted = false;
    private bool actuallyPlaying = false;
    private bool gameEnded = false;

    private float lerpTime = 0;

    BaseHopper soloHopper;
    [SerializeField] PlayerSpawner spawner;
    [SerializeField] CanvasManager canvasManager;
    //[SerializeField] LoseManager loseManager;

    public static SoloGameManager Instance { get => _instance; set => _instance = value; }
    public BaseHopper SoloHopper { get => soloHopper; set => soloHopper = value; }
    public bool GameEnded { get => gameEnded; set => gameEnded = value; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        canvasManager.ActivateAllCanvas();
        spawner.SpawnPlayers();
        soloHopper = spawner.SoloHopper;

        MusicManager.Instance.GetActualScene();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        cntTime = 0.0f;
        actuallyPlaying = false;

        //GameStarted Events
        onGameStarted.AddListener(StartHopperCooldown);
        onGameStarted.AddListener(GamesFaseStart);

        //GameEnded Events
        //onGameEnded.AddListener(SetEndPanel);
        onGameEnded.AddListener(GameFaseEnded);
        if (soloHopper)
        {
            onGameEnded.AddListener(soloHopper.DisableAllInput);
        }
    }

    private void Update()
    {
        if (gameEnded)
        {
            if (lerpTime < 1)
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

    public void OnPlayAgainButtonClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SoloGameplayScene");
    }

    public void StartHopperCooldown()
    {
        if(soloHopper)
            soloHopper.StartCooldown();
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
}
