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

    //[SerializeField] GameObject hitEffect;

    [SerializeField] float ballForce = 100f;

    GameObject pillarSpawn;

    bool hasCollided;

    [Header("Multiply with player direction")]
    [SerializeField] float offSetPillarSpawnMultiplier = 2;

    Transform pillarTop;

    GameObject[] groundTransform;

    Vector3 groundPositionUnderPlayer;

    Vector3 playerFuturePosition;

    GameObject pillarSmoke;

    float timer;
    bool hasSpawnedPillar = false;

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
        timer = 0;
        groundPositionUnderPlayer = new Vector3(gameObject.transform.position.x, groundTransform[0].transform.position.y, gameObject.transform.position.z);
        CmdSpawnPillarBridge(gameObject, groundPositionUnderPlayer);
        hasSpawnedPillar = true;
    }

    [Client]
    private void Update()
    {
        if (hasSpawnedPillar)
        {
            Debug.Log("Inside Pillarbridge");
            timer += Time.deltaTime;

            if (timer > abilityDuration)//Handles the duration of the ability
            {
                CmdDestroyPillar();
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
