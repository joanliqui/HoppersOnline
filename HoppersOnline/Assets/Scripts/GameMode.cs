using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    private static GameMode _instance;
    [SerializeField] GameModeEnum gameMode;

    public static GameMode Instance { get => _instance;}

    private void Start()
    {
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

    public void SetGameMode(bool isOnline)
    {
        if (isOnline)
        {
            gameMode = GameModeEnum.Muliplayer;
        }
        else
        {
            gameMode = GameModeEnum.Solo;
        }
    }
}

public enum GameModeEnum
{
    Solo,
    Muliplayer
}
