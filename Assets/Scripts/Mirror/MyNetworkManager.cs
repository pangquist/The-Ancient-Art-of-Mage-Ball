using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    // This script controls what happens on the server and the client when a client connects.
    // It also functions as a general tool to relay information regarding map-selectiona and character-selection.
    // The script also describes what happens when a client or host disconnects from the server.
    // Authors: Pär Ängqvist & Valter Lindecrantz

    public static int playersRequiredToStart = 1;
    public static MyNetworkPlayer connectedPlayer;
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    [Header("Script Dependencies")]
    [SerializeField] TeamManager teamManager;
    [SerializeField] GamestateManager gamestateManager;

    [Header("References")]
    [SerializeField] GameObject mainMenuPlayer;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject ballStartPos;
    [SerializeField] GameObject lobby;

    [Header("Characters")]
    [SerializeField] GameObject[] characters;
    
    bool isGameInProgress;

    public List<MyNetworkMenuPlayer> MenuPlayers { get; } = new List<MyNetworkMenuPlayer>();
    public List<string[]> CharacterInfoList = new List<string[]>();
    public GameObject[] Characters { get { return characters; } }
    public List<MyNetworkPlayer> Players { get; } = new List<MyNetworkPlayer>();
    public List<NetworkPlayerSpawner> Spawners { get; } = new List<NetworkPlayerSpawner>();
    public string SelectedScene { get { return SelectedScene; } set { SelectedScene = value; } }


    public void ClearMenuPlayers()
    {
        MenuPlayers.Clear();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStopServer()
    {
        MenuPlayers.Clear();
        Players.Clear();

        isGameInProgress = false;
    }

    // Checks if the requirements to start the game are fullfilled. If they are, the scene is switched to the currently selected scene.
    // For every player in the menu, it also stores that player information regarding name, team and character to be used when spawned in the game.
    [Server]
    public void StartGame()
    {
        if (MenuPlayers.Count < playersRequiredToStart)
            return;

        foreach (MyNetworkMenuPlayer menuPlayer in MenuPlayers)
        {
            string[] characterInfo = new string[4];
            characterInfo[0] = menuPlayer.SteamId.ToString();
            characterInfo[1] = menuPlayer.GetDisplayName();
            characterInfo[2] = menuPlayer.TeamName;
            characterInfo[3] = menuPlayer.ChosenCharacter.ToString();

            CharacterInfoList.Add(characterInfo);
        }

        isGameInProgress = true;

        ServerChangeScene(SelectedScene);
    }
    
    [Server]
    public void EndGame()
    {
        ServerChangeScene("PostMatch");
    }
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        
        ClientOnDisconnected?.Invoke();
    }

    //If the player attemts to connect while the game is in progress, they are disconnected.
    public override void OnServerConnect(NetworkConnection conn)
    {
        //Debug.Log("1. Trying to connect to the server");
        if (!isGameInProgress)
            return;
        conn.Disconnect();
    }

    //Removes the player from the list of active players so they wont be included in future code-interactions
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Players.Remove(conn.identity.GetComponent<MyNetworkPlayer>());
        MenuPlayers.Remove(conn.identity.GetComponent<MyNetworkMenuPlayer>());
        OnServerSceneChanged("MainMenu");

        base.OnServerDisconnect(conn);
    }

    //When the player enters the server, this method sets the steam ID of that player and subsequentially gets all the information from that ID.
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        if (conn.identity.tag == "MenuPlayer")
        {
            MyNetworkMenuPlayer menuPlayer = conn.identity.GetComponent<MyNetworkMenuPlayer>();
            MenuPlayers.Add(menuPlayer);
        
            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(MainMenu.LobbyId, numPlayers - 1);
            menuPlayer.SetSteamId(steamId.m_SteamID);
            
            menuPlayer.SetPartyOwner(MenuPlayers.Count == 1);
        }
        else if (conn.identity.tag == "CharacterSelecter") //Old was "Player"
        {
            NetworkPlayerSpawner spawner = conn.identity.GetComponent<NetworkPlayerSpawner>();
            Spawners.Add(spawner);
            spawner.SetGamestateManager(gamestateManager);
        }
    }

    //Called whenever a scene is changed. The players spawns a player prefabs that is decided in the character select. If the scene is an arena map, the ball is spawned and the game begins.
    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == SelectedScene)
        {
            playerPrefab = Characters[0]; //Here is where it is decided what character the player will spawn in as. Make it work with character select in lobby!
            ballStartPos = GameObject.Find("BallSpawnPosition");

            GameObject instantiatedBall;
            instantiatedBall = Instantiate(ball, ballStartPos.transform.position, ballStartPos.transform.rotation);
            NetworkServer.Spawn(instantiatedBall.gameObject);
        }
        else if (sceneName == "PostMatch")
        {
            playerPrefab = mainMenuPlayer;
            SelectedScene = "UnderwaterRuin";
        }
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        gamestateManager.matchIsOver = false;
        gamestateManager.matchIsPaused = true;
        gamestateManager.FillSpawnpointList();
        base.OnClientSceneChanged(conn);

        if (SceneManager.GetActiveScene().name== "PostMatch")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gamestateManager.AssignScoreAtPostScreen();
        }
    }

    public override void OnStopClient()
    {
        Players.Clear();
        MenuPlayers.Clear();
    }
    
    public override void OnStopHost()
    {
        base.OnStopHost();
    }
}
