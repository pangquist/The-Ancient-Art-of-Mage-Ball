using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionExplosion : NetworkBehaviour
{
    [SerializeField] float rotateX, rotateY, rotateZ;

    [SerializeField] float explosionForceForward, explosionForceUpwards;

    [SerializeField] float explosionRadius;

    [SerializeField] GameObject hitEffect;

    bool hasCollided = false, hasPushed = false;

    Vector3 rotateAxis;


    float radius = 2;

    //public override void OnStartAuthority()
    //{
    //    enabled = true;
    //}

    private void Start()
    {
        rotateAxis = new Vector3(rotateX, rotateY, rotateZ);// point of boulder-rotation in air.
    }  

    [Server]
    private void Update()
    {
        
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
                hasCollided = true;
            }
        }
        catch
        {
            Debug.Log("No ball existing");
        }
        
        if (!hasCollided) //The boulder will only be forced to rotate during the initial airtime.
        {
            transform.Rotate(rotateAxis, 3f);
        }

    }
    //[Server]
    //void OnCollisionEnter(Collision col)
    //{
    //    hasCollided = true;
        
    //    if (col.gameObject.CompareTag("Enemy"))//if the ball is the object being collided with.
    //        {
    //            CmdSpawnHitEffect(transform.position);
    //            Debug.Log("Collided with ball");
    //            CmdDoPush(col.gameObject);
    //            Destroy(gameObject);//Destroy boulder when it explodes. this can be changed to splitting stones in later itteration.
    //        }
    //}

    [Server]
    void ServerSpawnHitEffect(Vector3 hitLocation)
    {
        Debug.Log("Inside Hiteffect");
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);       
        //RpcSpawnHitEffect(hitLocation);
    }

    //[ClientRpc]
    //void RpcSpawnHitEffect(Vector3 hitLocation)
    //{
    //    GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
    //    NetworkServer.Spawn(magicExplosion);
    //}

    [Server]
    void ServerDoPush(GameObject ball)
    {
        Debug.Log("Server is moving the ball for the clients!");
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForceForward, gameObject.transform.position, explosionRadius, explosionForceUpwards);
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForceForward, gameObject.transform.position, explosionRadius);
        //RpcDoExplosion(ball);
        Destroy(gameObject);
    }

    //[Server]
    //void ServerDoExplosion(GameObject ball)
    //{
    //    RpcDoExplosion(ball);
    //}

    [ClientRpc]
    void RpcDoExplosion(GameObject ball)
    {
        Debug.Log("Server is moving the ball for the clients!");
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForceForward, gameObject.transform.position, explosionRadius, explosionForceUpwards);
        Destroy(gameObject);
    }


}
