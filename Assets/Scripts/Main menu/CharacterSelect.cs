using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
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
        //Get the players current position, the image and give the image the sprite of the character
    }
}
