using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthLevitate : NetworkBehaviour
{
    [SerializeField] GameObject pillarBridgePrefab;
    [SerializeField] float abilityDuration;
    [SerializeField] float pillarSpawnInterval;
    [SerializeField] float forceUpwards;
    [SerializeField] float ballForce = 100f;
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] Animator animator;

    GameObject pillarSpawn;
    GameObject[] groundTransform;
    GameObject pillarSmoke;
    Transform pillarTop;

    Vector3 groundPositionUnderPlayer;
    Vector3 playerFuturePosition;

    float timer;
    bool hasSpawnedPillar = false;
    bool hasCollided;


    [Header("Offset")]
    [Tooltip("Multiply with player direction")]
    [SerializeField] float offSetPillarSpawnMultiplier = 2;


    public override void OnStartAuthority()
    {
        enabled = true;
    }
    private void Start()
    {
        groundTransform = GameObject.FindGameObjectsWithTag("Ground");
        hasSpawnedPillar = false;
        pillarTop = pillarBridgePrefab.transform.GetChild(0);
    }

    [Client]
    void DoPillarBridge()
    {
        animator.SetBool("isPose", true);
        timer = 0;
        groundPositionUnderPlayer = new Vector3(gameObject.transform.position.x, groundTransform[0].transform.position.y, gameObject.transform.position.z);
        CmdSpawnPillarBridge(gameObject, groundPositionUnderPlayer);
        hasSpawnedPillar = true;
        useAbilities.SetOnCooldownAbility3();
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
                animator.SetBool("isPose", false);
                hasSpawnedPillar = false;
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
                        hasCollided = true;
                    }
                }
            }
        }
    }

    [Command]
    void CmdSpawnPillarBridge(GameObject player, Vector3 position)//Spawns pillar at groundlevel under player
    {
        pillarTop = pillarBridgePrefab.transform.GetChild(0);
        playerFuturePosition = player.transform.forward;
        groundPositionUnderPlayer = position;
        
        pillarSpawn = Instantiate(pillarBridgePrefab, groundPositionUnderPlayer - pillarTop.position, Quaternion.identity);

        NetworkServer.Spawn(pillarSpawn, connectionToClient);
    }

    [Command]
    void CmdDestroyPillar()
    {
        NetworkServer.Destroy(pillarSpawn);
    }

    [Command]
    void CmdPushBall(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().transform.Translate(0, Time.deltaTime * forceUpwards, 0, Space.World);
        ball.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0) * ballForce, ForceMode.Force);
    }


}
