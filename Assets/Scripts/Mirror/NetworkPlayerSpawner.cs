using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    // This script assign the correct character to each player when they enter the game.
    // Author: Valter Lindecrantz.

    [SerializeField] int chosenCharacter;
    [SerializeField] string playerTeam;
    [SerializeField] string playerName;
    [SerializeField] CSteamID steamID;
    [SerializeField] GamestateManager gamestateManager;
    [SerializeField] Vector3 spawnPosition;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AssignCharacterPrefab();
    }

    public void SetGamestateManager(GamestateManager _gamestateManager)
    {
        gamestateManager = _gamestateManager;
    }

    [Command]
    public void AssignCharacterPrefab()
    {
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;

        steamID = SteamUser.GetSteamID();

        foreach (string[] info in characterInfoList)
        {
            Debug.Log($"Comparing user steamID: {steamID} to ID in the list: {info.GetValue(0)}");
            if(info.GetValue(0).ToString() == steamID.ToString())
            {
                playerName = info.GetValue(1).ToString();
                playerTeam = info.GetValue(2).ToString();
                chosenCharacter = Convert.ToInt32(info.GetValue(3));

                Debug.Log($"Name: {playerName}, Team: { playerTeam}, Chosen Character: {chosenCharacter}");
            }
        }

        spawnPosition = gamestateManager.GetRespawnPosition(playerName);
        SpawnCharacter(chosenCharacter, spawnPosition);
    }

    // Take in the parameter of what character the player has chosen, and spawns a prefab from the array of available characters to play as.
   
    void SpawnCharacter(int characterIndex, Vector3 spawnPosition)
    {
        GameObject[] characters = ((MyNetworkManager)NetworkManager.singleton).Characters;

        GameObject instantiatedCharacter = Instantiate(characters[characterIndex + 1], spawnPosition, gameObject.transform.rotation);
        
        NetworkServer.Spawn(instantiatedCharacter, connectionToClient);
        ((MyNetworkManager)NetworkManager.singleton).Players.Add(instantiatedCharacter.GetComponent<MyNetworkPlayer>());

        instantiatedCharacter.GetComponent<MyNetworkPlayer>().AssignNameInGame(((MyNetworkManager)NetworkManager.singleton).Players.Count - 1);

        Destroy(gameObject);
    }
}
