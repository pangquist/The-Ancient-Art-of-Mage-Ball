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
    [SerializeField] string team;
    [SerializeField] string name;
    [SerializeField] GamestateManager gamestateManager;
    [SerializeField] Vector3 spawnPosition;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    public void SetGamestateManager(GamestateManager _gamestateManager)
    {
        gamestateManager = _gamestateManager;
    }

    [Server]
    public void AssignCharacterPrefab(int playerIndex)
    {
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;
        string[] characterInfo = characterInfoList[playerIndex];

        name = characterInfo.GetValue(1).ToString();
        team = characterInfo.GetValue(2).ToString();
        chosenCharacter = Convert.ToInt32(characterInfo.GetValue(3));

        spawnPosition = gamestateManager.GetRespawnPosition(name);
        Debug.Log($"Respawn Position: {spawnPosition}");
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
