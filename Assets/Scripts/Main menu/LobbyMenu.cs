using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] Button startGameButton;
    [SerializeField] TMP_Text[] playerNameTexts = new TMP_Text[6];

    private void Start()
    {
        MyNetworkManager.ClientOnConnected += HandleClientConnected;
        MyNetworkPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        MyNetworkPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientConnected;
        MyNetworkPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        MyNetworkPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    void HandleClientConnected() //When a client connects to the server, they set the Lobby UI to be active
    {
        lobbyUI.SetActive(true);
    }
    
    //Method that is connected to the action event from MyNetworkPlayer. This method activates whenever the player name is changed on the client and gets the displayname and writes it out on the corresponding slot in the lobby.
    void ClientHandleInfoUpdated() 
    {
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;
        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].GetComponent<MyNetworkPlayer>().GetDisplayName();
        }
        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
        }

        //startGameButton.interactable = players.Count >= MyNetworkManager.playersRequiredToStart; //Add this when testing is complete
    }

    void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    //Method that starts the command in the player script, telling the server to start the game if the neccesary requirements are fullfilled.
    public void StartGame() 
    {
        NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>().CmdStartGame();
    }

    //Method that checks whether the player is the server or a client. If they are the server, the lobby is shut down. If they are the client they are reloaded to the main menu scene.
    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
