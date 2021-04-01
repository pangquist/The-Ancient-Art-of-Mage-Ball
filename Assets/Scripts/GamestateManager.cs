using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamestateManager : NetworkManager
{
    [SerializeField] TMP_Text scoreUi;
    //[SyncVar]
    //[SerializeField]
    //public static bool gameIsOver = false;

    List<MyNetworkPlayer> team1 = new List<MyNetworkPlayer>();
    List<MyNetworkPlayer> team2 = new List<MyNetworkPlayer>();
    
    //[SyncVar]
    //[SerializeField] double time;
    double startTime = 300;

    int startScore = 0;
    //[SyncVar]
    //[SerializeField] int team1Score; 
    //[SyncVar]
    //[SerializeField] int team2Score;

    List<MyNetworkPlayer> players = new List<MyNetworkPlayer>();

    public GameObject gameOverUI;
    public GameObject gameWinUI;

    bool isGameInProgress = false;

    public override void OnServerSceneChanged(string sceneName)
    {
        //gameIsOver = false;
        //team1Score = startScore;
        //team2Score = startScore;
        //time = startTime;
    }

    public void StartGame()
    {
        if (players.Count < 2)
            return;

        isGameInProgress = true;

        foreach(MyNetworkPlayer player in players)
        {
            if(player.TeamNumber == 1)
            {
                team1.Add(player);
            }
            else
            {
                team2.Add(player);
            }
        }

        ServerChangeScene("Scene_Map_01");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress)
            return;

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

        players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    void Update()
    {


        //if (time <= 0)
        //    EndGame();
    }
    //void EndGame()
    //{
    //    if(team1Score > team2Score)
    //    {
    //        //team 1 vinner
    //        foreach(MyNetworkPlayer player in team1)
    //        {
                
    //        }
    //    }
    //    else if(team1Score < team2Score)
    //    {
    //        //team 2 vinner
    //        foreach (MyNetworkPlayer player in team2)
    //        {
                
    //        }
    //    }
    //    else
    //    {
    //        //overtime
    //        time += 60;
    //    }

    //    gameIsOver = true;

    //    gameOverUI.SetActive(true);
    //}
    //void WinGame()
    //{

    //}
}
