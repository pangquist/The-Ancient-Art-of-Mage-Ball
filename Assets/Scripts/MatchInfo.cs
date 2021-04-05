using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchInfo : NetworkBehaviour
{
    public int blueScore = 0;
    public int redScore = 0;
    public int time = 300;

    public void OnServerStart()
    {
    blueScore = 0;
    redScore = 0;
    time = 300;
    }
}
