using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterList", menuName = "HoppersMenu/CharactersList")]
public class CharactersListSO : ScriptableObject
{
    public List<CharacterSO> charactersList;
}
