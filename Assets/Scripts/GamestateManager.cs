using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GamestateManager : NetworkBehaviour
{
    public static bool gameIsOver = false;

    List<MyNetworkPlayer> blueTeam = new List<MyNetworkPlayer>();
    List<MyNetworkPlayer> redTeam = new List<MyNetworkPlayer>();

    [SerializeField] MyNetworkManager myNetworkManager;

    [SyncVar (hook = nameof(HandleTimeChange))]
    [SerializeField] float time;
    float startTime = 20f;

    int startScore = 0;
    [SyncVar (hook = nameof(HandleRedScore))]
    [SerializeField] int redScore;
    [SyncVar(hook = nameof(HandleBlueScore))]
    [SerializeField] int blueScore;

    public int BlueScore { get { return blueScore; } set { blueScore = value; } }
    public int RedScore { get { return redScore; } set { redScore = value; } }
    public float Timer { get { return time; } set { time = value; } }


    public static event Action HandleTimeChanged, HandleScoreChanged;

    public override void OnStartServer()
    {
        myNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        time = startTime;
        ResetScore();
    }

    private void Update()
    {
        //if (gameIsOver) return;

        if (time <= 0)
        {
            gameIsOver = true;
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
            myNetworkManager.EndGame();
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
}

 
