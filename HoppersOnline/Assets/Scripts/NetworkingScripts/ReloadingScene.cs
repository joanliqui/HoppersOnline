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
        yield return new WaitForSeconds(0.2f);
        PhotonNetwork.LoadLevel("MultiGameplayScene");

    }
}
