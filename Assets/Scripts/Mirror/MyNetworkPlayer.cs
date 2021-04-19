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
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action OnClientTeamUpdated;
    public static event Action ClientOnInfoUpdated;

    [SyncVar (hook = nameof(HandlePlayerTeamAssigned))]
    [SerializeField] string teamName;

    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    bool isPartyOwner = false;

    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    ulong steamId;
    
    //[SyncVar(hook = nameof(HandlePlayerColorUpdated))]
    [SerializeField] Color playerColour = Color.white;

    [SerializeField] TMP_Text displayNameText = null;
    [SerializeField] TMP_Text redScoreText;
    [SerializeField] TMP_Text blueScoreText;
    [SerializeField] TMP_Text timeText;

    [SerializeField] GamestateManager gamestateManager;
    
    public TMP_Text BlueScore { get { return blueScoreText; } set { blueScoreText = value; } }
    public TMP_Text RedScore { get { return redScoreText; } set { redScoreText = value; } }
    public TMP_Text TimeText { get { return timeText; } set { timeText = value; } }
    public string TeamName { get { return teamName; } }


    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    void SetTimerText()
    {
        
        float minutes = Mathf.FloorToInt(gamestateManager.Timer / 60);
        float seconds = Mathf.FloorToInt(gamestateManager.Timer % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void SetScoreText()
    {
        redScoreText.text = "Red: " + gamestateManager.RedScore.ToString();
        blueScoreText.text = "Blue: " + gamestateManager.BlueScore.ToString();
    }

    #region Server

    [Server]
    public void SetSteamId(ulong _steamId)
    {
        Debug.Log("3. Sent in steam ID: " + _steamId);
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
        Debug.Log($"10. The name on the server has been changed to: {displayName}");
    }

    [Server]
    public void SetTeamName(string newTeamName)
    {
        Debug.Log($"9. The player is being assigned to the: {newTeamName} on the server");
        teamName = newTeamName;
        RpcSetTeamName(newTeamName);
    }
    
    [ClientRpc]
    public void RpcSetTeamName(string newTeamName)
    {
        Debug.Log($"Changing {displayName}'s team to {newTeamName} for all clients");
        teamName = newTeamName;
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
        //if (newDisplayName.Length < 2 || newDisplayName.Length > 15)
        //    return;
        

        Debug.Log($"5. Sending a command to the server to set the new name on the server: " + newDisplayName);
        RpcLogNewName(newDisplayName);

        SetDisplayName(newDisplayName);
    }
    
    [Command]
    public void CmdSetTeamName(string newTeamName)
    {
        Debug.Log($"8. Sending a command to the server that the player is being assigned to the: {newTeamName}");
        SetTeamName(newTeamName);
    }
    #endregion
    #region Client

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;
        gamestateManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
        GamestateManager.HandleTimeChanged += SetTimerText;
        GamestateManager.HandleScoreChanged += SetScoreText;
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active)
            return;
        
        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);
        //gameObject.GetComponent<Animator>().enabled = true;
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!hasAuthority)
            return;

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
            Debug.Log($"4. The steam ID has been updated, starting to set Display name to: { SteamFriends.GetFriendPersonaName(CSteamID) }");
            CmdSetDisplayName(SteamFriends.GetFriendPersonaName(CSteamID));
        }
        else
        {
            CmdSetDisplayName($"Player 1");
        }
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
            return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void HandlePlayerNameUpdated(string oldName, string newName)
    {
        Debug.Log($"6. Handling that the name has been changed on the server!");
        ClientOnInfoUpdated?.Invoke();
        displayNameText.text = displayName;
    }
    
    private void HandlePlayerTeamAssigned(string oldTeam, string newTeam)
    {
        if (teamName == "Red Team")
        {
            //playerColour = Color.red;
            displayNameText.color = Color.red;
            //CmdSetColor(Color.red);
        }
        else
        {
            //playerColour = Color.blue;
            displayNameText.color = Color.blue;
            //CmdSetColor(Color.blue);
        }

        OnClientTeamUpdated?.Invoke();
        Debug.Log($"Setting {displayName} text color to: {teamName}");
        //CmdChangeTeamColor(displayNameText.color);
    }

    //[Command]
    //void CmdSetColor(Color color)
    //{
    //    displayNameText.color = color;
    //    RpcSetColor(color);
    //}

    //[ClientRpc]
    //void RpcSetColor(Color color)
    //{
    //    displayNameText.color = color;
    //}
    //private void HandlePlayerColorUpdated(Color oldColor, Color newColor)
    //{
    //    CmdChangeTeamColor(playerColour);
    //}

    //[Command]
    //private void CmdChangeTeamColor(Color _color)
    //{
    //    Debug.Log("Sending a command to change the color on the server");
    //    displayNameText.color = _color;
    //}

    //[Server]
    //private void SetTeamColor (Color _color)
    //{
    //    Debug.Log("The server is trying to set the color");
    //    RpcSetTeamColor(_color);
    //}

    //[ClientRpc]
    //private void RpcSetTeamColor (Color _color)
    //{
    //    displayNameText.color = _color;
    //}

    [ClientRpc]
    void RpcLogNewName(string newDisplayName)
    {
        Debug.Log($" This is the RPC sending out the new name: {newDisplayName}");
    }
    #endregion

}
