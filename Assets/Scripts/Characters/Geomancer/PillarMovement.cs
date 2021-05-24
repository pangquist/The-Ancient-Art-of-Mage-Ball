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
        //pillar X/Z Movement
        gameObject.transform.Translate(pillarPosition.transform.position - pillarTop.transform.position);


        if (gameObject.transform.position.y > 0)
        {
            goingUp = false;

        }

        //pillar Y movement
        if (goingUp)
        {           
            gameObject.transform.Translate(Vector3.up * forceUpwards * Time.deltaTime, Space.World);
            playerObject.transform.Translate(Vector3.up * (forceUpwards + 2) * Time.deltaTime, Space.World);
        }
        playerObject.GetComponent<PlayerMovement>().velocity.y = 0;


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
