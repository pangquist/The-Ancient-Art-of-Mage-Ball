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

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    //private void Start()
    //{
    //    Debug.Log("attempting to call a command!");
    //    AssignCharacterPrefab();
    //}


    // Called on the client from the network Manager to assign each player their chosen player prefab (character).
    //[Client]
    //public void AssignCharacterPrefab()
    //{
    //    Debug.Log("ASSIGN HAS BEEN TRIGGERED");
    //    List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;
    //    Debug.Log($"CHARACTER INFO LIST: {characterInfoList.Count}");
    //    CSteamID steamId = SteamUser.GetSteamID();

    //    foreach (string[] infoArray in characterInfoList)
    //    {
    //        Debug.Log($"COMPARING USER STEAM ID: {steamId} TO: {infoArray.GetValue(0).ToString()}");
    //        if (steamId.ToString() == infoArray.GetValue(0).ToString())
    //        {
    //            Debug.Log("Found steam ID in the list!");
    //            chosenCharacter = Convert.ToInt32(infoArray.GetValue(3));
    //            break;
    //        }
    //    }

    //    CmdSpawnCharacter(chosenCharacter);
    //}
    
    public void AssignCharacterPrefab()
    {
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;

        foreach (string[] infoArray in characterInfoList)
        {
            Debug.Log($"COMPARING USER NAME: {name} TO: {infoArray.GetValue(1).ToString()}");
            if (name == infoArray.GetValue(1).ToString())
            {
                chosenCharacter = Convert.ToInt32(infoArray.GetValue(3));
                break;
            }
        }

        CmdSpawnCharacter(chosenCharacter);
    }

    // Take in the parameter of what character the player has chosen, and spawns a prefab from the array of available characters to play as.
    [Command]
    void CmdSpawnCharacter(int characterIndex)
    {
        GameObject[] characters = ((MyNetworkManager)NetworkManager.singleton).Characters;

        GameObject instantiatedCharacter = Instantiate(characters[characterIndex + 1], gameObject.transform.position, gameObject.transform.rotation);
        
        NetworkServer.Spawn(instantiatedCharacter, connectionToClient);
        ((MyNetworkManager)NetworkManager.singleton).Players.Add(instantiatedCharacter.GetComponent<MyNetworkPlayer>());

        instantiatedCharacter.GetComponent<MyNetworkPlayer>().AssignNameInGame(((MyNetworkManager)NetworkManager.singleton).Players.Count - 1);

        Destroy(gameObject);
    }
}
