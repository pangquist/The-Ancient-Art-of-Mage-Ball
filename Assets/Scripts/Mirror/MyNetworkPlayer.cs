using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyNetworkPlayer : NetworkBehaviour
{
    //Events.
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action OnClientTeamUpdated;
    public static event Action ClientOnInfoUpdated;

    //Handlers.
    [SyncVar (hook = nameof(HandlePlayerTeamAssigned))]
    [SerializeField] string teamName;

    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    bool isPartyOwner = false;

    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    ulong steamId;

    [SyncVar(hook = nameof(HandlePlayerColorUpdated))]
    [SerializeField] Color playerColor = Color.white;

    //Canvas elements for nametag and UI.
    [SerializeField] TMP_Text displayNameText = null;
    [SerializeField] TMP_Text redScoreText;
    [SerializeField] TMP_Text blueScoreText;
    [SerializeField] TMP_Text timeText;

    //Gamestate Manager.
    [SerializeField] GamestateManager gamestateManager;
    
    //Getters and setters.
    public TMP_Text BlueScore { get { return blueScoreText; } set { blueScoreText = value; } }
    public TMP_Text RedScore { get { return redScoreText; } set { redScoreText = value; } }
    public TMP_Text TimeText { get { return timeText; } set { timeText = value; } }
    public string TeamName { get { return teamName; } }

    //Returns whether the player is host or not.
    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    //Returns the name of the player.
    public string GetDisplayName()
    {
        return displayName;
    }

    //Updates the timer in the UI.
    void SetTimerText()
    {
        float minutes = Mathf.FloorToInt(gamestateManager.Timer / 60);
        float seconds = Mathf.FloorToInt(gamestateManager.Timer % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    //Updates the score in the UI.
    void SetScoreText()
    {
        redScoreText.text = $"Red: {gamestateManager.RedScore.ToString()}";
        blueScoreText.text = $"Blue: {gamestateManager.BlueScore.ToString()}";
    }

    #region Server

    //Sets the steam ID of a player.
    [Server]
    public void SetSteamId(ulong _steamId)
    {
        //Debug.Log("3. Sent in steam ID: " + _steamId);
        steamId = _steamId;

        if (steamId == 0) //THIS SHOULD BE REMOVED WHEN TESTING FOR REAL
        {
            steamId = 1;
        }
    }

    //Sets a player as host of the lobby.
    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    //Sets the display name of a player.
    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
        //Debug.Log($"10. The name on the server has been changed to: {displayName}");
    }

    //Sets the team name of a player.
    [Server]
    void ServerSetTeamName(string newTeamName)
    {
        teamName = newTeamName;
    }

    //Sets the color of the players name tag based on team.
    [Server]
    void ServerSetTextColor(Color color)
    {
        Debug.Log($"Updating {displayName}'s team color to: {color}!");
        displayNameText.color = color;
    }

    //Starts a game from the lobby.
    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner)
            return;

        ((MyNetworkManager)NetworkManager.singleton).StartGame();
    }

    //Sends a command to the server telling it to update the display name of the client.
    [Command]
    void CmdSetDisplayName(string newDisplayName)
    {
        //Debug.Log($"5. Sending a command to the server to set the new name on the server: " + newDisplayName);
        RpcLogNewName(newDisplayName);

        SetDisplayName(newDisplayName);
    }
    
    //Command for the server to set a players team name.
    [Command]
    public void CmdSetTeamName(string newTeamName)
    {
        ServerSetTeamName(newTeamName);
    }

    //Command for the server to set a players name tag color.
    [Command]
    void CmdSetTextColor(Color color)
    {
        ServerSetTextColor(color);
    }
    #endregion
    #region Client

    //Assigns handlers to methods and finds the gamestate manager when the player gameobject is started.
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }   

        gamestateManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
        GamestateManager.HandleTimeChanged += SetTimerText;
        GamestateManager.HandleScoreChanged += SetScoreText;
    }

    //Runs when a new client connects.
    public override void OnStartClient()
    {
        if (NetworkServer.active)
        {
            return;
        }     

        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);
        //gameObject.GetComponent<Animator>().enabled = true;
    }

    //Runs when a client disconnects.
    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!hasAuthority)
        {
            return;
        }   

        GamestateManager.HandleTimeChanged -= SetTimerText;
        GamestateManager.HandleScoreChanged -= SetScoreText;
        ((MyNetworkManager)NetworkManager.singleton).Players.Remove(this);
    }

    //Hook method that is called whenever the steam ID of the client is updated. Starts a method that finds the steam name that is connected to that steam id.
    [Client]
    void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (steamId != 1)
        {
            var CSteamID = new CSteamID(newSteamId);
            //Debug.Log($"4. The steam ID has been updated, starting to set Display name to: { SteamFriends.GetFriendPersonaName(CSteamID) }");
            CmdSetDisplayName(SteamFriends.GetFriendPersonaName(CSteamID));
        }
        else
        {
            CmdSetDisplayName($"Player 1");
        }
    }

    //Hook method that is called whenever the lobby host is updated.
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
        {
            return;
        }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    //Hook method that is called whenever the player display name is set.
    [Client]
    private void HandlePlayerNameUpdated(string oldName, string newName)
    {
        Debug.Log($"6. Handling that the name has been changed on the server!");
        ClientOnInfoUpdated?.Invoke();
        displayNameText.text = displayName;
    }
    
    //Sets the team name on the client.
    public void SetTeamName(string name)
    {
        Debug.Log($"Changing team from: {teamName} to: {name} on the client!");
        teamName = name;
        CmdSetTeamName(name);
    }

    //Hook method that is called whenever a player's team is set.
    [Client]
    private void HandlePlayerTeamAssigned(string oldTeam, string newTeam)
    {
        Debug.Log($"Handling that the team name is being changed on the client!");
        if (teamName == "Red Team")
        {
            displayNameText.color = Color.red;
            CmdSetTextColor(Color.red);
        }
        else
        {
            displayNameText.color = Color.blue;
            CmdSetTextColor(Color.blue);
        }

        OnClientTeamUpdated?.Invoke();
        Debug.Log($"Setting {displayName} text color to: {teamName}");
    }

    //Hook method that is called whenever a player's nametag color is set.
    [Client]
    void HandlePlayerColorUpdated(Color oldColor, Color newColor)
    {
        Debug.Log("The clients color has been updated!");
        CmdSetTextColor(playerColor);
    }

    //Client RPC that logs a player's assigned name to all clients.
    [ClientRpc]
    void RpcLogNewName(string newDisplayName)
    {
        //Debug.Log($" This is the RPC sending out the new name: {newDisplayName}");
    }

    //Client RPC that sets the team name for a player on all clients.
    [ClientRpc]
    public void RpcSetTeamName(string newTeamName)
    {
        teamName = newTeamName;
        Debug.Log($"Setting a new team name for {displayName}: {teamName} on every client!");
    }
    #endregion

}
