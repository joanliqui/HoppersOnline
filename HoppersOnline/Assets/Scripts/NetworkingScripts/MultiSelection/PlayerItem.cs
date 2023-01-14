using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] Image bg;
    private Image border;

    [SerializeField] Color highlightColor;
    [SerializeField] Color emptyColor;

    private Player myPlayer = null;

    private void Awake()
    {
        border = GetComponent<Image>();
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
        myPlayer = null;
    }

    public void SelectedCharacter(Sprite characterSprite)
    {
        bg.sprite = characterSprite;
    }


}
