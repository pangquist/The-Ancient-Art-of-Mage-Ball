using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarBridge : NetworkBehaviour
{
    [SerializeField] GameObject pillarBridgePrefab;

    [SerializeField] float abilityDuration;

    [SerializeField] float pillarSpawnInterval;

    [SerializeField] float forceUpwards;

    [SerializeField] GameObject hitEffect;

    [SerializeField] float ballForce = 100f;

    UseAbilities useAbility;

    GameObject pillarSpawn;

    List<GameObject> pillarEffects;

    bool hasCollided;

    [Header("Multiply with player direction")]
    [SerializeField] float offSetPillarSpawnMultiplier = 2;

    Transform pillarTop;

    GameObject[] groundTransform;

    PlayerMovement playerMovement;

    Vector3 groundPositionUnderPlayer;

    Vector3 playerFuturePosition;

    GameObject magicExplosionLeft;
    GameObject magicExplosionRight;
    GameObject magicExplosionAbove;
    GameObject magicExplosionBelow;

    float timer = 0;
    bool hasSpawnedPillar = false;

    public override void OnStartAuthority()
    {
        enabled = true;
    }
    private void Start()
    {
        groundTransform = GameObject.FindGameObjectsWithTag("Ground");
        hasSpawnedPillar = false;
        useAbility = gameObject.GetComponent<UseAbilities>();        
    }

    [Client]
    void DoPillarBridge()
    {        
        pillarTop = pillarBridgePrefab.transform.GetChild(0);
        timer = 0;
        playerMovement = GetComponent<PlayerMovement>();
        CmdSpawnPillarBridge();
        hasSpawnedPillar = true;
        CmdSpawnHitEffect();
    }

    [Client]
    private void Update()
    {
        if (hasSpawnedPillar)
        {
                timer += Time.deltaTime;

                if (timer > abilityDuration)//Handles the duration of the ability
                {
                    CmdDestroyPillar();
                    hasSpawnedPillar = false;
                }
                else
                {
                    CmdMovePillar(forceUpwards);
                    CmdMoveHitEffect();
                }

            Collider[] colliders = Physics.OverlapSphere(pillarTop.transform.position, 5f);
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
            }
        }                                  
    }

    [Command]
    void CmdSpawnPillarBridge()//Spawns pillar at groundlevel under player
    {
        playerFuturePosition = gameObject.transform.forward;
        groundPositionUnderPlayer = new Vector3(gameObject.transform.position.x, groundTransform[0].transform.position.y, gameObject.transform.position.z);

        pillarSpawn = Instantiate(pillarBridgePrefab, groundPositionUnderPlayer - pillarTop.position, Quaternion.identity);

        NetworkServer.Spawn(pillarSpawn, connectionToClient);

        hasSpawnedPillar = true;
    }

    [Command]
    void CmdMovePillar(float forceUpwards)
    {
        pillarSpawn.transform.Translate(playerMovement.move * playerMovement.speed * Time.deltaTime);
        pillarSpawn.transform.Translate(Vector3.up * forceUpwards * Time.deltaTime);
    }

    [Command]
    void CmdDestroyPillar()
    {
        Destroy(pillarSpawn);
    }

    [Command]
    void CmdSpawnHitEffect()
    {
        Vector3 left = new Vector3(-2, 1, 0);
        Vector3 right = new Vector3(2, 1, 0);
        Vector3 above = new Vector3(0, 1, -2);
        Vector3 under = new Vector3(0, 1, 2);

        magicExplosionLeft = Instantiate(hitEffect, groundPositionUnderPlayer + left, Quaternion.identity) as GameObject;
        magicExplosionRight = Instantiate(hitEffect, groundPositionUnderPlayer + right, Quaternion.identity) as GameObject;
        magicExplosionAbove = Instantiate(hitEffect, groundPositionUnderPlayer + above, Quaternion.identity) as GameObject;
        magicExplosionBelow = Instantiate(hitEffect, groundPositionUnderPlayer + under, Quaternion.identity) as GameObject;

        NetworkServer.Spawn(magicExplosionLeft);
        NetworkServer.Spawn(magicExplosionRight);
        NetworkServer.Spawn(magicExplosionAbove);
        NetworkServer.Spawn(magicExplosionBelow);
    }

    [Command]
    void CmdMoveHitEffect()
    {
        Vector3 playerMove = playerMovement.move * playerMovement.speed * Time.deltaTime;
       
        magicExplosionLeft.transform.Translate(playerMove, Space.World);
        magicExplosionRight.transform.Translate(playerMove, Space.World);
        magicExplosionAbove.transform.Translate(playerMove, Space.World);
        magicExplosionBelow.transform.Translate(playerMove, Space.World);

    }
    [Command]
    void CmdPushBall(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().transform.Translate(0, Time.deltaTime * forceUpwards, 0, Space.World);
        ball.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0) * ballForce, ForceMode.Force);

        hasCollided = true;
    }


}
