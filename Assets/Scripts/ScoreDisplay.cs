using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField]
    public int score;
    public TMP_Text scoreText;
    public bool isRed;

    private void Update()
    {
        if(isRed)
        scoreText.text = "Red: " + score.ToString();
        else
            scoreText.text = "Blue: " + score.ToString();
    }

}
