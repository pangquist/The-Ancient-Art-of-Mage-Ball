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
        myNetworkManager.ServerChangeScene(sceneName);
        //if (NetworkServer.active && NetworkClient.isConnected)
        //{
        //    NetworkManager.singleton.StopHost();
        //}
        //else
        //{
        //    NetworkManager.singleton.StopClient();
        //}
        //SceneManager.LoadScene(sceneName);
    }

    public void GoToMainMenu()
    {
        Destroy(myNetworkManager.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
