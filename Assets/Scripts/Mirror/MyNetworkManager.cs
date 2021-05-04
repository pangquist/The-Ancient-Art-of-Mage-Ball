using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    public static int playersRequiredToStart = 1;

    [SerializeField] TeamManager teamManager;
    [SerializeField] GamestateManager gamestateManager;

    //bool ballIsSpawned = false;
    [SerializeField] GameObject mainMenuPlayer;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject ballStartPos;
    [SerializeField] GameObject lobby;
    [SerializeField] GameObject[] characters;
    [SerializeField] string selectedScene = "Playground";

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    bool isGameInProgress;
    public static bool timeIsStarted = false; //ÄNDRA
    public static MyNetworkPlayer connectedPlayer;

    //public List<string[]> menuPlayers = new List<string[]>();

    //public List<string[]> MenuPlayers { get { return menuPlayers; } set { menuPlayers = value; } }

    public GameObject[] Characters { get { return characters; } }

    public List<MyNetworkMenuPlayer> MenuPlayers { get; } = new List<MyNetworkMenuPlayer>();
    public List<string[]> CharacterInfoList = new List<string[]>();

    public List<MyNetworkPlayer> Players { get; } = new List<MyNetworkPlayer>();
    public List<NetworkPlayerSpawner> Spawners { get; } = new List<NetworkPlayerSpawner>();

    public string SelectedScene { get { return selectedScene; } set { selectedScene = value; } }

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
    
    //Checks if the requirements to start the game are fullfilled.
    [Server]
    public void StartGame()
    {
        if (MenuPlayers.Count < playersRequiredToStart)
            return;

        foreach (MyNetworkMenuPlayer menuPlayer in MenuPlayers)
        {
            string[] characterInfo = new string[3];
            characterInfo[0] = menuPlayer.GetDisplayName();
            characterInfo[1] = menuPlayer.TeamName;
            characterInfo[2] = menuPlayer.ChosenCharacter.ToString();

            CharacterInfoList.Add(characterInfo);
        }
        

        isGameInProgress = true;

        ServerChangeScene(selectedScene);
    }
    
    [Server]
    public void EndGame()
    {
        ServerChangeScene("PostMatch");
    }
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log($"A client has connected to the server! It has the connection: {conn}");
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

        Debug.Log("Server has added a player!");

        if (conn.identity.tag == "MenuPlayer")
        {
            Debug.Log("The player was a Menu Player");
            MyNetworkMenuPlayer menuPlayer = conn.identity.GetComponent<MyNetworkMenuPlayer>();
            MenuPlayers.Add(menuPlayer);

            Debug.Log($"Number of players in Menu Players: {MenuPlayers.Count}");

            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(MainMenu.LobbyId, numPlayers - 1);
            menuPlayer.SetSteamId(steamId.m_SteamID);
            
            menuPlayer.SetPartyOwner(MenuPlayers.Count == 1);
        }
        else if (conn.identity.tag == "CharacterSelecter")
        {
            NetworkPlayerSpawner spawner = conn.identity.GetComponent<NetworkPlayerSpawner>();
            Spawners.Add(spawner);
            spawner.AssignCharacterPrefab(Spawners.Count - 1);
        }
    }

    //Called whenever a scene is changed. The players spawns a player prefabs that is decided in the character select. If the scene is an arena map, the ball is spawned and the game begins.
    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == SelectedScene)
        {
            playerPrefab = Characters[0]; //Here is where it is decided what character the player will spawn in as. Make it work with character select in lobby!
            GamestateManager.gameIsOver = false;
            ballStartPos = GameObject.Find("BallSpawnPosition");

            GameObject instantiatedBall;
            instantiatedBall = Instantiate(ball, ballStartPos.transform.position, ballStartPos.transform.rotation);
            NetworkServer.Spawn(instantiatedBall.gameObject);
        }
        else if (sceneName == "PostMatch")
        {
            playerPrefab = mainMenuPlayer;
            gamestateManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
            gamestateManager.AssignScoreAtPostScreen();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            selectedScene = "Playground";
            //Players.Clear();
            //MenuPlayers.Clear();
            //CharacterInfoList.Clear();
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
