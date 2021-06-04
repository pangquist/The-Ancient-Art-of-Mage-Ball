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

    [Header("Script Dependencies")]
    [SerializeField] GamestateManager gamestateManager;

    [Header("´Player Information")]
    [SerializeField] int chosenCharacter;
    [SerializeField] string playerTeam;
    [SerializeField] string playerName;
    [SerializeField] CSteamID steamID;
    
    GameObject spawnPositionObject;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        steamID = SteamUser.GetSteamID();
        AssignCharacterPrefab(steamID);
    }

    public void SetGamestateManager(GamestateManager _gamestateManager)
    {
        gamestateManager = _gamestateManager;
    }

    [Command]
    public void AssignCharacterPrefab(CSteamID _steamID)
    {
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;

        foreach (string[] info in characterInfoList)
        {
            Debug.Log($"Comparing user steamID: {_steamID} to ID in the list: {info.GetValue(0)}");
            if(info.GetValue(0).ToString() == _steamID.ToString())
            {
                playerName = info.GetValue(1).ToString();
                playerTeam = info.GetValue(2).ToString();

                if (info.GetValue(3).ToString() == "5")
                {
                    System.Random random = new System.Random();
                    chosenCharacter = random.Next(0, 4);
                }
                else
                {
                    chosenCharacter = Convert.ToInt32(info.GetValue(3));
                }

                Debug.Log($"Name: {playerName}, Team: { playerTeam}, Chosen Character: {chosenCharacter}");
            }
        }

        spawnPositionObject = gamestateManager.GetRespawnPositionObject(playerName);
        SpawnCharacter(chosenCharacter, spawnPositionObject, _steamID);
    }

    // Take in the parameter of what character the player has chosen, and spawns a prefab from the array of available characters to play as.
    void SpawnCharacter(int characterIndex, GameObject _spawnPositionObject, CSteamID _steamID)
    {
        GameObject[] characters = ((MyNetworkManager)NetworkManager.singleton).Characters;

        GameObject instantiatedCharacter = Instantiate(characters[characterIndex + 1], _spawnPositionObject.transform.position, _spawnPositionObject.transform.rotation);
        
        NetworkServer.Spawn(instantiatedCharacter, connectionToClient);
        instantiatedCharacter.GetComponent<MyNetworkPlayer>().AssignNameInGame(_steamID);
        ((MyNetworkManager)NetworkManager.singleton).Players.Add(instantiatedCharacter.GetComponent<MyNetworkPlayer>());
        Destroy(gameObject);
    }
}
