using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchInfo : NetworkBehaviour
{
    public int blueScore = 0;
    public int redScore = 0;

    public int BlueScore { get { return blueScore; } set { blueScore = value; } }

    public int RedScore { get { return redScore; } set { redScore = value; } }

    public override void OnStartServer()
    {
        ResetScore();
    }

    public void ResetScore()
    {
        blueScore = 0;
        redScore = 0;
    }
}
