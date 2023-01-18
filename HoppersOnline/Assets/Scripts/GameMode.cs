using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    private static GameMode _instance;
    [SerializeField] GameModeEnum gameModeEnum;

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
            gameModeEnum = GameModeEnum.Muliplayer;
        }
        else
        {
            gameModeEnum = GameModeEnum.Solo;
        }
    }

    public GameModeEnum GetGameMode()
    {
        return gameModeEnum;
    }
}

public enum GameModeEnum
{
    Solo,
    Muliplayer
}
