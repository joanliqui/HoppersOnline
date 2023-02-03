using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    static MusicManager _instance;

    string menuScene = "MenuScene";
    string actualScene = "";
    private AudioSource source;
    public static MusicManager Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        source = GetComponent<AudioSource>();

        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(_instance != this)
        {
            Destroy(gameObject);
        }

    }

    public void GetActualScene()
    {
        actualScene = SceneManager.GetActiveScene().name;

        if(actualScene ==  "SoloGameplayScene" || actualScene == "MultiGameplayScene")
        {
            source.Stop();
        }
        else if(actualScene == "MainMenuScene")
        {
            source.Play();
        }
    }

}
