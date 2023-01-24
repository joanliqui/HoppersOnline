using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    Sprite initialSprite;
    [SerializeField] Image bg;
    private Image border;

    [SerializeField] Color highlightColor;
    [SerializeField] Color emptyColor;
    [SerializeField] Color pointedColor;

    private Player myPlayer = null;

    private void Awake()
    {
        border = GetComponent<Image>();
        initialSprite = bg.sprite;
    }
    public Player MyPlayer { get => myPlayer;}

    public void SetPlayerInfo(Player _player)
    {
        myPlayer = _player;
    }

    public void ApplyLocalChanges()
    {
        border.color = highlightColor;   
    }

    public void ActivateItem()
    {
        border.color = Color.white;
        bg.color = Color.white;
    }
    public void ClearItem()
    {
        border.color = emptyColor;
        bg.color = emptyColor;
        bg.sprite = initialSprite;
        myPlayer = null;
    }

    public void ResetItemOnPoinerExit()
    {
        bg.color = Color.white;
        bg.sprite = initialSprite;
    }

    public void SelectedCharacter(Sprite characterSprite)
    {
        bg.color = Color.white;
        bg.sprite = characterSprite;
    }

    public void PointedCharacter(Sprite characterSprite)
    {
        bg.sprite = characterSprite;
        bg.color = pointedColor;
    }

    public void DeleteCharacterSprite()
    {
        bg.sprite = initialSprite;
    }


}
