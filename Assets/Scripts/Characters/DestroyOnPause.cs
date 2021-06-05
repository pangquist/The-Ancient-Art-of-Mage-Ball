using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnPause : MonoBehaviour
{
    // This script, when applied to a gameObject, will destroy the object when the gamestateManager is "paused" (a bool on the gamestate manager) decides this.
    // Author: Valter Lindecrantz.

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
