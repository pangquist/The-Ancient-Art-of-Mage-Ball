using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PillarMovement : NetworkBehaviour
{
    [SerializeField] GameObject pillarSmoke;
    [SerializeField] Transform pillarTop;
    GameObject playerObject;
    Transform pillarPosition;
    bool hasSpawnedEffect = false;

    void Start()
    {
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        Debug.Log("SEARCHING FOR A POSITION. Player count: " + players.Count);

        foreach (MyNetworkPlayer player in players)
        {
            if (player.hasAuthority)
            {
                Debug.Log("FOUND A POSITION");
                playerObject = player.gameObject;
            }
        }
        Debug.Log("Playerposition is: " + playerObject.transform.position);
    }

    [Client]
    private void Update()
    {
        pillarPosition = playerObject.transform.Find("PillarSpawnPoint");
        Debug.Log("Player" + pillarPosition);
        gameObject.transform.position = pillarPosition.transform.position - pillarTop.transform.position;
        //gameObject.transform.position = Vector3.zero;
        Vector3 effectPosition = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        CmdMoveHitEffect(effectPosition);
        if (!hasSpawnedEffect)
        {
            CmdSpawnHitEffect(gameObject.transform.position);
            hasSpawnedEffect = true;
        }
        
    }

    [Command]
    void CmdMoveHitEffect(Vector3 effectPosition)
    {
        pillarSmoke.transform.Translate(effectPosition, Space.World);
    }

    [Command]
    void CmdSpawnHitEffect(Vector3 effectPosition)
    {
        GameObject pillarSmokeEffect = Instantiate(pillarSmoke, effectPosition, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(pillarSmokeEffect);
    }
}
