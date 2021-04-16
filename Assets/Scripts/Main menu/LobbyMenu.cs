using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] Button startGameButton;
    //[SerializeField] TMP_Text[] lobbyNames = new TMP_Text[6];

    [SerializeField] TeamManager teamManager;

    [SerializeField] TMP_Text[] redTeamNames = new TMP_Text[3];
    [SerializeField] TMP_Text[] blueTeamNames = new TMP_Text[3];
    
    private List<string[]> menuPlayers = new List<string[]>();


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
        Debug.Log("Handling info!!");
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        MyNetworkPlayer newPlayer = players[players.Count - 1];

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

        UpdateNameLists();

        //for (int i = 0; i < teamManager.redTeam.Length; i++)
        //{
        //    if (teamManager.redTeam[i] == "")
        //    {
        //        teamManager.redTeam[i] = newPlayer.GetDisplayName();
        //        UpdateNameLists();
        //        return;
        //    }
        //}
        //for (int i = 0; i < teamManager.blueTeam.Length; i++)
        //{
        //    if (teamManager.blueTeam[i] == "")
        //    {
        //        teamManager.blueTeam[i] = newPlayer.GetDisplayName();
        //        UpdateNameLists();
        //        return;
        //    }
        //}
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

        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        int redPlayers = 0;
        int bluePlayers = 0;

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

        if (team == "Red Team")
        {
            if (redPlayers >= 3 || localPlayer.TeamName == "Red Team") return;

            localPlayer.CmdSetTeamName("Red Team");

            for (int j = 0; j < blueTeamNames.Length; j++)
            {
                if (blueTeamNames[j].text == localPlayer.GetDisplayName())
                {
                    blueTeamNames[j].text = "Waiting For Player...";
                    break;
                }
            }
        }

        if (team == "Blue Team")
        {
            if (bluePlayers >= 3 || localPlayer.TeamName == "Blue Team") return;

            localPlayer.CmdSetTeamName("Blue Team");

            for (int j = 0; j < redTeamNames.Length; j++)
            {
                if (redTeamNames[j].text == localPlayer.GetDisplayName())
                {
                    redTeamNames[j].text = "Waiting For Player...";
                    break;
                }
            }
        }
        UpdateNameLists();

        //if (team == "Red Team")
        //{
        //    for (int i = 0; i < teamManager.blueTeam.Length; i++)
        //    {
        //        if (teamManager.blueTeam[i] == localPlayer.GetDisplayName())
        //        {
        //            for (int j = 0; j < teamManager.redTeam.Length; j++)
        //            {
        //                if (teamManager.redTeam[j] == "")
        //                {
        //                    teamManager.blueTeam[i] = "";
        //                    teamManager.redTeam[j] = localPlayer.GetDisplayName();
        //                    UpdateNameLists();
        //                    return;
        //                }
                            
        //            }
        //            return;
        //        }
        //    }
        //}
        //else if (team == "Blue Team")
        //{
        //    for (int i = 0; i < teamManager.redTeam.Length; i++)
        //    {
        //        if (teamManager.redTeam[i] == localPlayer.GetDisplayName())
        //        {
        //            for (int j = 0; j < teamManager.blueTeam.Length; j++)
        //            {
        //                if (teamManager.blueTeam[j] == "")
        //                {
        //                    teamManager.redTeam[i] = "";
        //                    teamManager.blueTeam[j] = localPlayer.GetDisplayName();
        //                    UpdateNameLists();
        //                    return;
        //                }

        //            }
                    
        //            return;
        //        }
        //    }
        //}
        Debug.Log("Changing player to " + team);
    }

    [ClientRpc]
    public void UpdateNameLists()
    {
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        ((MyNetworkManager)NetworkManager.singleton).ClearMenuPlayers();

        foreach (MyNetworkPlayer player in players)
        {
            menuPlayers.Add(new string[] { player.GetDisplayName(), player.TeamName });
        }

        foreach (string[] listItem in menuPlayers)
        {
            Debug.Log($"{listItem[0]} {listItem[1]}");
        }

        ((MyNetworkManager)NetworkManager.singleton).MenuPlayers = menuPlayers;

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

        //Debug.Log("Red Team Count: " + teamManager.redTeam.Length);

        //for (int i = 0; i < teamManager.redTeam.Length; i++)
        //{
        //    if (teamManager.redTeam[i] != "")
        //        redTeamNames[i].text = teamManager.redTeam[i];
        //    else
        //        redTeamNames[i].text = "Waiting For Player...";
        //}

        //Debug.Log("Blue Team Count: " + teamManager.blueTeam.Length);

        //for (int i = 0; i < teamManager.blueTeam.Length; i++)
        //{
        //    if (teamManager.blueTeam[i] != "")
        //        blueTeamNames[i].text = teamManager.blueTeam[i];
        //    else
        //        blueTeamNames[i].text = "Waiting For Player...";
        //}

    }
}
