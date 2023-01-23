using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorDisplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplayer;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void DisplayMessage(string message)
    {
        gameObject.SetActive(true);
        textDisplayer.text = message;
        StartCoroutine(WaitToDeactivate(3f));
    }

    IEnumerator WaitToDeactivate(float sec)
    {
        yield return new WaitForSeconds(sec);
        gameObject.SetActive(false);
    }
}
