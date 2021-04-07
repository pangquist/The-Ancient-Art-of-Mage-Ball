using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] MyNetworkManager myNetworkManager;

    //Method that is called through a button click that changes the selected character integer in the manager script
    public void PickCharacter(int characterIndex)
    {
        myNetworkManager.ChosenCharacter = characterIndex;
    }
}
