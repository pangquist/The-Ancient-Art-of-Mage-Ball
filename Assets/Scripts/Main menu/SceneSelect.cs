using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSelect : MonoBehaviour
{
    // Handles the map selection process in the lobby menu.
    // When a player clicks a map-button the "selected map"-variable on the Network Manager is changed accordingly.
    // When a map is changed. The script invokes an event action that changes the background of the selected map picture in the lobby.
    // Author: Valter Lindecrantz

    MyNetworkManager networkManager;
    [SerializeField] ChangeMenuBackground changeBackground;
    [SerializeField] List<string> maps = new List<string>();
    
    public static event Action OnMenuBackgroundUpdated;

    public List<string> Maps()
    {
        return maps;
    }

    private void Start()
    {
        networkManager = GameObject.Find("MenuNetworkManager").GetComponent<MyNetworkManager>();
    }

    public void ChangeSelectedScene(int sceneIndex)
    {
        networkManager.SelectedScene = maps[sceneIndex];
        changeBackground.SetMapName(maps[sceneIndex]);
        OnMenuBackgroundUpdated?.Invoke();
    }

    public void RandomizeMap()
    {
        System.Random random = new System.Random();

        int sceneIndex = random.Next(0, maps.Count);

        networkManager.SelectedScene = maps[sceneIndex];
        changeBackground.SetMapName(maps[sceneIndex]);
        OnMenuBackgroundUpdated?.Invoke();
    }
}
