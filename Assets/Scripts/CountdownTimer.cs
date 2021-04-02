using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class CountdownTimer : NetworkBehaviour
{
    float currentTime = 0f;
    float startingTime = 300f;
    TMP_Text timerText = null;

    public float CurrentTime { get { return currentTime; } set { currentTime = value; } }

    private void Start()
    {
        currentTime = startingTime;
    }
    
    private void Update()
    {
        if (currentTime <= 0) { return; }
        Debug.Log(currentTime);
        RpcShowTimer();
    }
    
    private void RpcShowTimer()
    {
        if (currentTime <= 0) { return; }
        currentTime -= 1 * Time.deltaTime;
        foreach (MyNetworkPlayer player in MyNetworkManager.players)
        {
            player.CmdChangeTimer(currentTime);
        }
    }
}
