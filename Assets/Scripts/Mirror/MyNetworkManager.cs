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

    bool timeIsStarted = false;

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
        }
        Debug.Log(player.TeamNumber);

        if(ballIsSpawned == false)
        {
            ball = Instantiate(ball, ballStartPos.position, ballStartPos.rotation);
            ballIsSpawned = true;
        }

        NetworkServer.Spawn(ball.gameObject);
    }
    //public override void OnClientSceneChanged(NetworkConnection conn)
    //{
    //    if (SceneManager.GetActiveScene().name.StartsWith("Arena"))
    //    {
    //        ball = Instantiate(ball, ballStartPos.position, ballStartPos.rotation);

    //        NetworkServer.Spawn(ball.gameObject);

    //        foreach (RTSPlayer player in players)
    //        {
    //            GameObject baseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);
    //            NetworkServer.Spawn(baseInstance, player.connectionToClient);
    //        }
    //    }
    //}
}
