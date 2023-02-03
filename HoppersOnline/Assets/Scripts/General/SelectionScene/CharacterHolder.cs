using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CharacterHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image characterImage;
    CharacterSO character;


    public delegate void OnClick(CharacterSO characterSO);
    public event OnClick onClickedButton;
    public event OnClick onPointerEntered;

    public delegate void OnExit();
    public event OnExit onPointerExit; 

    public void SetCharacterHolder(CharacterSO _character)
    {
        character = _character;
        characterImage.sprite = _character.characterSprite;
    }


    public void OnClickButton()
    {
        onClickedButton?.Invoke(character);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEntered?.Invoke(character);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
    }
}
