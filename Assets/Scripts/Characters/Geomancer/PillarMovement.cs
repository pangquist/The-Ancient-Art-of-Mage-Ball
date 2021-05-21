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


    

    void Start()
    {
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        //Debug.Log("SEARCHING FOR A POSITION. Player count: " + players.Count);

        foreach (MyNetworkPlayer player in players)
        {
            if (player.hasAuthority)
            {

                //Debug.Log("FOUND A POSITION");
                playerObject = player.gameObject;
            }
        }
        //Debug.Log("Playerposition is: " + playerObject.transform.position);
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
        //pillar X/Z Movement
        gameObject.transform.Translate(pillarPosition.transform.position - pillarTop.transform.position);
        

        //pillar Y movement
        if (gameObject.transform.position.y< heightOfPillar)
        {           
            gameObject.transform.Translate(Vector3.up * forceUpwards * Time.deltaTime, Space.World);
        }       

        Vector3 effectPosition = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        CmdMoveHitEffect(effectPosition);
        if (!hasSpawnedEffect)
        {
            CmdSpawnHitEffect(gameObject.transform.position);
            hasSpawnedEffect = true;
        }
        
    }

    //[Command]
    //void CmdMovePillar(Vector3 _pillarPosition, Vector3 _pillarTop)
    //{
    //    gameObject.transform.Translate(_pillarPosition - _pillarTop);
    //    gameObject.transform.Translate(Vector3.up * forceUpwards * Time.deltaTime, Space.World);
    //}

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
