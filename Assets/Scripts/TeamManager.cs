using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<MyNetworkPlayer> redTeam = new List<MyNetworkPlayer>();
    public List<MyNetworkPlayer> blueTeam = new List<MyNetworkPlayer>();
    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void AssignTeam()
    {
        for (int i = 0; i < redTeam.Count; i++)
        {
            redTeam[i].TeamName = "Red Team";
        }
        for (int i = 0; i < blueTeam.Count; i++)
        {
            blueTeam[i].TeamName = "Blue Team";
        }
    }
}
