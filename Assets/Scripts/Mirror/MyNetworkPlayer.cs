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
    [SyncVar(hook = nameof(HandlePlayerTeamAssigned))]
    [SerializeField] string teamName;

    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";

    [SyncVar(hook = nameof(HandlePlayerColorUpdated))]
    [SerializeField] Color playerColor = Color.white;

    [SerializeField] int chosenCharacter;

    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject inGameUI;
    [SerializeField] TMP_Text displayNameText = null;
    [SerializeField] TMP_Text redScoreText;
    [SerializeField] TMP_Text blueScoreText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] GameObject nameCanvas;
    [SerializeField] GameObject settingsCanvas;


    [SerializeField] GamestateManager gamestateManager;

    public TMP_Text BlueScore { get { return blueScoreText; } set { blueScoreText = value; } }
    public TMP_Text RedScore { get { return redScoreText; } set { redScoreText = value; } }
    public TMP_Text TimeText { get { return timeText; } set { timeText = value; } }
    public string TeamName { get { return teamName; } set { teamName = value; } }

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

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    [ClientRpc]
    public void RpcSetTeamName(string newTeamName)
    {
        teamName = newTeamName;
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

    #region Client

    public override void OnStartAuthority()
    {
        settingsCanvas.SetActive(true);
        gameObject.GetComponent<AudioListener>().enabled = true;
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        controller = gameObject.GetComponent<CharacterController>();
        inGameUI.SetActive(true);
        nameCanvas.SetActive(false);
        gamestateManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
        GamestateManager.HandleTimeChanged += SetTimerText;
        GamestateManager.HandleScoreChanged += SetScoreText;
        GamestateManager.HandleScoreChanged += Respawn;
        GamestateManager.HandleMatchStarted += StartGameSpawn;

        base.OnStartAuthority();
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active)
            return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority)
            return;

        GamestateManager.HandleTimeChanged -= SetTimerText;
        GamestateManager.HandleScoreChanged -= SetScoreText;
        GamestateManager.HandleScoreChanged -= Respawn;
        GamestateManager.HandleMatchStarted -= StartGameSpawn;
        ((MyNetworkManager)NetworkManager.singleton).Players.Remove(this);
    }

    [Client]
    private void HandlePlayerNameUpdated(string oldName, string newName)
    {
        displayNameText.text = displayName;
    }

    public void SetTeamName(string name)
    {
        teamName = name;
        CmdSetTeamName(name);
    }

    [Client]
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
        Debug.Log("The color has been updated for the client!");

        displayNameText.color = playerColor;

        if (hasAuthority)
        {
            CmdSetTextColor();
        }
    }

    [Command]
    void CmdSetTextColor()
    {
        ServerSetTextColor();
    }

    [Server]
    void ServerSetTextColor()
    {
        displayNameText.color = playerColor;
    }
    
    [Client]
    public void AssignNameInGame(int playerIndex)
    {
        Debug.Log("ASSIGNING NAMES AND TEAMS");

        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;

        displayName = characterInfoList[playerIndex].GetValue(1).ToString();
        teamName = characterInfoList[playerIndex].GetValue(2).ToString();
        chosenCharacter = Convert.ToInt32(characterInfoList[playerIndex].GetValue(3));
    }

    [Client]
    public void Respawn()
    {
        if (!hasAuthority)
        {
            return;
        }

        Vector3 respawnPosition = gamestateManager.GetRespawnPosition(GetDisplayName());

        Debug.Log($"RESPAWNING! Respawn position: {respawnPosition}");
        gameObject.transform.position = respawnPosition;
        CmdRespawn(respawnPosition);

    }

    [Client]
    void StartGameSpawn()
    {
        Debug.Log("START GAME SPAWN");
        Respawn();
        playerMovement.enabled = true;
        controller.enabled = true;
    }

    [Command]
    void CmdRespawn(Vector3 respawnPosition)
    {
        gameObject.transform.position = respawnPosition;
    }
    #endregion

}
