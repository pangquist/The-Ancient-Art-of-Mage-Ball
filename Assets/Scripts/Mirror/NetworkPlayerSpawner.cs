using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField] int chosenCharacter;
    
    [Server]
    public void AssignCharacterPrefab(int playerIndex)
    {
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;
        
        chosenCharacter = Convert.ToInt32(characterInfoList[playerIndex].GetValue(2));

        SpawnCharacter(chosenCharacter);
    }

    void SpawnCharacter(int characterIndex)
    {
        GameObject[] characters = ((MyNetworkManager)NetworkManager.singleton).Characters;

        GameObject instantiatedCharacter = Instantiate(characters[characterIndex + 1], gameObject.transform.position, gameObject.transform.rotation);
        
        NetworkServer.Spawn(instantiatedCharacter, connectionToClient);
        ((MyNetworkManager)NetworkManager.singleton).Players.Add(instantiatedCharacter.GetComponent<MyNetworkPlayer>());

        instantiatedCharacter.GetComponent<MyNetworkPlayer>().AssignNameInGame(((MyNetworkManager)NetworkManager.singleton).Players.Count - 1);
    }
}
