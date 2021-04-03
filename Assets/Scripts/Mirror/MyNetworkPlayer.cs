using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";
    [SerializeField] Color playerColour = Color.white;

    [SerializeField] TMP_Text displayNameText = null;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    private int teamNumber;
    public int TeamNumber   // property
    {
        get { return teamNumber; }   // get method
        set { teamNumber = value; }  // set method
    }
    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }
    public string GetDisplayName()
    {
        return displayName;
    }

    #region Server

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

    [Command]
    void CmdSetDisplayName(string newDisplayName)
    {

        if (newDisplayName.Length < 2 || newDisplayName.Length > 15)
            return;

        RpcLogNewName(newDisplayName);

        SetDisplayName(newDisplayName);
    }
    #endregion
    #region Client

    public override void OnStartClient()
    {
        if (NetworkServer.active)
            return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);

        gameObject.GetComponent<CountdownTimer>().enabled = true;
        gameObject.GetComponent<Animator>().enabled = true;
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!hasAuthority)
            return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Remove(this);
    }

    //private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    //{
    //    ClientOnInfoUpdated?.Invoke();
    //}

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

    [ContextMenu("SetMyName")]
    void SetMyName()
    {
        CmdSetDisplayName("M");
    }

    [ClientRpc]
    void RpcLogNewName(string newDisplayName)
    {
        Debug.Log(newDisplayName);
    }
    #endregion

}
