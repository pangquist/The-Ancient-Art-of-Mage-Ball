using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarBridge : NetworkBehaviour
{
    [SerializeField] GameObject pillarBridgePrefab;

    [SerializeField] float abilityDuration;

    [SerializeField] float pillarSpawnInterval;

    [SerializeField] float forceUpwards;

    GameObject pillarSpawn;

    [Header("Multiply with player direction")]
    [SerializeField] float offSetPillarSpawnMultiplier = 2;

    Transform pillarTop;

    GameObject[] groundTransform;

    PlayerMovement playerMovement;

    Vector3 groundPositionUnderPlayer;

    Vector3 playerFuturePosition;

    float timer = 0;

    bool startAbility = false;
    bool hasSpawnedPillar;

    public override void OnStartAuthority()
    {
        enabled = true;
    }
    private void Start()
    {
        groundTransform = GameObject.FindGameObjectsWithTag("Ground");
        hasSpawnedPillar = false;
    }

    [Client]
    void DoPillarBridge()
    {        
        pillarTop = pillarBridgePrefab.transform.GetChild(0);
        timer = 0;
        startAbility = true;
        playerMovement = GetComponent<PlayerMovement>();
    }

    [Client]
    private void Update()
    {
        if (startAbility && !hasSpawnedPillar)
        {
            CmdSpawnPillarBridge();
        }
        if (startAbility)
        {
            timer += Time.deltaTime;

            if (timer > abilityDuration)//Handles the duration of the ability
            {
                startAbility = false;
                
            }
            else
            {
                CmdMovePillar(forceUpwards);
            }

        }

        
  
        //    if (!playerMovement.isGrounded)//Only spawns the pillars if the player is above ground.
        //    {
        //        if(timer%pillarSpawnInterval>0.05 && timer% pillarSpawnInterval < 0.1)
        //        {
        //            hasSpawnedPillar = false;
        //        }
        //    }
        //    if (!hasSpawnedPillar)
        //    {
        //        CmdSpawnPillarBridge();
        //    }
        //}                       
    }

    [Command]
    void CmdSpawnPillarBridge()//Spawns pillar at groundlevel under player
    {
        playerFuturePosition = gameObject.transform.forward * offSetPillarSpawnMultiplier;
        groundPositionUnderPlayer = new Vector3(gameObject.transform.position.x, groundTransform[0].transform.position.y, gameObject.transform.position.z) + playerFuturePosition;

        pillarSpawn = Instantiate(pillarBridgePrefab, groundPositionUnderPlayer - pillarTop.position, Quaternion.identity);

        NetworkServer.Spawn(pillarSpawn, connectionToClient);

        hasSpawnedPillar = true;
    }

    [Command]
    void CmdMovePillar(float forceUpwards)
    {
        //pillarSpawn.transform.Translate();
    }
}
