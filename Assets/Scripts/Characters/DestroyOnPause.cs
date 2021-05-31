using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnPause : MonoBehaviour
{
    void Start()
    {
        GamestateManager gamestateManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
        GamestateManager.HandleMatchPaused += DestroyWhenGamePaused;
    }

    private void OnDestroy()
    {
        GamestateManager.HandleMatchPaused -= DestroyWhenGamePaused;
    }

    void DestroyWhenGamePaused()
    {
        Destroy(gameObject);
    }
}
