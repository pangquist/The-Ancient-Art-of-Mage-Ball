using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSelect : MonoBehaviour
{
    MyNetworkManager networkManager;
    [SerializeField] ChangeMenuBackground changeBackground;

    public static event Action OnMenuBackgroundUpdated;

    private void Start()
    {
        networkManager = GameObject.Find("MenuNetworkManager").GetComponent<MyNetworkManager>();
    }

    public void ChangeSelectedScene(string sceneName)
    {
        networkManager.SelectedScene = sceneName;
        changeBackground.SetMapName(sceneName);
        OnMenuBackgroundUpdated?.Invoke();
    }
}
