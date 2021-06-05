using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : NetworkBehaviour
{
    // This script calls the "ServerChangeScene" method on the referenced Network Manager.
    // Author: Valter Lindecrantz

    [SerializeField] MyNetworkManager myNetworkManager;

    private void Start()
    {
        myNetworkManager = GameObject.Find("MenuNetworkManager").GetComponent<MyNetworkManager>();
    }

    public void ChangeTheScene(string sceneName)
    {
        myNetworkManager.ServerChangeScene(sceneName);
    }
}
