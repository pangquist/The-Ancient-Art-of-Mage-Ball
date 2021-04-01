using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
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
            player.TeamNumber = 1;
        }
        else
        {
            player.TeamNumber = 2;
        }
        Debug.Log(player.TeamNumber);
    }
}
