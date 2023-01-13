using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterHolder : MonoBehaviour
{
    [SerializeField] Image characterImage;
    CharacterSO character;

    public void SetCharacterHolder(CharacterSO _character)
    {
        character = _character;
        characterImage.sprite = _character.characterSprite;
    }
   
   
}
