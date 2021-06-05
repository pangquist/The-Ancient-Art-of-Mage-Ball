using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionExplosion : NetworkBehaviour
{
    [Header("Dependency")]
    [SerializeField] GameObject hitEffect;

    [Header("Values")]
    [SerializeField] float rotateX, rotateY, rotateZ;
    [SerializeField] float explosionForceHorizontal, explotionForceVertical;
    [SerializeField] float explosionRadius;

    bool hasCollided = false, hasPushed = false;

    GamestateManager gameState;

    Vector3 rotateAxis;

    float radius = 2;

    private void Start()
    {
        rotateAxis = new Vector3(rotateX, rotateY, rotateZ);// point of boulder-rotation in air.
        gameState = GameObject.Find("GamestateManager").GetComponent<GamestateManager>(); //allowing to get matchIsPaused for destroying object when goal is made.
    }
    //This toggles off the initial rotate through the bool
    private void OnCollisionEnter(Collision collision)
    {
        hasCollided = true;
    }

    [Server]
    private void Update()
    {        
        if (gameState.matchIsPaused)
        {
            Destroy(gameObject);
        }
        
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius);
        try {
            foreach (Collider pushedObject in colliders)
            {
                if (pushedObject.CompareTag("Enemy") && !hasPushed) // if ball is hit
                {
                    ServerDoPush(pushedObject.gameObject);
                    ServerSpawnHitEffect(gameObject.transform.position);
                    hasPushed = true;
                }                
            }
        }
        catch
        {
        }     
        //rotate as long as the object has not collided yet
        if (!hasCollided) //The boulder will only be forced to rotate during the initial airtime.
        {
            transform.Rotate(rotateAxis, 3f);
        }

    }

    [Server]
    void ServerSpawnHitEffect(Vector3 hitLocation)
    {
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);       
    }


    [Server]
    void ServerDoPush(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForceHorizontal, gameObject.transform.position, explosionRadius, explotionForceVertical);
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForceHorizontal, gameObject.transform.position, explosionRadius);
        Destroy(gameObject);
    }


}
