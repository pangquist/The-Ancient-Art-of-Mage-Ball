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
    [Header("Script Dependencies")]
    [SerializeField] CharacterController controller;
    [SerializeField] GamestateManager gamestateManager;
    [SerializeField] UseAbilities useAbilities;

    [Header("Sync Variables")]
    [SyncVar(hook = nameof(HandlePlayerTeamAssigned))]
    [SerializeField] string teamName;

    [SyncVar(hook = nameof(HandlePlayerNameUpdated))]
    [SerializeField] string displayName = "Missing Name";

    [SyncVar(hook = nameof(HandlePlayerColorUpdated))]
    [SerializeField] Color playerColor = Color.white;

    [Header("In Game UI")]
    [SerializeField] GameObject inGameUI;
    [SerializeField] TMP_Text redScoreText;
    [SerializeField] TMP_Text blueScoreText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] Image teamIcon;

    [Header("Name Canvas")]
    [SerializeField] GameObject nameCanvas;
    [SerializeField] TMP_Text displayNameText = null;

    [Header("Settings Canvas")]
    [SerializeField] GameObject settingsCanvas;

    [Header("Countdown Canvas")]
    [SerializeField] GameObject countdownCanvas;
    [SerializeField] TMP_Text CountdownText;
    [SerializeField] TMP_Text scoringTeamText;

    [Tooltip("The audio play when the match starts (countdown is 0)")]
    [SerializeField] AudioSource trialStartSound;
    bool trialSoundHasPlayed;
    [SerializeField] bool playStartSound;

    int chosenCharacter;
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

    void SetRedScoreText()
    {
        redScoreText.text = "Red: " + gamestateManager.RedScore.ToString();
        scoringTeamText.text = "Red Team Scored!";
        scoringTeamText.color = Color.red;
    }

    void SetBlueScoreText()
    {
        blueScoreText.text = "Blue: " + gamestateManager.BlueScore.ToString();
        scoringTeamText.text = "Blue Team Scored!";
        scoringTeamText.color = Color.blue;
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
        base.OnStartAuthority();

        trialSoundHasPlayed = false;
        settingsCanvas.SetActive(true);
        gameObject.GetComponent<AudioListener>().enabled = true;
        controller = gameObject.GetComponent<CharacterController>();
        inGameUI.SetActive(true);
        nameCanvas.SetActive(false);
        gamestateManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
        GamestateManager.HandleTimeChanged += SetTimerText;
        GamestateManager.HandleRedScoreChanged += SetRedScoreText;
        GamestateManager.HandleBlueScoreChanged += SetBlueScoreText;
        GamestateManager.HandleRedScoreChanged += Respawn;
        GamestateManager.HandleBlueScoreChanged += Respawn;
        GamestateManager.HandlePauseTimeChanged += Countdown;
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
        GamestateManager.HandleRedScoreChanged -= SetRedScoreText;
        GamestateManager.HandleBlueScoreChanged -= SetBlueScoreText;
        GamestateManager.HandleRedScoreChanged -= Respawn;
        GamestateManager.HandleBlueScoreChanged -= Respawn;
        GamestateManager.HandlePauseTimeChanged -= Countdown;
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
        displayNameText.color = playerColor;
        teamIcon.color = playerColor;

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
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;

        displayName = characterInfoList[playerIndex].GetValue(1).ToString();
        teamName = characterInfoList[playerIndex].GetValue(2).ToString();
        chosenCharacter = Convert.ToInt32(characterInfoList[playerIndex].GetValue(3));
    }
    
    [Client]
    void Respawn()
    {
        Vector3 respawnPosition = gamestateManager.GetRespawnPositionObject(GetDisplayName()).transform.position;
        Quaternion respawnRotation = gamestateManager.GetRespawnPositionObject(GetDisplayName()).transform.rotation;
        Debug.Log($"RESPAWNING! Respawn position: {respawnPosition}");
        gameObject.transform.position = respawnPosition;
        gameObject.transform.rotation = respawnRotation;
        useAbilities.ResetAllCooldowns();
    }

    void Countdown()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (!countdownCanvas.activeSelf)
        {
            countdownCanvas.SetActive(true);
        }

        CountdownText.text = Convert.ToInt32(gamestateManager.PauseTimer).ToString();
        countdownCanvas.GetComponent<Animator>().Play("Pulse");

        if (gamestateManager.PauseTimer <= 0)
        {
            countdownCanvas.SetActive(false);
            if (!trialSoundHasPlayed && playStartSound)
            {
                trialStartSound.Play();
                trialSoundHasPlayed = true;
            }

        }
    }
    #endregion

}
