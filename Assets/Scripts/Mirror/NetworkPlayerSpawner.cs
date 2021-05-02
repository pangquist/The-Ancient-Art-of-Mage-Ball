using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void AssignNameInGame(int playerIndex)
    {
        Debug.Log("ASSIGNING NAMES AND TEAMS");

        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;
        List<string[]> characterInfoList = ((MyNetworkManager)NetworkManager.singleton).CharacterInfoList;

        displayName = characterInfoList[playerIndex].GetValue(0).ToString();
        teamName = characterInfoList[playerIndex].GetValue(1).ToString();
        chosenCharacter = Convert.ToInt32(characterInfoList[playerIndex].GetValue(2));
    }
}
