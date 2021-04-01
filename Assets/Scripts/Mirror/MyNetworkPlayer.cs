using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";

    //[SyncVar(hook = nameof(HandleDisplayColourUpdated))]
    [SerializeField] Color playerColour = Color.white;

    [SerializeField] TMP_Text displayNameText = null;

    private int teamNumber;
    public int TeamNumber   // property
    {
        get { return teamNumber; }   // get method
        set { teamNumber = value; }  // set method
    }

    #region Server

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
    void CmdSetDisplayName(string newDisplayName)
    {

        if (newDisplayName.Length < 2 || newDisplayName.Length > 15)
            return;

        RpcLogNewName(newDisplayName);

        SetDisplayName(newDisplayName);
    }
    #endregion
    #region Client

    public override void OnStartAuthority()
    {
        gameObject.GetComponent<Animator>().enabled = true;
    }

    private void HandlePlayerNameUpdated(string oldName, string newName)
    {
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
