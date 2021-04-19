using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] GameObject landingPagePanel;
    [SerializeField] TMP_InputField addressInput;
    [SerializeField] Button joinButton;

    private void OnEnable()
    {
        MyNetworkManager.ClientOnConnected += HandleClientConnected;
        MyNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientConnected;
        MyNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }
    
    public void Join()
    {
        Debug.Log("Trying to join Lobby!");
        string address = addressInput.text;

        MyNetworkManager.singleton.networkAddress = address;
        MyNetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    void HandleClientConnected() 
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
