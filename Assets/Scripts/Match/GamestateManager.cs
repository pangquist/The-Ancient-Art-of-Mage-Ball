using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GamestateManager : NetworkBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] MyNetworkManager myNetworkManager;
    [SerializeField] PostMatch postMatch;


    [Header("Lists")]
    [SerializeField] List<string> redTeam = new List<string>();
    [SerializeField] List<string> blueTeam = new List<string>();
    [SerializeField] List<Transform> spawnpointPositions = new List<Transform>();
    
    [Header("Match Timers")]
    [SyncVar (hook = nameof(HandleTimeChange))]
    [SerializeField] float time;
    [Tooltip("The duration of each game in seconds")]
    [SerializeField] float matchStartTime = 180f;

    [Header("Pause Timers")]
    [SyncVar(hook = nameof(HandlePausTimeChange))]
    [SerializeField] float pauseTimer;
    [Tooltip("The amount of seconds the game will wait before starting when first entering the match")]
    [SerializeField]  public float startPauseTime = 12f;
    [Tooltip("The amount of seconds the game will wait before starting when a goal is made")]
    [SerializeField] float goalPauseTime = 5f;

    [Header("Team Scores")]
    [SyncVar (hook = nameof(HandleRedScore))]
    [SerializeField] int redScore;
    [SyncVar(hook = nameof(HandleBlueScore))]
    [SerializeField] int blueScore;

    [Header("Settings")]
    [SerializeField] TMP_Text postGameRedScoreDisplay;
    [SerializeField] TMP_Text postGameBlueScoreDisplay;
    [SerializeField] TMP_Text winningTeamText;
    [SerializeField] AudioSource overtimeSoundSource;
    
    [SyncVar (hook = nameof(HandleMatchPause))]
    public bool matchIsPaused = true;

    [HideInInspector]
    public bool matchIsOver = false;
    int startScore = 0;

    public int BlueScore { get { return blueScore; } set { blueScore = value; } }
    public int RedScore { get { return redScore; } set { redScore = value; } }
    public float Timer { get { return time; } set { time = value; } }
    public float PauseTimer { get { return pauseTimer; } set { PauseTimer = value; } }
    public List<string> RedTeam { get { return redTeam; } set { redTeam = value; } }
    public List<string> BlueTeam { get { return blueTeam; } set { blueTeam = value; } }
    
    public static event Action HandleTimeChanged, HandleRedScoreChanged, HandleBlueScoreChanged, HandleMatchPaused, HandleMatchUnpaused, HandlePausTimeChanged;

    public override void OnStartServer()
    {
        time = matchStartTime;
        pauseTimer = startPauseTime;
        ResetScore();
    }

    private void Start()
    {
        myNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "PostMatch")
        {
            return;
        }

        if (matchIsPaused)
        {
            pauseTimer -= Time.deltaTime;

            if (pauseTimer <= 0)
            {
                UnpauseMatch();
            }
            return;
        }

        if (matchIsOver) return;

        if (time <= 0)
        {
            EndGame();
        }
            
        time -= Time.deltaTime;
    }

    [Server]
    void UnpauseMatch()
    {
        matchIsPaused = false;
    }

    void EndGame()
    {
        if(blueScore == redScore)
        {
            overtimeSoundSource.Play();
            time = 60f;
        }
        else
        {
            matchIsOver = true;
            myNetworkManager.EndGame();
        }
    }

    [Client]
    public void AssignScoreAtPostScreen()
    {
        postMatch = GameObject.Find("Main Camera").transform.Find("ScoreBoard").GetComponent<PostMatch>();
        postMatch.SetGamestateManager(this);
        if (redScore > blueScore)
        {
            postMatch.SetTexts("Red");
        }
        else
        {
            postMatch.SetTexts("Blue");
        }
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

    public void HandlePausTimeChange(float oldTime, float newTime)
    {
        HandlePausTimeChanged?.Invoke();
    }
    
    [Server]
    public void HandleBlueScore(int oldScore, int newScore)
    {
        HandleBlueScoreChanged?.Invoke();
        matchIsPaused = true;
        pauseTimer = goalPauseTime;
    }

    [Server]
    public void HandleRedScore(int oldScore, int newScore)
    {
        HandleRedScoreChanged?.Invoke();
        matchIsPaused = true;
        pauseTimer = goalPauseTime;
    }
    
    public void HandleMatchPause(bool oldBool, bool newBool)
    {
        Debug.Log("PAUSE EVENT IS CALLED");
        if (matchIsPaused)
        {
            HandleMatchPaused?.Invoke();
        }
        else
        {
            HandleMatchUnpaused?.Invoke();
        }
    }
    
    public void FillSpawnpointList()
    {
        if (SceneManager.GetActiveScene().name == "PostMatch")
        {
            return;
        }

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

        //Debug.Log($"NUMBER OF PLAYERS IN LIST: {players.Count}, NAME GIVEN: {name}");
        for (int i = 0; i < redTeam.Count; i++)
        {
            Debug.Log($"Red team member {i}: {redTeam[i]}");
        }

        for (int i = 0; i < blueTeam.Count; i++)
        {
            Debug.Log($"Blue team member {i}: {blueTeam[i]}");
        }

        for (int i = 0; i < redTeam.Count; i++)
        {
            Debug.Log($"Comparing user name: {name} to Name in the list: {redTeam[i]}");
            if (name == redTeam[i])
            {
                Debug.Log($"Returning red position: {spawnpointPositions[i].transform.position}");
                return (spawnpointPositions[i].transform.position);
            }
        }

        for (int i = 0; i < blueTeam.Count; i++)
        {
            Debug.Log($"Comparing user name: {name} to Name in the list: {blueTeam[i]}");
            if (name == blueTeam[i])
            {
                Debug.Log($"Returning blue position: {spawnpointPositions[i+3].transform.position}");
                return (spawnpointPositions[i + 3].transform.position);
            }
        }

        return new Vector3(0, 0, 0);
    }

}

 
