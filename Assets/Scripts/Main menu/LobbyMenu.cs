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

    [SerializeField] TeamManager teamManager;

    [SerializeField] TMP_Text[] redTeamNames = new TMP_Text[3];
    [SerializeField] TMP_Text[] blueTeamNames = new TMP_Text[3];
    
    private List<string[]> menuPlayers = new List<string[]>();


    private void Start()
    {
        MyNetworkManager.ClientOnConnected += HandleClientConnected;
        MyNetworkPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        MyNetworkPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        MyNetworkPlayer.OnClientTeamUpdated += ClientHandleTeamUpdated;
    }

    private void OnDestroy()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientConnected;
        MyNetworkPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        MyNetworkPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        MyNetworkPlayer.OnClientTeamUpdated -= ClientHandleTeamUpdated;
    }
    
    // When a client connects to the server, they set the Lobby UI to be active.
    void HandleClientConnected()
    {
        //Debug.Log("Setting the lobby UI to active for the client!");

        lobbyUI.SetActive(true);
    }
    // Method that starts the command in the player script, telling the server to start the game if the neccesary requirements are fullfilled.
    public void StartGame() 
    {
        NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>().CmdStartGame();
    }

    // Method that checks whether the player is the server or a client. If they are the server, the lobby is shut down. If they are the client they are reloaded to the main menu scene.
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

    void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    
    // Method that is connected to the action event from MyNetworkPlayer. This method activates whenever the player name is changed on the client and gets the displayname and writes it out on the corresponding slot in the lobby.
    void ClientHandleInfoUpdated() 
    {
        //Debug.Log("7. Handling that the clients info has been updated!");
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;
        MyNetworkPlayer newPlayer = NetworkClient.connection.identity.GetComponent<MyNetworkPlayer>();
        Debug.Log($"Number of players in list: {players.Count}");


        if (players.Count != 0)
        {
            newPlayer = players[players.Count - 1];
        }
        else
        {
            return;
        }

        Debug.Log($"Player: {newPlayer}");

        int redPlayers = 0;
        int bluePlayers = 0;

        foreach(MyNetworkPlayer player in players)
        {
            if(player.TeamName == "Red Team")
            {
                redPlayers++;
            }
            else if(player.TeamName == "Blue Team")
            {
                bluePlayers++;
            }
        }

        if(redPlayers >= 3 && bluePlayers < 3)
        {
            newPlayer.CmdSetTeamName("Blue Team");
        }
        else
        {
            newPlayer.CmdSetTeamName("Red Team");
        }

        RpcUpdateNameLists();

        startGameButton.interactable = players.Count >= MyNetworkManager.playersRequiredToStart; 
    }
    
    public void ChangeTeam(string team)
    {
        MyNetworkPlayer localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<MyNetworkPlayer>();

        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        if (players.Count == 0)
        {
            return;
        }

        int redPlayers = 0;
        int bluePlayers = 0;
        
        if (team == "Red Team")
        {
            if (redPlayers >= 3 || localPlayer.TeamName == "Red Team")
            {
                return;
            }

            localPlayer.CmdSetTeamName("Red Team");
        }

        if (team == "Blue Team")
        {
            if (bluePlayers >= 3 || localPlayer.TeamName == "Blue Team")
            {
                return;
            }

            localPlayer.CmdSetTeamName("Blue Team");
        }

        foreach (MyNetworkPlayer player in players)
        {
            if (player.TeamName == "Red Team")
            {
                redPlayers++;
            }

            else if (player.TeamName == "Blue Team")
            {
                bluePlayers++;
            }
        }

        //RpcUpdateNameLists();

        //Debug.Log($"Changing player to: {team}");
        //Debug.Log($"Red players: {redPlayers} Blue players: {bluePlayers}");
    }
    
    void ClientHandleTeamUpdated()
    {
        RpcUpdateNameLists();
    }
    
    public void RpcUpdateNameLists()
    {
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        ((MyNetworkManager)NetworkManager.singleton).ClearMenuPlayers();

        foreach (MyNetworkPlayer player in players)
        {
            menuPlayers.Add(new string[] { player.GetDisplayName(), player.TeamName });
        }

        foreach (string[] listItem in menuPlayers)
        {
            Debug.Log($"Lising up all the players in the list: {listItem[0]} is on the {listItem[1]}");
        }

        ((MyNetworkManager)NetworkManager.singleton).MenuPlayers = menuPlayers;

        for (int i = 0; i < redTeamNames.Length; i++)
        {
            redTeamNames[i].text = "Waiting For Player...";
        }

        for (int i = 0; i < blueTeamNames.Length; i++)
        {
            blueTeamNames[i].text = "Waiting For Player...";
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].TeamName == "Red Team")
            {
                for (int j = 0; j < redTeamNames.Length; j++)
                {
                    if (redTeamNames[j].text == "Waiting For Player...")
                    {
                        redTeamNames[j].text = players[i].GetDisplayName();
                        break;
                    }
                }

            }
            else if (players[i].TeamName == "Blue Team")
            {
                for (int j = 0; j < blueTeamNames.Length; j++)
                {
                    if (blueTeamNames[j].text == "Waiting For Player...")
                    {
                        blueTeamNames[j].text = players[i].GetDisplayName();
                        break;
                    }
                }
            }
        }
    }
}
