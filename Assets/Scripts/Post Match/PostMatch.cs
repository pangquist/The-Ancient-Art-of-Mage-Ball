using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PostMatch : MonoBehaviour
{
    // This script handles all functions that relates to the post game screen.
    // It assignes the players name to either the winning or the losing side, based on whether they were victorious or not.
    // Author: Kasper Veileborg & Valter Lindecrantz

    GamestateManager gamestateManager;

    [Header("Team texts")]
    [SerializeField] TMP_Text winnerTeamText;
    [SerializeField] TMP_Text losingTeamText;

    [Header("Victory status texts")]
    [SerializeField] TMP_Text winText;
    [SerializeField] TMP_Text loseText;

    [Header("Score count texts")]
    [SerializeField] TMP_Text winGoalText;
    [SerializeField] TMP_Text loseGoalText;

    [Header("Winning player lists")]
    [SerializeField] List<Image> winningBackgrounds = new List<Image>(); 
    [SerializeField] List<TMP_Text> winPlayerTexts = new List<TMP_Text>();

    [Header("Losing player lists")]
    [SerializeField] List<Image> losingBackgrounds = new List<Image>();
    [SerializeField] List<TMP_Text> losePlayerTexts = new List<TMP_Text>();
    
    public void SetGamestateManager(GamestateManager _gamestateManager)
    {
        gamestateManager = _gamestateManager;
    }

    public void SetTexts(string team)
    {
        if (team == "Red")
        {
            winnerTeamText.text = "Red Team";
            losingTeamText.text = "Blue Team";

            winText.color = Color.red;
            loseText.color = Color.blue;

            winGoalText.text = gamestateManager.RedScore.ToString();
            loseGoalText.text = gamestateManager.BlueScore.ToString();

            for(int i = 0; i < winningBackgrounds.Count; i++)
            {
                winningBackgrounds[i].color = new Color32(190, 85, 85, 255);
            }

            for (int i = 0; i < losingBackgrounds.Count; i++)
            {
                losingBackgrounds[i].color = new Color32(77, 86, 197, 255);
            }

            for (int i = 0; i < gamestateManager.RedTeam.Count; i++)
            {
                winPlayerTexts[i].text = gamestateManager.RedTeam[i];
            }

            for (int i = 0; i < gamestateManager.BlueTeam.Count; i++)
            {
                losePlayerTexts[i].text = gamestateManager.BlueTeam[i];
            }
        }
        else
        {
            winnerTeamText.text = "Blue Team";
            losingTeamText.text = "Red Team";

            winText.color = Color.blue;
            loseText.color = Color.red;

            winGoalText.text = gamestateManager.BlueScore.ToString();
            loseGoalText.text = gamestateManager.RedScore.ToString();

            for (int i = 0; i < winningBackgrounds.Count; i++)
            {
                winningBackgrounds[i].color = new Color32(77, 86, 197, 255);
            }

            for (int i = 0; i < losingBackgrounds.Count; i++)
            {
                losingBackgrounds[i].color = new Color32(190, 85, 85, 255);
            }

            for (int i = 0; i < gamestateManager.BlueTeam.Count; i++)
            {
                winPlayerTexts[i].text = gamestateManager.BlueTeam[i];
            }

            for (int i = 0; i < gamestateManager.RedTeam.Count; i++)
            {
                losePlayerTexts[i].text = gamestateManager.RedTeam[i];
            }
        }
    }

}
