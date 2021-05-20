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

    [SerializeField] GamestateManager gamestateManager;

    [SerializeField] TMP_Text[] redTeamNames = new TMP_Text[3];
    [SerializeField] TMP_Text[] blueTeamNames = new TMP_Text[3];
    
    private List<string[]> menuPlayers = new List<string[]>();


    [SerializeField] int redPlayers = 0;
    [SerializeField] int bluePlayers = 0;

    [SerializeField] GameObject[] mapButtons;
    [SerializeField] Sprite[] characterSprites;
    [SerializeField] Image[] redTeamImage = new Image[3];
    [SerializeField] Image[] blueTeamImage = new Image[3];

    private void Start()
    {
        Debug.Log("Lobbymenu started! Subscribing to events!");
        MyNetworkManager.ClientOnConnected += HandleClientConnected;
        MyNetworkMenuPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        MyNetworkMenuPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        MyNetworkMenuPlayer.OnClientTeamUpdated += UpdateNameLists;
        MyNetworkMenuPlayer.ClientOnCharacterUpdated += UpdateNameLists;
    }

    private void OnDestroy()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientConnected;
        MyNetworkMenuPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        MyNetworkMenuPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        MyNetworkMenuPlayer.OnClientTeamUpdated -= UpdateNameLists;
        MyNetworkMenuPlayer.ClientOnCharacterUpdated -= UpdateNameLists;
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
        startGameButton.enabled = state;
        startGameButton.interactable = state;
        startGameButton.gameObject.GetComponent<OnHoverButton>().enabled = state;
        ColorBlock cb = startGameButton.colors;

        cb.normalColor = Color.white;

        startGameButton.colors = cb;
        foreach (GameObject mapButton in mapButtons)
        {
            mapButton.GetComponent<Button>().enabled = true;
            mapButton.GetComponent<OnHoverButton>().enabled = true;
        }
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

        if (NetworkClient.ready)
        {
            if (redPlayers >= 3 && bluePlayers < 3)
            {
                newPlayer.CmdSetTeamName("Blue Team");
            }
            else
            {
                newPlayer.CmdSetTeamName("Red Team");
            }
<<<<<<< HEAD
=======
        }
        else
        {
            Debug.Log("NetworkClient is not ready (LobbyMenu)!");
>>>>>>> main
        }
        

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
    }
    
    public void UpdateNameLists()
    {
        List<MyNetworkMenuPlayer> menuPlayers = ((MyNetworkManager)NetworkManager.singleton).MenuPlayers;

        gamestateManager.ClearPlayerList();

        int numberOfRedPlayer = 0;
        int numberOfBluePlayer = 0;

        foreach (MyNetworkMenuPlayer player in menuPlayers)
        {
            if (player.TeamName == "Red Team")
            {
                numberOfRedPlayer++;
            }

            else if (player.TeamName == "Blue Team")
            {
                numberOfBluePlayer++;
            }
        }

        redPlayers = numberOfRedPlayer;
        bluePlayers = numberOfBluePlayer;


        for (int i = 0; i < redTeamNames.Length; i++)
        {
            redTeamNames[i].text = "Waiting For Player...";
            redTeamImage[i].enabled = false;
        }

        for (int i = 0; i < blueTeamNames.Length; i++)
        {
            blueTeamNames[i].text = "Waiting For Player...";
            blueTeamImage[i].enabled = false;
        }

        for (int i = 0; i < menuPlayers.Count; i++)
        {
            if (menuPlayers[i].TeamName == "Red Team")
            {
                gamestateManager.AddPlayerToTeam(menuPlayers[i], "Red");
                for (int j = 0; j < redTeamNames.Length; j++)
                {
                    if (redTeamNames[j].text == "Waiting For Player...")
                    {
                        redTeamNames[j].text = menuPlayers[i].GetDisplayName();
                        redTeamImage[j].enabled = true;
                        redTeamImage[j].sprite = characterSprites[menuPlayers[i].ChosenCharacter];
                        break;
                    }
                }

            }
            else if (menuPlayers[i].TeamName == "Blue Team")
            {
                gamestateManager.AddPlayerToTeam(menuPlayers[i], "Blue");
                for (int j = 0; j < blueTeamNames.Length; j++)
                {
                    if (blueTeamNames[j].text == "Waiting For Player...")
                    {
                        blueTeamNames[j].text = menuPlayers[i].GetDisplayName();
                        blueTeamImage[j].enabled = true;
                        blueTeamImage[j].sprite = characterSprites[menuPlayers[i].ChosenCharacter];
                        break;
                    }
                }
            }
        }
    }
}
