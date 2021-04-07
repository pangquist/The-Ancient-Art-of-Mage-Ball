using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class ScoreDisplay : NetworkBehaviour
{
    [SerializeField]
    public ScoreArea scoreArea;
    public TMP_Text scoreText;
    public bool isRed;
    public MatchInfo matchInfo;

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    private void Update()
    {
        //redScore = matchInfo.redScore;
        //blueScore = matchInfo.blueScore;
        //if(isRed)
        //scoreText.text = "Red: " + redScore.ToString();
        //else
        //    scoreText.text = "Blue: " + blueScore.ToString();
    }

}
