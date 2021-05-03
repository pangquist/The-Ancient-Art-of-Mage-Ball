using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] MyNetworkManager myNetworkManager;
    [SerializeField] MyNetworkMenuPlayer myNetworkMenuPlayer;
    
    private void Start()
    {
        myNetworkMenuPlayer = myNetworkManager.MenuPlayers[myNetworkManager.MenuPlayers.Count - 1];
    }

    //Method that is called through a button click that changes the selected character integer in the manager script
    public void PickCharacter(int characterIndex)
    {
        foreach (MyNetworkMenuPlayer menuPlayer in myNetworkManager.MenuPlayers)
        {
            if (menuPlayer.isLocalPlayer)
            {
                menuPlayer.CmdUpdateChosenCharacter(characterIndex);
            }
        }
    }
}
