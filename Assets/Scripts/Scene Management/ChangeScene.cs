using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : NetworkBehaviour
{
    [SerializeField] MyNetworkManager myNetworkManager;

    private void Start()
    {
        myNetworkManager = GameObject.Find("MenuNetworkManager").GetComponent<MyNetworkManager>();
    }

    public void ChangeTheScene(string sceneName)
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
            NetworkServer.Shutdown();
            NetworkClient.Shutdown();
        }
        else
        {
            NetworkManager.singleton.StopClient();
            NetworkClient.Shutdown();
        }

        //SceneManager.LoadScene(sceneName);
    }
}
