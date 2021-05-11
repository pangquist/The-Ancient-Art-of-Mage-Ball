using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderTrail : NetworkBehaviour
{   
    [SerializeField] GameObject stonePrefab;

    [SerializeField] Transform stoneSpawnPosition; //slightly ahead of the playercharacters feet.

    [SerializeField] float timeBetweenStones = 1f;

    [SerializeField] float scaleMin, scaleMax;

    [SerializeField] GameObject hitEffect;

    [Header("2 is default, minimum is the collider x  or z value of player")]
    [SerializeField] float dropPositionMarginalToPlayer = 2;

    PlayerMovement playerMovement;

    Vector3 dropPosition;

    float timer = 0;
    GameObject projectileInstance;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    [Client]
    private void Update()
    {        
        timer += Time.deltaTime;
        if (timer % 0.3 > 0 && timer % 0.3 < 0.1) // 0.1 sec window to set a new dropPosition. every 0.7 sec
        {
            dropPosition = stoneSpawnPosition.transform.position;
        }
        if (timer > timeBetweenStones)
        {
            if (playerMovement.isGrounded && playerMovement.velocity != Vector3.zero)
            {
                //Makes sure the stones can't spawn if the player is standing on the dropPosition, 
                if (gameObject.transform.position.x > dropPosition.x + dropPositionMarginalToPlayer ||
                    gameObject.transform.position.x < dropPosition.x - dropPositionMarginalToPlayer ||
                    gameObject.transform.position.z > dropPosition.z + dropPositionMarginalToPlayer ||
                    gameObject.transform.position.z < dropPosition.z - dropPositionMarginalToPlayer)
                {
                    CmdDropBoulder();//Spawns boulder
                }
            }
        }
    }

    [Command]
    void CmdDropBoulder()
    {        
            stonePrefab.transform.localScale = new Vector3(Random.Range(scaleMin, scaleMax), Random.Range(scaleMin, scaleMax), Random.Range(scaleMin, scaleMax));
            projectileInstance = Instantiate(stonePrefab, dropPosition, Quaternion.identity);

            CmdSpawnHitEffect();

            NetworkServer.Spawn(projectileInstance, connectionToClient);
            timer = 0;
    }
    [Command]
    void CmdSpawnHitEffect()
    {
        Vector3 offSet = new Vector3(1, 0, 0);
        GameObject smokeEffect = Instantiate(hitEffect, projectileInstance.transform.position + offSet, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(smokeEffect);
    }

}
