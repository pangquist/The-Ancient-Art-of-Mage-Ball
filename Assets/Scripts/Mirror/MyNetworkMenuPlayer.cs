using Mirror;
using System;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkMenuPlayer : NetworkBehaviour
{
    // This script stores and syncronizes information regarding the player during the menu scene.
    // It also functions as a general tool to relay information regarding map-selectiona and character-selection.
    // The script also describes what happens when a client or host disconnects from the server.
    // Authors: Valter Lindecrantz.

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action OnClientTeamUpdated;
    public static event Action ClientOnInfoUpdated;
    public static event Action ClientOnCharacterUpdated;

    [SyncVar(hook = nameof(HandlePlayerTeamAssigned))]
    [SerializeField] string teamName;

    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    bool isPartyOwner = false;

    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    ulong steamId;

    [SyncVar(hook = nameof(HandlePlayerColorUpdated))]
    [SerializeField] Color playerColor = Color.white;

    [SyncVar(hook = nameof(HandlePlayerCharacterUpdated))]
    [SerializeField] int chosenCharacter;

    [SerializeField] TMP_Text displayNameText = null;
    [SerializeField] TMP_Text redScoreText;
    [SerializeField] TMP_Text blueScoreText;
    [SerializeField] TMP_Text timeText;

    [SerializeField] GamestateManager gamestateManager;

    public TMP_Text BlueScore { get { return blueScoreText; } set { blueScoreText = value; } }
    public TMP_Text RedScore { get { return redScoreText; } set { redScoreText = value; } }
    public TMP_Text TimeText { get { return timeText; } set { timeText = value; } }
    public string TeamName { get { return teamName; } }
    public bool IsPartyOwner { get { return isPartyOwner; } }
    public int ChosenCharacter { get { return chosenCharacter; } set { chosenCharacter = value; } }
    public ulong SteamId { get { return steamId; } }


    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    #region Server

    // Gives the player the correct steam ID, which is passed in as a parameter from the NetworkManager.
    [Server]
    public void SetSteamId(ulong _steamId)
    {
        steamId = _steamId;

        if (steamId == 0) //THIS SHOULD BE REMOVED WHEN TESTING FOR REAL
        {
            steamId = 1;
        }
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

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
        SetDisplayName(newDisplayName);
    }

    [Command]
    public void CmdSetTeamName(string newTeamName)
    {
        ServerSetTeamName(newTeamName);
    }

    [Server]
    void ServerSetTeamName(string newTeamName)
    {
        teamName = newTeamName;
    }
    #endregion
    #region Client

    public override void OnStartClient()
    {
        if (NetworkServer.active)
            return;

        SceneSelect.OnMenuBackgroundUpdated += HandleSceneChanged;

        ((MyNetworkManager)NetworkManager.singleton).MenuPlayers.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!hasAuthority)
            return;

        SceneSelect.OnMenuBackgroundUpdated -= HandleSceneChanged;
        ((MyNetworkManager)NetworkManager.singleton).MenuPlayers.Remove(this);
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
            CmdSetDisplayName(SteamFriends.GetFriendPersonaName(CSteamID));
        }
        else
        {
            CmdSetDisplayName($"Player 1");
        }
    }

    [Client]
    void HandleSceneChanged()
    {
        Debug.Log("Scene has been changed!");
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
            return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    // When the players name is updated to match their steam name, the text above their characters change to reflect their new name.
    // An event is also called which calls methods in other scripts where they are subscribed to the event.
    [Client]
    private void HandlePlayerNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
        displayNameText.text = displayName;
    }

    // Sets the team name based on what button the player presses, and calls a command to the server to change the team name aswell.
    public void SetTeamName(string name)
    {
        teamName = name;
        CmdSetTeamName(name);
    }
    
    // When a players team name change, their player color should change aswell to visually display what team the player is on.
    private void HandlePlayerTeamAssigned(string oldTeam, string newTeam)
    {
        if (hasAuthority)
        {
            if (teamName == "Red Team")
            {
                CmdSetPlayerColor(Color.red);
            }
            else
            {
                CmdSetPlayerColor(Color.blue);
            }
           
        }

        OnClientTeamUpdated?.Invoke();
    }

    // Command that calls on the server to change the players color.
    [Command]
    void CmdSetPlayerColor(Color color)
    {
        ServerSetPlayerColor(color);
    }

    [Server]
    void ServerSetPlayerColor(Color color)
    {
        playerColor = color;
    }

    //When the players color is updated, a command is called to change the color of the text above the players character.
    [Client]
    void HandlePlayerColorUpdated(Color oldColor, Color newColor)
    {
        if (!hasAuthority)
        {
            return;
        }

        CmdSetTextColor();
    }

    // Command that tells the server to set the text color for the player.
    [Command]
    void CmdSetTextColor()
    {
        ServerSetTextColor();
    }

    // The players text color is updated on the server to reflect what team the player is on.
    [Server]
    void ServerSetTextColor()
    {
        Debug.Log($"Updating {displayName}'s team color to: {playerColor}!");
        displayNameText.color = playerColor;
    }

    // Sends a command to the server to tell the server that the player has chosen a character in the character select.
    [Command]
    public void CmdUpdateChosenCharacter(int charactedIndex)
    {
        chosenCharacter = charactedIndex;
    }

    [Client]
    void HandlePlayerCharacterUpdated(int oldValue, int newValue)
    {
        ClientOnCharacterUpdated?.Invoke();
    }
    #endregion
}
