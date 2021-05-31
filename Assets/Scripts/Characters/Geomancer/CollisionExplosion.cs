using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionExplosion : NetworkBehaviour
{
    [SerializeField] float rotateX, rotateY, rotateZ;

    [SerializeField] float explosionForceHorizontal, explotionForceVertical;

    [SerializeField] float explosionRadius;

    [SerializeField] GameObject hitEffect;

    bool hasCollided = false, hasPushed = false;

    GamestateManager gameState;

    Vector3 rotateAxis;


    float radius = 2;


    private void Start()
    {
        rotateAxis = new Vector3(rotateX, rotateY, rotateZ);// point of boulder-rotation in air.
        gameState = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
    }

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
                if (pushedObject.CompareTag("Enemy") && !hasPushed)
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
      

        if (!hasCollided) //The boulder will only be forced to rotate during the initial airtime.
        {
            transform.Rotate(rotateAxis, 3f);
        }

    }


    [Server]
    void ServerSpawnHitEffect(Vector3 hitLocation)
    {
        Debug.Log("Inside Hiteffect");
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);       
        //RpcSpawnHitEffect(hitLocation);
    }


    [Server]
    void ServerDoPush(GameObject ball)
    {
        Debug.Log("Server is moving the ball for the clients!");
        //ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForceHorizontal, gameObject.transform.position, explosionRadius, explotionForceVertical);
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForceHorizontal, gameObject.transform.position, explosionRadius);
        Destroy(gameObject);
    }


}
