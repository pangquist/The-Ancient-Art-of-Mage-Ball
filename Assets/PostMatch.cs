using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

 public class PostMatch : MonoBehaviour
{

   [SerializeField] GamestateManager gameStateManager;
    void Awake()
    {
        gameStateManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
    }
    
    private string winnerTeam;
    private string losingTeam;
    private int winGoal;
    private int loseGoal;
    public string winPlayer1, winPlayer2, winPlayer3;
    public string losePlayer1, losePlayer2, losePlayer3;
    [SerializeField] public TMP_Text winnerTeamText;
    [SerializeField] private TMP_Text losingTeamText;

    [SerializeField] private TMP_Text winGoalText;
    [SerializeField] private TMP_Text loseGoalText;

    [SerializeField] private TMP_Text winPlayer1Text;
    [SerializeField] private TMP_Text winPlayer2Text;
    [SerializeField] private TMP_Text winPlayer3Text;

    [SerializeField] private TMP_Text losePlayer1Text;
    [SerializeField] private TMP_Text losePlayer2Text;
    [SerializeField] private TMP_Text losePlayer3Text;

    public string WinnerTeam { get { return winnerTeam; } set { winnerTeam = value; } }
    public string LosingTeam { get { return losingTeam; } set { losingTeam = value; } }
    public int WinGoal { get { return winGoal; } set { winGoal = value; } }
    public int LoseGoal { get { return loseGoal; } set { loseGoal = value; } }

   

    public void Update()
    {
        if (gameStateManager.BlueScore > gameStateManager.RedScore)
        {
            WinnerTeam = "Blue Team Wins";
            LosingTeam = "Red Team Wins";
            WinGoal = gameStateManager.BlueScore;
            LoseGoal = gameStateManager.RedScore;
        }
        else
        {
            WinnerTeam = "Red Team Wins";
            LosingTeam = "Blue Team Wins";
            WinGoal = gameStateManager.RedScore;
            LoseGoal = gameStateManager.BlueScore;
        }
        
        winnerTeamText.text = winnerTeam;
        losingTeamText.text = losingTeam;
        winGoalText.text = winGoal.ToString();
        loseGoalText.text = loseGoal.ToString();


    }



}
