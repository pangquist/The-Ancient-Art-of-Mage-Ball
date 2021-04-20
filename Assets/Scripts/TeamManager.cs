using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<string> redTeam = new List<string>();
    public List<string> blueTeam = new List<string>();
    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
