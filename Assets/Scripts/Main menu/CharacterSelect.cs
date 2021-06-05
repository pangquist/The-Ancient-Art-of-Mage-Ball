using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    // This script changes the character index of what character the player has currently selected.
    // When a character is picked, the picture in the main menu which displays the selected character is updated to correspond to the selection.
    // Author: Valter Lindecrantz.

    [Header("Script Dependencies")]
    [SerializeField] MyNetworkManager myNetworkManager;
    [SerializeField] MyNetworkMenuPlayer myNetworkMenuPlayer;

    [Header("Settings")]
    [SerializeField] Image characterImage;
    [SerializeField] AudioSource selectSound;

    [Header("Images")]
    [SerializeField] Sprite[] characterImages;
    
    private void Start()
    {
        myNetworkManager = GameObject.Find("MenuNetworkManager").GetComponent<MyNetworkManager>();
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
        selectSound.Play();
        characterImage.sprite = characterImages[characterIndex];
    }
}
