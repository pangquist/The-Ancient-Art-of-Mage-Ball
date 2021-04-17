using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : NetworkBehaviour
{
    public void ChangeTheScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        if (sceneName == "MainMenu")
        {
            NetworkManager.singleton.StopHost();
        }
    }
}
