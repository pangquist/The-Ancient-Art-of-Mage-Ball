using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] TeamManager teamManager;

    bool ballIsSpawned = false;
    [SerializeField] GameObject mainMenuPlayer;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject ballStartPos;
    [SerializeField] GameObject[] characters;
    [SerializeField] public int playersRequiredToStart = 1;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    bool isGameInProgress;
    public static bool timeIsStarted = false; //ÄNDRA
    public static MyNetworkPlayer connectedPlayer;


    public List<MyNetworkPlayer> Players { get; } = new List<MyNetworkPlayer>();
    int chosenCharacter = 0;

    public int ChosenCharacter { get { return chosenCharacter; } set { chosenCharacter = value; } }

    //If the player attemts to connect while the game is in progress, they are disconnected.
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress)
            return;
        conn.Disconnect();
    }

    //Removes the player from the list of active players so they wont be included in future code-interactions
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
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
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }

    //When the player enters the server, this method sets the steam ID of that player and subsequentially gets all the information from that ID.
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(MainMenu.LobbyId, numPlayers - 1);
        Players.Add(player);
        player.SetSteamId(steamId.m_SteamID);
        
        GameObject playerGameObject = conn.identity.gameObject;
        player.SetPartyOwner(Players.Count == 1);
    }

    //Called whenever a scene is changed. The players spawns a player prefabs that is decided in the character select. If the scene is an arena map, the ball is spawned and the game begins.
    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == "Playground")
        {
            teamManager.AssignTeam();
            playerPrefab = characters[chosenCharacter]; //Here is where it is decided what character the player will spawn in as. Make it work with character select in lobby!

            ballStartPos = GameObject.Find("BallSpawnPosition");
            ball = Instantiate(ball, ballStartPos.transform.position, ballStartPos.transform.rotation);
            NetworkServer.Spawn(ball.gameObject);
            ballIsSpawned = true;
        }
        else if (sceneName == "PostMatch")
        {
            playerPrefab = mainMenuPlayer;
        }
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }
}
