using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GamestateManager : NetworkBehaviour
{
    public bool gameIsOver = false;

    [SerializeField] List<string> redTeam = new List<string>();
    [SerializeField] List<string> blueTeam = new List<string>();
    [SerializeField] List<Transform> spawnpointPositions = new List<Transform>();
    //[SerializeField] Transform[] spawnpointPositions = new Transform[6];

    [SerializeField] MyNetworkManager myNetworkManager;

    [SyncVar (hook = nameof(HandleTimeChange))]
    [SerializeField] float time;
    [SerializeField] float startTime = 180f;

    int startScore = 0;
    [SyncVar (hook = nameof(HandleRedScore))]
    [SerializeField] int redScore;
    [SyncVar(hook = nameof(HandleBlueScore))]
    [SerializeField] int blueScore;

    public int BlueScore { get { return blueScore; } set { blueScore = value; } }
    public int RedScore { get { return redScore; } set { redScore = value; } }
    public float Timer { get { return time; } set { time = value; } }

    [SerializeField] TMP_Text postGameRedScoreDisplay;
    [SerializeField] TMP_Text postGameBlueScoreDisplay;
    [SerializeField] TMP_Text winningTeamText;

    public static event Action HandleTimeChanged, HandleScoreChanged;

    public override void OnStartServer()
    {
        time = startTime;
        ResetScore();
    }

    private void Start()
    {
        myNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        DontDestroyOnLoad(this.gameObject);
        //ScoreArea.ClientOnGoal += OnGoal;
    }

    private void Update()
    {
        if (gameIsOver) return;

        if (time <= 0)
        {
            EndGame();
        }
            
        time -= Time.deltaTime;
    }

    void EndGame()
    {
        if(blueScore == redScore)
        {
            time = 60f;
        }
        else
        {
            gameIsOver = true;
            myNetworkManager.EndGame();
        }
    }

    public void AssignScoreAtPostScreen()
    {
        Debug.Log("Trying to find texts!");

        postGameRedScoreDisplay = 
            GameObject.Find("Canvas").
            gameObject.transform.Find("BackgroundRedTeam").
            gameObject.transform.Find("RedTeamScoreText").gameObject.GetComponent<TMP_Text>();

        postGameBlueScoreDisplay =
            GameObject.Find("Canvas").
            gameObject.transform.Find("BackgroundBlueTeam").
            gameObject.transform.Find("BlueTeamScoreText").gameObject.GetComponent<TMP_Text>();

        winningTeamText = GameObject.Find("Canvas").gameObject.transform.Find("WinningTeamText").gameObject.GetComponent<TMP_Text>();

        Debug.Log("Red: " + redScore + " and Blue: " + blueScore);

        postGameRedScoreDisplay.text = "Red Team score: " + redScore;
        postGameBlueScoreDisplay.text = "Blue Team score: " + blueScore;

        if (redScore > blueScore)
            winningTeamText.text = "Red Team Wins!";
        else
            winningTeamText.text = "Blue Team Wins!";
    }

    public void ResetScore()
    {
        blueScore = startScore;
        redScore = startScore;
    }
    
    public void HandleTimeChange(float oldTime, float newTime)
    {
        HandleTimeChanged?.Invoke();
    }

    public void HandleBlueScore(int oldScore, int newScore)
    {
        Debug.Log("Blue score has been changed!");
        HandleScoreChanged?.Invoke();
    }

    public void HandleRedScore(int oldScore, int newScore)
    {
        Debug.Log("Red score has been changed!");
        HandleScoreChanged?.Invoke();
    }
    
    public void StartGame()
    {
        Debug.Log("START GAME IS CALLED");

        for (int i = 0; i < 6; i++)
        {
            spawnpointPositions.Add(GameObject.Find("Spawnpoints").transform.GetChild(i));
        }
    }

    public void ClearPlayerList()
    {
        redTeam.Clear();
        blueTeam.Clear();
    }

    public void AddPlayerToTeam(MyNetworkMenuPlayer player, string team)
    {
        if (team == "Red")
        {
            redTeam.Add(player.GetDisplayName());
        }
        else
        {
            blueTeam.Add(player.GetDisplayName());
        }
    }

    public Vector3 GetRespawnPosition(string name)
    {
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        Debug.Log($"NUMBER OF PLAYERS IN LIST: {players.Count}, NAME GIVEN: {name}");

        foreach (MyNetworkPlayer player in players)
        {
            if (player.GetDisplayName() == name)
            {
                if (player.TeamName == "Red Team")
                {
                    for (int i = 0; i < redTeam.Count; i++)
                    {
                        if (player.GetDisplayName() == redTeam[i])
                        {
                            return(spawnpointPositions[i].transform.position);
                        }
                    }
                }
                else if (player.TeamName == "Blue Team")
                {
                    for (int i = 0; i < blueTeam.Count; i++)
                    {
                        if (player.GetDisplayName() == blueTeam[i])
                        {
                            return(spawnpointPositions[i + 3].transform.position);
                        }
                    }
                }
            }
        }

        return new Vector3(0, 0, 0);
    }

}

 
