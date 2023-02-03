using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private static PauseManager _instance;
    private static bool isPaused;

    [SerializeField] GameObject pausePanelObject;
    BaseHopper hopperInGame;

    public static bool IsPaused { get => isPaused; }

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
    }


    private void Start()
    {
        pausePanelObject.SetActive(false);
        isPaused = false;

        hopperInGame = SoloGameManager.Instance.SoloHopper;

        if(hopperInGame != null)
            hopperInGame.OnPausePerformed += TooglePause;
    }
    private void OnDisable()
    {
        if(hopperInGame != null)
            hopperInGame.OnPausePerformed -= TooglePause;
    }

    public void TooglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pausePanelObject.SetActive(isPaused);
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}
