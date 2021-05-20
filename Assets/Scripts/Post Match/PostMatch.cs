using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

 public class PostMatch : MonoBehaviour
{
    GamestateManager gamestateManager;
    
    [SerializeField] public TMP_Text winnerTeamText;
    [SerializeField] private TMP_Text losingTeamText;

    [SerializeField] TMP_Text winText;
    [SerializeField] TMP_Text loseText;

    [SerializeField] private TMP_Text winGoalText;
    [SerializeField] private TMP_Text loseGoalText;
    
    [SerializeField] List<TMP_Text> winPlayerTexts = new List<TMP_Text>();

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

            for(int i = 0; i < gamestateManager.RedTeam.Count; i++)
            {
                winPlayerTexts[0].text = gamestateManager.RedTeam[0];
            }

            for (int i = 0; i < gamestateManager.BlueTeam.Count; i++)
            {
                losePlayerTexts[0].text = gamestateManager.BlueTeam[0];
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

            for (int i = 0; i < gamestateManager.BlueTeam.Count; i++)
            {
                winPlayerTexts[0].text = gamestateManager.BlueTeam[0];
            }

            for (int i = 0; i < gamestateManager.RedTeam.Count; i++)
            {
                losePlayerTexts[0].text = gamestateManager.RedTeam[0];
            }
        }
    }

}
