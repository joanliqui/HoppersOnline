using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1;
    }

    public void MultiplayerButton()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void SoloButton()
    {
        SceneManager.LoadScene("SoloHopperSelectionScene");
    }

    public void CreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }
}
