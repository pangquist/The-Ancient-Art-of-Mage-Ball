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
    [SerializeField] Button changeSceneButton;

    [SerializeField] TeamManager teamManager;

    [SerializeField] TMP_Text[] redTeamNames = new TMP_Text[3];
    [SerializeField] TMP_Text[] blueTeamNames = new TMP_Text[3];
    
    private List<string[]> menuPlayers = new List<string[]>();


    private void Start()
    {
        Debug.Log("Lobbymenu started! Subscribing to events!");
        MyNetworkManager.ClientOnConnected += HandleClientConnected;
        MyNetworkMenuPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        MyNetworkMenuPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        MyNetworkMenuPlayer.OnClientTeamUpdated += ClientHandleTeamUpdated;
    }

    private void OnDestroy()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientConnected;
        MyNetworkMenuPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        MyNetworkMenuPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        MyNetworkMenuPlayer.OnClientTeamUpdated -= ClientHandleTeamUpdated;
    }
    
    // When a client connects to the server, they set the Lobby UI to be active.
    void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }
    // Method that starts the command in the player script, telling the server to start the game if the neccesary requirements are fullfilled.
    public void StartGame() 
    {
        NetworkClient.connection.identity.GetComponent<MyNetworkMenuPlayer>().CmdStartGame();
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
        }
        SceneManager.LoadScene(0);
    }

    void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    
    // Method that is connected to the action event from MyNetworkPlayer. This method activates whenever the player name is changed on the client and gets the displayname and writes it out on the corresponding slot in the lobby.
    void ClientHandleInfoUpdated() 
    {
        List<MyNetworkMenuPlayer> menuPlayers = ((MyNetworkManager)NetworkManager.singleton).MenuPlayers;
        MyNetworkMenuPlayer newPlayer;
        
        if (menuPlayers.Count != 0)
        {
            newPlayer = menuPlayers[menuPlayers.Count - 1];
        }
        else
        {
            return;
        }

        if (!newPlayer.hasAuthority)
        {
            return;
        }
        Debug.Log($"The new player is named: {newPlayer.GetDisplayName()} and has Authority: {newPlayer.hasAuthority}");
        
        int redPlayers = 0;
        int bluePlayers = 0;

        foreach(MyNetworkMenuPlayer player in menuPlayers)
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
        
        Debug.Log($"There are currently {redPlayers} players on the red team, and {bluePlayers} players on the blue team");
        startGameButton.interactable = menuPlayers.Count >= MyNetworkManager.playersRequiredToStart; 
    }
    
    public void ChangeTeam(string team)
    {
        Debug.Log("-------------------------------------------------------------------");

        MyNetworkMenuPlayer localMenuPlayer = NetworkClient.localPlayer.gameObject.GetComponent<MyNetworkMenuPlayer>();

        List<MyNetworkMenuPlayer> menuPlayers = ((MyNetworkManager)NetworkManager.singleton).MenuPlayers;

        if (menuPlayers.Count == 0)
        {
            return;
        }

        int redPlayers = 0;
        int bluePlayers = 0;
        
        if (team == "Red Team")
        {
            if (redPlayers >= 3 || localMenuPlayer.TeamName == "Red Team")
            {
                return;
            }

            localMenuPlayer.CmdSetTeamName("Red Team");
        }

        if (team == "Blue Team")
        {
            if (bluePlayers >= 3 || localMenuPlayer.TeamName == "Blue Team")
            {
                return;
            }

            localMenuPlayer.CmdSetTeamName("Blue Team");
        }

        foreach (MyNetworkMenuPlayer player in menuPlayers)
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
    }
    
    void ClientHandleTeamUpdated()
    {
        UpdateNameLists();
    }
    
    public void UpdateNameLists()
    {
        List<MyNetworkMenuPlayer> menuPlayers = ((MyNetworkManager)NetworkManager.singleton).MenuPlayers;
        
        foreach (MyNetworkMenuPlayer player in menuPlayers)
        {
            Debug.Log($"There are {menuPlayers.Count} players in the list, currently on: {player.GetDisplayName()} who is on the {player.TeamName}");
        }
        
        for (int i = 0; i < redTeamNames.Length; i++)
        {
            redTeamNames[i].text = "Waiting For Player...";
        }

        for (int i = 0; i < blueTeamNames.Length; i++)
        {
            blueTeamNames[i].text = "Waiting For Player...";
        }

        for (int i = 0; i < menuPlayers.Count; i++)
        {
            if (menuPlayers[i].TeamName == "Red Team")
            {
                for (int j = 0; j < redTeamNames.Length; j++)
                {
                    if (redTeamNames[j].text == "Waiting For Player...")
                    {
                        redTeamNames[j].text = menuPlayers[i].GetDisplayName();
                        break;
                    }
                }

            }
            else if (menuPlayers[i].TeamName == "Blue Team")
            {
                for (int j = 0; j < blueTeamNames.Length; j++)
                {
                    if (blueTeamNames[j].text == "Waiting For Player...")
                    {
                        blueTeamNames[j].text = menuPlayers[i].GetDisplayName();
                        break;
                    }
                }
            }
        }
    }
}
