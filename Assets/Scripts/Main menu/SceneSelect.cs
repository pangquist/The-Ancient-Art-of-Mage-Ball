using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSelect : MonoBehaviour
{
    [SerializeField] MyNetworkManager networkManager;
    
    public void ChangeSelectedScene(string sceneName)
    {
        networkManager.SelectedScene = sceneName;
    }
}
