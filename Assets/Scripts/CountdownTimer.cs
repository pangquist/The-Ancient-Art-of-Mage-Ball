using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
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
        if(currentTime <= 0) { return; }
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");
    }
}
