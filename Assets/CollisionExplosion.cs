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
    float explosionForce;

    [SerializeField]
    GameObject hitEffect;


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
                Debug.Log("Collided with ball");
                CmdDoPush(col.gameObject);
            }

    }

    [Command]
    void CmdDoPush(GameObject ball)
    {
        Debug.Log("Server is moving the ball for the clients!");
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, ball.transform.position, 30);
    }


}
