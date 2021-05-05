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
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action OnClientTeamUpdated;
    public static event Action ClientOnInfoUpdated;

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

    [SyncVar(hook = nameof(HandleChosenCharacterUpdated))]
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
        Debug.Log("Setting steam ID!!");

        steamId = _steamId;

        //if (steamId == 0) //THIS SHOULD BE REMOVED WHEN TESTING FOR REAL
        //{
        //    steamId = 1;
        //}
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
        Debug.Log("Before return!");

        if (NetworkServer.active)
            return;

        Debug.Log("After return!");

        ((MyNetworkManager)NetworkManager.singleton).MenuPlayers.Add(this);
        //gameObject.GetComponent<Animator>().enabled = true;
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!hasAuthority)
            return;

        GamestateManager.HandleTimeChanged -= SetTimerText;
        GamestateManager.HandleScoreChanged -= SetScoreText;
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

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
            return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    [Client]
    private void HandlePlayerNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
        displayNameText.text = displayName;
    }

    public void SetTeamName(string name)
    {
        teamName = name;
        CmdSetTeamName(name);
    }
    
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

    [Client]
    void HandlePlayerColorUpdated(Color oldColor, Color newColor)
    {
        if (!hasAuthority)
        {
            return;
        }

        CmdSetTextColor();
    }

    [Command]
    void CmdSetTextColor()
    {
        ServerSetTextColor();
    }

    [Server]
    void ServerSetTextColor()
    {
        Debug.Log($"Updating {displayName}'s team color to: {playerColor}!");
        displayNameText.color = playerColor;
    }

    [Command]
    public void CmdUpdateChosenCharacter(int charactedIndex)
    {
        chosenCharacter = charactedIndex;
    }

    [Client]
    void HandleChosenCharacterUpdated(int oldCharacter, int newCharacter)
    {
        if (!hasAuthority)
        {
            return;
        }

        CmdSetChosenCharacter();
    }

    [Command]
    void CmdSetChosenCharacter()
    {
        ServerSetChosenCharacter();
    }

    [Server]
    void ServerSetChosenCharacter()
    {
        Debug.Log($"Updating {displayName}'s chosen character to: {chosenCharacter}!");
    }


    #endregion
}
