using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class NetPauseManager : MonoBehaviour
{
    private static NetPauseManager _instance;
    private static bool isPaused = false;

    [SerializeField] GameObject pausePanelObject;
    List<NetBaseHopper> hoppersInGame = new List<NetBaseHopper>();

    //Component References
    PhotonView view;

    public static bool IsPaused { get => isPaused; }

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
        view = GetComponent<PhotonView>();

        pausePanelObject.SetActive(false);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject item in players)
        {
            hoppersInGame.Add(item.GetComponent<NetBaseHopper>());
        }

        foreach (NetBaseHopper item in hoppersInGame)
        {
            if (item.View.IsMine)
            {
                item.OnPausePerformed += TogglePause;
            }
        }
    }
    private void OnDisable()
    {
        foreach (NetBaseHopper item in hoppersInGame)
        {
            if (item.View.IsMine)
            {
                item.OnPausePerformed -= TogglePause;
            }
        }
    }

    public void TogglePause()
    {
        view.RPC("TogglePauseRPC", RpcTarget.All);
    }

    [PunRPC]
    private void TogglePauseRPC()
    {
        Debug.Log("TogglePause");
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        pausePanelObject.SetActive(isPaused);
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("LobbyScene");
        }
        else
        {
            view.RPC("LoadLobbyScene", RpcTarget.All);
        }
    }

    [PunRPC]
    private void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
