using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ReloadingScene : MonoBehaviour
{

    private void Awake()
    {
        Time.timeScale = 1f;
    }
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Dale());
        }
    }

    private IEnumerator Dale()
    {
        yield return new WaitForSeconds(0.1f);
        PhotonNetwork.LoadLevel("MultiGameplayScene");

    }

    IEnumerator LoadLevelAsync()
    {
        PhotonNetwork.LoadLevel("Network Test");

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            //loadAmountText.text = "Loading: %" + (int)(PhotonNetwork.LevelLoadingProgress * 100);
            //loadAmount = async.progress;
            //progressBar.fillAmount = PhotonNetwork.LevelLoadingProgress;
            yield return new WaitForEndOfFrame();
        }
    }
}
