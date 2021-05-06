using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderTrail : NetworkBehaviour
{
    PlayerMovement playerMovement;
    [SerializeField]
    GameObject stonePrefab;
    [SerializeField]
    Transform stoneSpawnPosition;
    [SerializeField]
    float timeBetweenStones = 1f;

    Vector3 dropPosition;

    float timer = 0;
    GameObject projectileInstance;
    private void Update()
    {
        timer += Time.deltaTime;

        if(timer % 0.7 >0 && timer % 0.7< 0.2)
        {
            dropPosition = gameObject.transform.position;
        }
        playerMovement = GetComponent<PlayerMovement>();
        if(timer> timeBetweenStones)
        {
            if (playerMovement.isGrounded && playerMovement.velocity != Vector3.zero)
            {
                if(gameObject.transform.position.x > dropPosition.x + 2 || 
                    gameObject.transform.position.x < dropPosition.x -2 ||
                    gameObject.transform.position.y > dropPosition.y + 2 || 
                    gameObject.transform.position.y < dropPosition.y - 2)
                {
                    projectileInstance = Instantiate(stonePrefab, dropPosition, Quaternion.identity);
                    timer = 0;
                }
               
            }
        }
        
    }

}
