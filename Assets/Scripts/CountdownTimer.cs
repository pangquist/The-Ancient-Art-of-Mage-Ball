using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class CountdownTimer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangeTime))]
    float currentTime = 0f;
    float startingTime = 300f;

    public float StartingTime { get { return startingTime; } set { startingTime = value; } }


    [SerializeField] TMP_Text countdownText;

    private void Start()
    {
        currentTime = startingTime;
    }

    private void Update()
    {
        //Problem beror p� att alla clienter har tillg�ng till varje klient timer. Kan s�tta timern p� klienten s� att bara de har authentication?

        if (currentTime <= 0) { return; }
        ShowTimer();       
    }

    private void ShowTimer()
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        if (currentTime <= 0) { return; }
        OnChangeTime(currentTime, currentTime -= 1 * Time.deltaTime);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnChangeTime(float oldTime, float newTime)
    {
        currentTime = newTime;
    }
}
