using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PillarMovement : NetworkBehaviour
{
    [SerializeField] GameObject pillarSmoke;
    [SerializeField] Transform pillarTop;
    [SerializeField] float forceUpwards;
    GameObject playerObject;
    Transform pillarPosition;
    bool hasSpawnedEffect = false;
    float heightOfPillar;
    int i=0;
    bool goingUp = true;
    GameObject pillarSmokeEffect;
    Vector3 effectPosition;

    void Start()
    {
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        CalculatePillarHeight();
        foreach (MyNetworkPlayer player in players)
        {
            if (player.hasAuthority)
            {
                playerObject = player.gameObject;
            }
        }
        pillarPosition = playerObject.transform.Find("PillarSpawnPoint");
    }

    private void CalculatePillarHeight()
    {
        // pillar has one child in it's object-structure (pillarTop). NOTE: If this changes this might be obsolete.
        heightOfPillar = pillarTop.position.y - gameObject.transform.position.y;
    }

    [Client]
    private void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        effectPosition = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        Debug.Log(effectPosition);

        Vector3 pillarPos = pillarPosition.transform.position - pillarTop.transform.position;

        //pillar X/Z Movement
        gameObject.transform.Translate(pillarPos.x, 0, pillarPos.z);


        if (gameObject.transform.position.y >= 0)
        {
            goingUp = false;
        }

        //pillar Y movement
        if (goingUp)
        {           
            gameObject.transform.Translate(Vector3.up * forceUpwards * Time.deltaTime, Space.World);          
        }

        playerObject.GetComponent<PlayerMovement>().velocity.y = 0;

        if (!hasSpawnedEffect)
        {
            CmdSpawnPillarSmokeEffect();
            hasSpawnedEffect = true;
        }
        else
        {
            CmdMoveSmokeEffect();
        }

    }
    [Command]
    void CmdSpawnPillarSmokeEffect()
    {
        pillarSmokeEffect = Instantiate(pillarSmoke, effectPosition, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(pillarSmokeEffect, connectionToClient);
    }

    [Command]
    void CmdMoveSmokeEffect()
    {
        pillarSmokeEffect.transform.position = effectPosition;
    }

    //[Command]
    //void CmdMovePillar(Vector3 _pillarPosition, Vector3 _pillarTop)
    //{
    //    gameObject.transform.Translate(_pillarPosition - _pillarTop);
    //    gameObject.transform.Translate(Vector3.up * forceUpwards * Time.deltaTime, Space.World);
    //}

    //[Command]
    //void CmdMoveHitEffect(Vector3 effectPosition)
    //{
    //    pillarSmoke.transform.Translate(effectPosition, Space.World);
    //}

    [Client]
    void SpawnHitEffect(Vector3 effectPosition)
    {
        GameObject pillarSmokeEffect = Instantiate(pillarSmoke, effectPosition, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(pillarSmokeEffect);
    }
}
