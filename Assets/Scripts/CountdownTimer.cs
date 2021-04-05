using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class CountdownTimer : NetworkBehaviour
{
    [SyncVar (hook = nameof(OnChangeTime))]
    float currentTime;
    float startingTime = 300f;
    [SerializeField] TMP_Text timerText = null;

    public float CurrentTime { get { return currentTime; } set { currentTime = value; } }

    public override void OnStartServer()
    {
        currentTime = startingTime;
    }

    private void OnEnable()
    {
        if (!hasAuthority)
            return;
        timerText.enabled = true;
    }

    private void Update()
    {
        if (currentTime <= 0) { return; }
        CmdUpdateTime();
    }
    
    [Command]
    void CmdUpdateTime()
    {
        //Debug.Log("Sending time: " + currentTime + " to server");
        SetTime();
    }

    [Server]
    public void SetTime()
    {
        currentTime -= 1 * Time.deltaTime;
    }

    void OnChangeTime(float oldTime, float newTime)
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
