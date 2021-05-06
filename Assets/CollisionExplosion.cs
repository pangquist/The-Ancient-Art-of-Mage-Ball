using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionExplosion : NetworkBehaviour
{
    [SerializeField]
    float rotateX, rotateY, rotateZ;
    Vector3 rotateAxis = new Vector3(4, 5, 2);
    bool hasCollided = false;
    [SerializeField]
    float explosionForce, explosionUpwards;
    [SerializeField]
    float explosionRadius;


    [SerializeField]
    GameObject hitEffect;

    [Client]
    private void Update()
    {
        if (!hasCollided)
        {
            transform.Rotate(rotateAxis, 3f);
        }

    }
    [Client]
    void OnCollisionEnter(Collision col)
    {
        hasCollided = true;
        
        if (col.gameObject.CompareTag("Enemy"))
            {
            CmdSpawnHitEffect(transform.position);
                Debug.Log("Collided with ball");
                CmdDoPush(col.gameObject);
            Destroy(gameObject);
            }


    }
    [Command]
    void CmdSpawnHitEffect(Vector3 hitLocation)
    {
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);
        Debug.Log($"Intantiating the hit effect: {magicExplosion}");
    }

    [Command]
    void CmdDoPush(GameObject ball)
    {
        Debug.Log("Server is moving the ball for the clients!");
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, gameObject.transform.position, explosionRadius, explosionUpwards);
        
    }


}
