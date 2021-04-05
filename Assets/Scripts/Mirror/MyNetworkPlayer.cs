using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";
    [SerializeField] TMP_Text timerText;
    [SerializeField] Color playerColour = Color.white;

    [SerializeField] TMP_Text displayNameText = null;

    [SerializeField] TMP_Text redScoreText;
    [SerializeField] TMP_Text blueScoreText;

    private int teamNumber;
    public int TeamNumber   // property
    {
        get { return teamNumber; }   // get method
        set { teamNumber = value; }  // set method
    }

    public static float time;

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

    [Command]
    public void CmdChangeTimer(float time)
    {
        RpcChangeTimer(time);
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

    [ClientRpc]
    public void RpcChangeTimer(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
