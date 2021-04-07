using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GamestateManager : NetworkBehaviour
{
    //[SyncVar]
    //[SerializeField]
    //public static bool gameIsOver = false;

    List<MyNetworkPlayer> blueTeam = new List<MyNetworkPlayer>();
    List<MyNetworkPlayer> redTeam = new List<MyNetworkPlayer>();

    [SerializeField] MyNetworkManager myNetworkManager;

    [SyncVar (hook = nameof(HandleTimeChange))]
    [SerializeField] float time;
    float startTime = 300f;

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
        if (time <= 0) { return; }
        time -= Time.deltaTime;
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
        //SendBlueScore();
    }

    public void HandleRedScore(int oldScore, int newScore)
    {
        Debug.Log("Red score has been changed!");
        HandleScoreChanged?.Invoke();
        //SendRedScore();
    }

    
    //void SendRedScore()
    //{
    //    foreach (MyNetworkPlayer player in myNetworkManager.Players)
    //    {
    //        player.RedScore.text = "Red: " + redScore.ToString();
    //    }
    //}

    //void SendBlueScore()
    //{
    //    foreach (MyNetworkPlayer player in myNetworkManager.Players)
    //    {
    //        player.BlueScore.text = "Blue: " + blueScore.ToString();
    //    }
    //}
}

 
