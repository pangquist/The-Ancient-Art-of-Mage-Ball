using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] MyNetworkManager myNetworkManager;

    public void PickCharacter(int characterIndex)
    {
        myNetworkManager.ChosenCharacter = characterIndex;
        Debug.Log("Chosen a new character: " + characterIndex);
    }
}
