using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SoloSelectionManager : MonoBehaviour
{

    [Header("Holders")]
    [SerializeField] CharacterHolder holderPrefab;
    [SerializeField] Transform holdersContent;
    [SerializeField] CharactersListSO charactersList;
    [SerializeField] PlayerItem playerItem;
    bool selected = false;

    [Header("StartGame")]
    [SerializeField] GameObject playButton;

    HopperData data;
    void Start()
    {
        playButton.SetActive(false);
        SetCharacterHolders();
        data = GameObject.FindGameObjectWithTag("SoloData").GetComponent<HopperData>();
    }

    private void SetCharacterHolders()
    {
        foreach (CharacterSO item in charactersList.charactersList)
        {
            CharacterHolder newHolder = Instantiate(holderPrefab, holdersContent);
            newHolder.SetCharacterHolder(item);
            newHolder.onClickedButton += OnCharacterClick;
            newHolder.onPointerEntered += OnCharacterPointerEntered;
            newHolder.onPointerExit += OnCharacterPointerExited;
        }
    }

    private void OnCharacterClick(CharacterSO characterSO)
    {
        if (playerItem)
        {
            selected = true;
            playerItem.SelectedCharacter(characterSO.characterSprite);
            playButton.SetActive(true);

            data.SelectedCharacter = characterSO.GetCharacterPrefab(false);
        }
    }

    private void OnCharacterPointerExited()
    {
        if (selected) return;

        if (playerItem)
        {
            playerItem.ResetItemOnPoinerExit();
        }
    }

    private void OnCharacterPointerEntered(CharacterSO characterSO)
    {
        if (selected) return;

        if (playerItem)
        {   
            playerItem.PointedCharacter(characterSO.characterSprite);   
        }
    }

    public void OnClickPlayButton()
    {

        SceneManager.LoadScene("SoloGameplayScene");
    }

}
