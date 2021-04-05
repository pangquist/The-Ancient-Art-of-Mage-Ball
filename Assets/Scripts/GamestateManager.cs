using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamestateManager : NetworkBehaviour
{
    //[SyncVar]
    //[SerializeField]
    //public static bool gameIsOver = false;

    List<MyNetworkPlayer> blueTeam = new List<MyNetworkPlayer>();
    List<MyNetworkPlayer> redTeam = new List<MyNetworkPlayer>();

    [SyncVar]
    [SerializeField] double time;
    double startTime = 300f;

    int startScore = 0;
    [SyncVar (hook = nameof(HandleRedScore))]
    [SerializeField] int redScore;
    [SyncVar(hook = nameof(HandleBlueScore))]
    [SerializeField] int blueScore;

    public int BlueScore { get { return blueScore; } set { blueScore = value; } }
    public int RedScore { get { return redScore; } set { redScore = value; } }


    public override void OnStartServer()
    {
        ResetScore();
    }

    public void ResetScore()
    {
        blueScore = startScore;
        redScore = startScore;
    }

    public void HandleRedScore()
    {
        redScoreUi.text = "Red: " + redScore.ToString();
 
    }

    public void HandleBlueScore()
    {
        blueScoreUi.text = "Blue: " + blueScore.ToString();
    }
}

 
