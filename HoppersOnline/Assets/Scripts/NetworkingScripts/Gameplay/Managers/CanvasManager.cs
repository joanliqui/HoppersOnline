using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] List<GameObject> canvasList;

    private void Start()
    {
        foreach (GameObject item in canvasList)
        {
            item.SetActive(true);
        }
    }
}
