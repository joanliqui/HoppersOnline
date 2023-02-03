using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperData : MonoBehaviour
{
    GameObject selectedCharacter;

    public GameObject SelectedCharacter { get => selectedCharacter; set => selectedCharacter = value; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
