using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeForce : NetworkBehaviour
{
    [SerializeField] float forceUpwards;

    [SerializeField] GameObject hitEffect;

    [SerializeField] float ballForce = 100f;

    PlayerMovement playerMovement;


    Transform topPartPillar;

    bool hasPlayedAnime = false;//Only plays the animation once (4 smoke clouds on each pillar)
    bool hasCollided = false; //If it collides with ball, adds force once.
    bool hasCollidedPlayer = false;//if pillar has collided with player, reduces the velocity in y for the pillar. Only reduces once.   

    Vector3 originposition;// bottom part of pillar.

    float heightOfPillar;



    public override void OnStartAuthority()
    {
        enabled = true;
    }
    private void Start()
    {
        CalculatePillarHeight();
        //playerMovement = connectionToClient.clientOwnedObjects<PlayerMovement>()
    }
    private void CalculatePillarHeight()
    {
        topPartPillar = gameObject.transform.GetChild(0);// pillar has one child in it's object-structure (pillarTop). NOTE: If this changes this might be obsolete.
        originposition = gameObject.transform.position;
        heightOfPillar = topPartPillar.position.y - originposition.y;
    }
    [Client]
    private void Update()
    {
        if (!hasPlayedAnime)
        {
            CmdSpawnHitEffect();
        }
        Debug.Log(transform.up.ToString());

        if (gameObject.transform.position.y < originposition.y + heightOfPillar)
        {
            if(hasCollidedPlayer)
            {
                CmdMovePillar(forceUpwards * 0.5f);
            }
            else
            {
                CmdMovePillar(forceUpwards);
            }
            
        }
        else if (gameObject.transform.position.y < originposition.y + heightOfPillar && hasCollidedPlayer)
        {
            CmdMovePillar(forceUpwards);
        }


        Collider[] colliders = Physics.OverlapSphere(topPartPillar.transform.position, 5f);
        //takes care of collisions with the player and the ball
        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {

                if (!hasCollided)
                {
                    CmdPushBall(pushedObject.gameObject);
                }
            }
            if (pushedObject.gameObject.CompareTag("Player"))
            {
                hasCollidedPlayer = true;
            }
        }
    }
    [Command]
    void CmdPushBall(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().transform.Translate(0, Time.deltaTime * forceUpwards, 0, Space.World);
        ball.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0) * ballForce, ForceMode.Force);

        hasCollided = true;
    }
    [Command]
    void CmdSpawnHitEffect()
    {
        Vector3 left = new Vector3(-2, 1, 0);
        Vector3 right = new Vector3(2, 1, 0);
        Vector3 above = new Vector3(0, 1, -2);
        Vector3 under = new Vector3(0, 1, 2);

        GameObject magicExplosionLeft = Instantiate(hitEffect, topPartPillar.transform.position + left, Quaternion.identity) as GameObject;
        GameObject magicExplosionRight = Instantiate(hitEffect, topPartPillar.transform.position + right, Quaternion.identity) as GameObject;
        GameObject magicExplosionAbove = Instantiate(hitEffect, topPartPillar.transform.position + above, Quaternion.identity) as GameObject;
        GameObject magicExplosionBelow = Instantiate(hitEffect, topPartPillar.transform.position + under, Quaternion.identity) as GameObject;

        NetworkServer.Spawn(magicExplosionLeft);
        NetworkServer.Spawn(magicExplosionRight);
        NetworkServer.Spawn(magicExplosionAbove);
        NetworkServer.Spawn(magicExplosionBelow);

        hasPlayedAnime = true;
    }

    [Command]
    void CmdMovePillar(float forceUpwards)
    {
        transform.Translate(0, Time.deltaTime * forceUpwards, 0, Space.World);
    }
}

