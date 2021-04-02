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
        Debug.Log(currentTime);
    }
    
    private void Update()
    {
        if (currentTime <= 0) { return; }
        currentTime -= 1 * Time.deltaTime;
        //Debug.Log(currentTime);
        SendTime(CurrentTime);
    }

    [ClientCallback]
    void SendTime(float time)
    {
        //Debug.Log(currentTime);
        CmdSendTime(time);
    }

    [Command]
    void CmdSendTime(float time)
    {
        //Debug.Log("Command recieved!");
        RpcSendTime(time);
    }

    [ClientRpc]
    private void RpcSendTime(float time)
    {
        //Debug.Log("Sending Time to all clients!");
        GetComponent<MyNetworkPlayer>().RpcChangeTimer(time);
    }
}
