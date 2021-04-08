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
    [SerializeField] TMP_Text[] lobbyNames = new TMP_Text[6];

    [SerializeField] TeamManager teamManager;
    [SerializeField] TMP_Text[] redTeamNames = new TMP_Text[3];
    [SerializeField] TMP_Text[] blueTeamNames = new TMP_Text[3];


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
            if (players.Count <= 3)
            {
                teamManager.redTeam.Add(players[i]);
                redTeamNames[i].text = players[i].GetComponent<MyNetworkPlayer>().GetDisplayName();
            }
            else
            {
                teamManager.blueTeam.Add(players[i]);
                blueTeamNames[i].text = players[i].GetComponent<MyNetworkPlayer>().GetDisplayName();
            }

        }
        for (int i = players.Count; i < redTeamNames.Length + blueTeamNames.Length; i++)
        {
            lobbyNames[i].text = "Waiting For Player...";
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
        MyNetworkPlayer localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<MyNetworkPlayer>();

        teamManager.AssignTeam();
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

    public void ChangeTeam(string team)
    {
        MyNetworkPlayer localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<MyNetworkPlayer>();

        if (team == "Red Team")
        {
            for (int i = 0; i < teamManager.blueTeam.Count; i++)
            {
                if (teamManager.blueTeam[i] == localPlayer)
                {
                    if (teamManager.redTeam.Count == 3)
                        return;
                    blueTeamNames[i].text = "Waiting For Player...";
                    teamManager.blueTeam.Remove(localPlayer);
                    teamManager.redTeam.Add(localPlayer);
                    UpdateNameLists();
                    return;
                }
            }
        }
        else if (team == "Blue Team")
        {
            for (int i = 0; i < teamManager.redTeam.Count; i++)
            {
                if (teamManager.redTeam[i] == localPlayer)
                {
                    if (teamManager.blueTeam.Count == 3)
                        return;
                    redTeamNames[i].text = "Waiting For Player...";
                    teamManager.redTeam.Remove(localPlayer);
                    teamManager.blueTeam.Add(localPlayer);
                    UpdateNameLists();
                    return;
                }
            }
        }
        Debug.Log("Changing player to " + team);
    }

    public void UpdateNameLists()
    {
        Debug.Log("Red Team Count: " + teamManager.redTeam.Count);

        for (int i = 0; i < teamManager.redTeam.Count; i++)
        {
            if (teamManager.redTeam[i] != null)
                redTeamNames[i].text = teamManager.redTeam[i].GetComponent<MyNetworkPlayer>().GetDisplayName();
        }

        Debug.Log("Blue Team Count: " + teamManager.blueTeam.Count);

        for (int i = 0; i < teamManager.blueTeam.Count; i++)
        {
            if (teamManager.blueTeam[i] != null)
                blueTeamNames[i].text = teamManager.blueTeam[i].GetComponent<MyNetworkPlayer>().GetDisplayName();
        }
    }
}
