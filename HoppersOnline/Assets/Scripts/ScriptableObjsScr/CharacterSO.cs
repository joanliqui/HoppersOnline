using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Character", menuName ="HoppersMenu/HopperSO")]
public class CharacterSO : ScriptableObject
{
    public int hopperNumber;
    public string characterName;
    public Sprite characterSprite;
    [SerializeField] GameObject netCharacterPrefab;
    [SerializeField] GameObject offCharacterPrefab;


    public GameObject GetCharacterPrefab(bool isNetworking)
    {
        if (isNetworking)
        {
            return netCharacterPrefab;
        }
        else
        {
            return offCharacterPrefab;
        }
    }

}
