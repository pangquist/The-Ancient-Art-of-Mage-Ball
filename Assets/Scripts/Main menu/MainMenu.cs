using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // This script makes it possible for the host to establish a lobby using the Steam API.
    // Additionally it handles the clients request to connect to the lobby, and establishes a connection between the client and the server.
    // Author: Valter Lindecrantz (Reference: "How To Connect Using Steam - Unity Multiplayer Tutorial" - Dapper Dino on youtube)

    [SerializeField] GameObject landingPagePanel;
    [SerializeField] MyNetworkManager myNetworkManager;
    [SerializeField] bool useSteam = false;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    
    public static CSteamID LobbyId { get; private set; }

    private void Start()
    {
        if (!useSteam)
            return;

        myNetworkManager = GameObject.Find("MenuNetworkManager").GetComponent<MyNetworkManager>();
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    //Creates a lobby for the host if the ability to use steam is enabled on the Network Manager
    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, myNetworkManager.maxConnections);
            return;
        }

        NetworkManager.singleton.StartHost();
    }

    //When a lobby is successfully created, the UI elements that the lobby consists of are enabled.
    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            landingPagePanel.SetActive(true);
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(
            LobbyId,
            "HostAddress",
            SteamUser.GetSteamID().ToString());
    }

    // When a request to join the lobby is made, Steamworkshops makes a call for the client to join the lobby.
    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
            return;

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        landingPagePanel.SetActive(false);
    }
}
