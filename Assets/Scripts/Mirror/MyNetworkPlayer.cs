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
    [SerializeField] PlayerMovement movement;

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
    [SerializeField] Image ability1Border;
    [SerializeField] Image ability2Border;
    [SerializeField] Image ability3Border;
    [SerializeField] Image border;
    [SerializeField] Image TimerBorder;
    Vector3 borderSize;

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
    bool hasPlayedBackgroundMusic;
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
        borderSize = new Vector3(TimerBorder.transform.localScale.x * 0.5f, TimerBorder.transform.localScale.y, TimerBorder.transform.localScale.z);
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
        GamestateManager.HandleRedScoreChanged += CmdRespawn;
        GamestateManager.HandleBlueScoreChanged += CmdRespawn;
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
        GamestateManager.HandleRedScoreChanged -= CmdRespawn;
        GamestateManager.HandleBlueScoreChanged -= CmdRespawn;
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
        ability1Border.color = playerColor;
        ability2Border.color = playerColor;
        ability3Border.color = playerColor;
        border.color = playerColor;

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
    public void AssignNameInGame(CSteamID _steamID)
    {
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;

        foreach (string[] info in characterInfoList)
        {
            Debug.Log($"Comparing user steamID: {_steamID} to ID in the list: {info.GetValue(0)}");
            if (info.GetValue(0).ToString() == _steamID.ToString())
            {
                displayName = info.GetValue(1).ToString();
                teamName = info.GetValue(2).ToString();
                chosenCharacter = Convert.ToInt32(info.GetValue(3));

                Debug.Log($"Name: {displayName}, Team: {teamName}, Chosen Character: {chosenCharacter}");
            }
        }
    }
    
    [Command]
    void CmdRespawn()
    {
        Vector3 respawnPosition = gamestateManager.GetRespawnPositionObject(GetDisplayName()).transform.position;
        Quaternion respawnRotation = gamestateManager.GetRespawnPositionObject(GetDisplayName()).transform.rotation;
        movement.velocity = Vector3.zero;
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

        AudioSource backgroundMusic = GameObject.Find("Audio").transform.GetChild(0).GetComponent<AudioSource>();

        if (gamestateManager.PauseTimer <= 0)
        {
            TimerBorder.transform.localScale = borderSize;
            countdownCanvas.SetActive(false);

            if (!hasPlayedBackgroundMusic)
            {
                backgroundMusic.Play();
            }
            if (!trialSoundHasPlayed && playStartSound)
            {
                trialStartSound.Play();
                trialSoundHasPlayed = true;
            }

        }
    }
    #endregion

}
