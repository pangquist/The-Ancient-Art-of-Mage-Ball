using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarBridge : NetworkBehaviour
{
    [SerializeField]
    GameObject pillarBridgePrefab;

    [SerializeField]
    GameObject [] groundTransform;

    CharacterController controller;

    Transform pillarTop;

    PlayerMovement playerMovement;

    [SerializeField]
    float abilityDuration;

    [SerializeField]
    float pillarSpawnInterval;

    float timer = 0;

    bool startAbility = false;
    public override void OnStartAuthority()
    {
        enabled = true;
    }
    private void Start()
    {
        groundTransform = GameObject.FindGameObjectsWithTag("Ground");
        controller = gameObject.GetComponent<CharacterController>();
    }

    [Client]
    void DoPillarBridge()
    {        
        pillarTop = pillarBridgePrefab.transform.GetChild(0);
        timer = 0;
        startAbility = true;
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        
        if (startAbility)
        {
            timer += Time.deltaTime;
            bool hasSpawnedPillar = true;

            if (timer > abilityDuration)//Handles the duration of the ability
            {
                startAbility = false;
            }

            if (!playerMovement.isGrounded)
            {
                if(timer%pillarSpawnInterval>0.05 && timer% pillarSpawnInterval < 0.1)
                {
                    hasSpawnedPillar = false;
                }

            }
            if (!hasSpawnedPillar)
            {
                float offSetMultiplier = 2;
                Vector3 playerFuturePosition = gameObject.transform.forward * offSetMultiplier;

                Debug.Log(playerFuturePosition);

                Vector3 groundPositionUnderPlayer = new Vector3(gameObject.transform.position.x, groundTransform[0].transform.position.y, gameObject.transform.position.z) + playerFuturePosition;

                GameObject pillarSpawn = Instantiate(pillarBridgePrefab, groundPositionUnderPlayer - pillarTop.position, Quaternion.identity);

                NetworkServer.Spawn(pillarSpawn, connectionToClient);
                hasSpawnedPillar = true;
            }
        }
        
        
        
    }
}
