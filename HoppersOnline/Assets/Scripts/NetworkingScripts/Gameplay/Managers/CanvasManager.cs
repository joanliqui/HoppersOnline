using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] List<GameObject> canvasList;

    public void ActivateAllCanvas()
    {
        foreach (GameObject item in canvasList)
        {
            item.SetActive(true);
        }
    }
}
