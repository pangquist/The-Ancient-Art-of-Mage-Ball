using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";
    [SerializeField] Color playerColour = Color.white;

    [SerializeField] TMP_Text displayNameText = null;

    [SerializeField] TMP_Text redScoreText;
    [SerializeField] TMP_Text blueScoreText;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    public TMP_Text BlueScore { get { return blueScoreText; } set { blueScoreText = value; } }
    public TMP_Text RedScore { get { return redScoreText; } set { redScoreText = value; } }


    private int teamNumber;

    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    ulong steamId;
    
    public int TeamNumber   // property
    {
        get { return teamNumber; }   // get method
        set { teamNumber = value; }  // set method
    }
    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    #region Server

    [Server]
    public void SetSteamId(ulong _steamId)
    {
        Debug.Log("Sent in steam ID: " + _steamId);
        steamId = _steamId;
    }

    public string GetDisplayName()
    {
        Debug.Log("Getting name: " + displayName);
        return displayName;
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

    [Server]
    public void SetPlayerColor(Color newColour)
    {
        playerColour = newColour;
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

        Debug.Log("Setting display name on server: " + newDisplayName);
        RpcLogNewName(newDisplayName);

        SetDisplayName(newDisplayName);
    }
    #endregion
    #region Client

    public override void OnStartClient()
    {
        if(gameObject.GetComponent<CountdownTimer>() != null)
            gameObject.GetComponent<CountdownTimer>().enabled = true;

        if (NetworkServer.active)
            return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);
        gameObject.GetComponent<Animator>().enabled = true;
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!hasAuthority)
            return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Remove(this);
    }

    //Hook method that is called whenever the steam ID of the client is updated. Starts a method that finds the steam name that is connected to that steam id.
    void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    {
        var CSteamID = new CSteamID(newSteamId);
        CmdSetDisplayName(SteamFriends.GetFriendPersonaName(CSteamID));
        Debug.Log("Display Name has been set to: " + displayName);
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
            return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void HandlePlayerNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
        displayNameText.text = displayName;
    }
    
    [ClientRpc]
    void RpcLogNewName(string newDisplayName)
    {
        Debug.Log(newDisplayName);
    }
    #endregion

}
