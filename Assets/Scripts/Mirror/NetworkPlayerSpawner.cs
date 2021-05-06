using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    // This script assign the correct character to each player when they enter the game.
    // Author: Valter Lindecrantz.

    [SerializeField] int chosenCharacter;
    
    // Called on the server from the network Manager to assign each player their chosen player prefab (character).
    [Server]
    public void AssignCharacterPrefab(int playerIndex)
    {
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;
        
        chosenCharacter = Convert.ToInt32(characterInfoList[playerIndex].GetValue(2));

        SpawnCharacter(chosenCharacter);
    }

    // Take in the parameter of what character the player has chosen, and spawns a prefab from the array of available characters to play as.
    void SpawnCharacter(int characterIndex)
    {
        GameObject[] characters = ((MyNetworkManager)NetworkManager.singleton).Characters;

        GameObject instantiatedCharacter = Instantiate(characters[characterIndex + 1], gameObject.transform.position, gameObject.transform.rotation);
        
        NetworkServer.Spawn(instantiatedCharacter, connectionToClient);
        ((MyNetworkManager)NetworkManager.singleton).Players.Add(instantiatedCharacter.GetComponent<MyNetworkPlayer>());

        instantiatedCharacter.GetComponent<MyNetworkPlayer>().AssignNameInGame(((MyNetworkManager)NetworkManager.singleton).Players.Count - 1);

        Destroy(gameObject);
    }
}
