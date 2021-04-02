using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] TeamManager teamManager;

    bool ballIsSpawned = false;
    [SerializeField] GameObject ball;
    [SerializeField] Transform ballStartPos;

    public static bool timeIsStarted = false; //ÄNDRA

    public static List<MyNetworkPlayer> players = new List<MyNetworkPlayer>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        GameObject playerGameObject = conn.identity.gameObject;
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        player.SetDisplayName("Player " + numPlayers);

        Color displayColour = new Color(
            Random.Range(0f, 1f), 
            Random.Range(0f, 1f), 
            Random.Range(0f, 1f));

        player.SetPlayerColor(displayColour);
        Debug.Log("Player number: " + numPlayers + " has joined the server!");

        if (numPlayers < 2)
        {
            teamManager.team1.Add(player);
        }
        else
        {
            teamManager.team2.Add(player);
            timeIsStarted = true;
        }
        Debug.Log(player.TeamNumber);

        if(ballIsSpawned == false)
        {
            ball = Instantiate(ball, ballStartPos.position, ballStartPos.rotation);
            ballIsSpawned = true;
        }
        players.Add(player);

        Debug.Log("players have " + numPlayers + " in its list");
        NetworkServer.Spawn(ball.gameObject);
    }
}
