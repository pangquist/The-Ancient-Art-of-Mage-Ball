using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderTrail : NetworkBehaviour
{
    PlayerMovement playerMovement;
    [SerializeField] GameObject stonePrefab;

    [SerializeField] Transform stoneSpawnPosition;

    [SerializeField] float timeBetweenStones = 1f;


    [SerializeField] float scaleMin, scaleMax;


    [SerializeField] GameObject hitEffect;




    Vector3 dropPosition;

    float timer = 0;
    GameObject projectileInstance;
    private void Update()
    {
        timer += Time.deltaTime;
        
        if(timer % 0.7 >0 && timer % 0.7< 0.2)
        {
            dropPosition = stoneSpawnPosition.transform.position;
        }
        playerMovement = GetComponent<PlayerMovement>();
        if(timer> timeBetweenStones)
        {
            if (playerMovement.isGrounded && playerMovement.velocity != Vector3.zero)
            {
                if(gameObject.transform.position.x > dropPosition.x + 2 || 
                    gameObject.transform.position.x < dropPosition.x -2 ||
                    gameObject.transform.position.z > dropPosition.z + 2 || 
                    gameObject.transform.position.z < dropPosition.z - 2)
                {
                    Debug.Log("Inside BoulderTrail!");
                    stonePrefab.transform.localScale = new Vector3(Random.Range(scaleMin, scaleMax), Random.Range(scaleMin, scaleMax), Random.Range(scaleMin, scaleMax));
                    projectileInstance = Instantiate(stonePrefab, dropPosition, Quaternion.identity);

                        CmdSpawnHitEffect();

                    NetworkServer.Spawn(projectileInstance, connectionToClient);
                    timer = 0;
                }
               
            }
        }
        
    }
    [Command]
    void CmdSpawnHitEffect()
    {
        Vector3 offSet = new Vector3(1, 0, 0);
        GameObject smokeEffect = Instantiate(hitEffect, projectileInstance.transform.position + offSet, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(smokeEffect);
        Debug.Log($"Intantiating the hit effect:");
    }

}
