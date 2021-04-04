using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] TeamManager teamManager;

    bool ballIsSpawned = false;
    [SerializeField] GameObject ball;
    [SerializeField] GameObject ballStartPos;
    [SerializeField] GameObject[] characters;
    [SerializeField] public int playersRequiredToStart = 1;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    bool isGameInProgress;
    public static bool timeIsStarted = false; //ÄNDRA


    public List<MyNetworkPlayer> Players { get; } = new List<MyNetworkPlayer>();
    //[SerializeField] int chosenCharacter = 0;

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress)
            return;
        conn.Disconnect();
    }

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
    
    [Server]
    public void StartGame()
    {
        if (Players.Count < playersRequiredToStart)
            return;

        isGameInProgress = true;

        ServerChangeScene("Playground");
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

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        GameObject playerGameObject = conn.identity.gameObject;
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        Players.Add(player);
        player.SetPartyOwner(Players.Count == 1);
        Debug.Log("We set the Party Owner");
        player.SetDisplayName("Player " + numPlayers);
        Debug.Log("We set the Player name");


        //if (SceneManager.GetActiveScene().name.StartsWith("Main"))
        //    return;

        //Color displayColour = new Color(
        //    UnityEngine.Random.Range(0f, 1f),
        //    UnityEngine.Random.Range(0f, 1f),
        //    UnityEngine.Random.Range(0f, 1f));

        //player.SetPlayerColor(displayColour);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("Scene is being changed!");
        playerPrefab = characters[0]; //Here is where it is decided what character the player will spawn in as. Make it work with character select in lobby!

        Debug.Log("Current prefab: " + playerPrefab);

        if (SceneManager.GetActiveScene().name.StartsWith("Play"))
        {
            Debug.Log("Playground is the changed scene!");
            //if (numPlayers < 2)
            //{
            //    teamManager.team1.Add(player);
            //}
            //else
            //{
            //    teamManager.team2.Add(player);
            //    timeIsStarted = true;
            //}
            //Debug.Log(player.TeamNumber);

            if (ballIsSpawned == false)
            {
                ballStartPos = GameObject.Find("BallSpawnPosition");
                ball = Instantiate(ball, ballStartPos.transform.position, ballStartPos.transform.rotation); //HARD CODED CHANGE LATER
                NetworkServer.Spawn(ball.gameObject);
                ballIsSpawned = true;
            }
        }
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }
}
