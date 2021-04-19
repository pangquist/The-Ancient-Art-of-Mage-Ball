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

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    bool isGameInProgress;
    public static bool timeIsStarted = false; //ÄNDRA
    public static MyNetworkPlayer connectedPlayer;

    private List<string[]> menuPlayers = new List<string[]>();

    public List<string[]> MenuPlayers { get { return menuPlayers; } set { menuPlayers = value; } }

    public void ClearMenuPlayers()
    {
        menuPlayers.Clear();
    }

    public List<MyNetworkPlayer> Players { get; } = new List<MyNetworkPlayer>();
    int chosenCharacter = 0;

    public int ChosenCharacter { get { return chosenCharacter; } set { chosenCharacter = value; } }

    public override void OnStartServer()
    {
        base.OnStartServer();

        //GameObject instantiatedLobby;
        //instantiatedLobby = Instantiate(lobby);
        //NetworkServer.Spawn(instantiatedLobby);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }
    
    //Checks if the requirements to start the game are fullfilled.
    [Server]
    public void StartGame()
    {
        if (Players.Count < playersRequiredToStart)
            return;

        isGameInProgress = true;

        foreach (MyNetworkPlayer player in Players)
        {

        }

        ServerChangeScene("Playground");
    }

    [Server]
    public void EndGame()
    {
        ServerChangeScene("PostMatch");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("A client has connected to the server!");
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
        Debug.Log("1. Trying to connect to the server");
        if (!isGameInProgress)
            return;
        conn.Disconnect();
    }

    //Removes the player from the list of active players so they wont be included in future code-interactions
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Players.Remove(conn.identity.GetComponent<MyNetworkPlayer>());

        base.OnServerDisconnect(conn);
    }

    //When the player enters the server, this method sets the steam ID of that player and subsequentially gets all the information from that ID.
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("2. Player has been added to the server!");
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        Players.Add(player);
        
        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(MainMenu.LobbyId, numPlayers - 1);
        player.SetSteamId(steamId.m_SteamID);

        AssignNames();
        //GameObject playerGameObject = conn.identity.gameObject;
        player.SetPartyOwner(Players.Count == 1);
    }

    //Called whenever a scene is changed. The players spawns a player prefabs that is decided in the character select. If the scene is an arena map, the ball is spawned and the game begins.
    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log($"Scene has been changed to: {sceneName}");
        if (sceneName == "Playground")
        {
            playerPrefab = characters[chosenCharacter]; //Here is where it is decided what character the player will spawn in as. Make it work with character select in lobby!
            GamestateManager.gameIsOver = false;
            ballStartPos = GameObject.Find("BallSpawnPosition");

            GameObject instantiatedBall;
            instantiatedBall = Instantiate(ball, ballStartPos.transform.position, ballStartPos.transform.rotation);
            NetworkServer.Spawn(instantiatedBall.gameObject);
            //ballIsSpawned = true;
        }
        else if (sceneName == "PostMatch")
        {
            playerPrefab = mainMenuPlayer;
            gamestateManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
            gamestateManager.AssignScoreAtPostScreen();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        Destroy(gamestateManager.gameObject);
    }

    void AssignNames()
    {
        Debug.Log($"11. Time to assign the players to their teams!");
        foreach(string[] menuPlayer in menuPlayers)
        {
            foreach (MyNetworkPlayer player in Players)
            {
                //if (player.GetDisplayName() == menuPlayer[0])
                //{
                //    player.CmdSetTeamName(menuPlayer[1]);
                //}
                for (int i = 0; i < menuPlayers.Count; i += 2)
                {
                    if (player.GetDisplayName() == menuPlayer[i])
                    {
                        player.CmdSetTeamName(menuPlayer[i+1]);
                        continue;
                    }
                }
            }
               
        }
    }
}
