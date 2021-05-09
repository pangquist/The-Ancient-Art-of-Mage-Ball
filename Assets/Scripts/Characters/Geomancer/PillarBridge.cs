using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarBridge : NetworkBehaviour
{
    [SerializeField] GameObject pillarBridgePrefab;

    [SerializeField] float abilityDuration;

    [SerializeField] float pillarSpawnInterval;

    [Header("Multiply with player direction")]
    [SerializeField] float offSetPillarSpawnMultiplier = 2;

    Transform pillarTop;

    GameObject[] groundTransform;

    PlayerMovement playerMovement;    

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
        hasSpawnedPillar = true;
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
        
        if (startAbility)
        {
            timer += Time.deltaTime;            

            if (timer > abilityDuration)//Handles the duration of the ability
            {
                startAbility = false;
            }

            if (!playerMovement.isGrounded)//Only spawns the pillars if the player is above ground.
            {
                if(timer%pillarSpawnInterval>0.05 && timer% pillarSpawnInterval < 0.1)
                {
                    hasSpawnedPillar = false;
                }
            }
            if (!hasSpawnedPillar)
            {
                CmdSpawnPillarBridge();
            }
        }                       
    }

    [Command]
    void CmdSpawnPillarBridge()//Spawns pillar at groundlevel under player
    {
        Vector3 playerFuturePosition = gameObject.transform.forward * offSetPillarSpawnMultiplier;
        Vector3 groundPositionUnderPlayer = new Vector3(gameObject.transform.position.x, groundTransform[0].transform.position.y, gameObject.transform.position.z) + playerFuturePosition;

        GameObject pillarSpawn = Instantiate(pillarBridgePrefab, groundPositionUnderPlayer - pillarTop.position, Quaternion.identity);

        NetworkServer.Spawn(pillarSpawn, connectionToClient);

        hasSpawnedPillar = true;
    }
}
