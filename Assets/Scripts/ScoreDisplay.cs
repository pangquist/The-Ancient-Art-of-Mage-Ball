using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField]
    public int redScore = 0;
    public int blueScore = 0;
    public ScoreArea scoreArea;
    public TMP_Text scoreText;
    public bool isRed;
    public MatchInfo matchInfo;

    private void Update()
    {
        redScore = matchInfo.redScore;
        blueScore = matchInfo.blueScore;
        if(isRed)
        scoreText.text = "Red: " + redScore.ToString();
        else
            scoreText.text = "Blue: " + blueScore.ToString();
    }

}
