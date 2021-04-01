using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] TeamManager teamManager;

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
        //playerGameObject.GetComponent<MeshRenderer>().material.color = displayColour;
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
    }
}
