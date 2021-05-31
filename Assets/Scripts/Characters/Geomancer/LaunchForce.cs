using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchForce : NetworkBehaviour
{
    [SerializeField] float forceUpwards;

    [SerializeField] GameObject hitEffect;

    [SerializeField] float ballForce = 100f;

    [Header("This is a radius, around 2 is default")]
    [SerializeField] float collideBallRadius = 2;

    Transform topPartPillar;
    //So animation and collision only happens once.
    bool hasPlayedAnime = false;
    bool hasCollided = false;

    Vector3 originposition;

    GamestateManager gameState;

    float heightOfPillar;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    private void Start()
    {
        CalculatePillarHeight();
        gameState = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
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

        if (gameState.matchIsPaused)
        {
            Destroy(gameObject);
        }

        if (!hasPlayedAnime)
        {
            CmdSpawnHitEffect();
            hasPlayedAnime = true;
        }

        if (gameObject.transform.position.y < originposition.y + heightOfPillar)
        {
            transform.Translate(Vector3.up * forceUpwards * Time.deltaTime, Space.World);
        }

        Collider[] colliders = Physics.OverlapSphere(topPartPillar.transform.position, collideBallRadius);

        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {                
                if (!hasCollided)
                {
                    CmdPushBall(pushedObject.gameObject);
                    hasCollided = true;
                }                
            }
        }
    }

    [Command]
    void CmdPushBall(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().transform.Translate(Vector3.up * forceUpwards * Time.deltaTime);

        ball.GetComponent<Rigidbody>().AddForce(Vector3.up * ballForce, ForceMode.Force);           
    }

    [Command]
    void CmdSpawnHitEffect()
    {
        Vector3 left = new Vector3(-2, 0, 0);
        Vector3 right = new Vector3(2, 0, 0);
        Vector3 above = new Vector3(0, 0, -2);
        Vector3 under = new Vector3(0, 0, 2);

        GameObject magicExplosionLeft = Instantiate(hitEffect, topPartPillar.transform.position + left, Quaternion.identity) as GameObject;
        GameObject magicExplosionRight = Instantiate(hitEffect, topPartPillar.transform.position + right, Quaternion.identity) as GameObject;
        GameObject magicExplosionAbove = Instantiate(hitEffect, topPartPillar.transform.position + above, Quaternion.identity) as GameObject;
        GameObject magicExplosionBelow = Instantiate(hitEffect, topPartPillar.transform.position + under, Quaternion.identity) as GameObject;

        NetworkServer.Spawn(magicExplosionLeft);
        NetworkServer.Spawn(magicExplosionRight);
        NetworkServer.Spawn(magicExplosionAbove);
        NetworkServer.Spawn(magicExplosionBelow);
    }
}
