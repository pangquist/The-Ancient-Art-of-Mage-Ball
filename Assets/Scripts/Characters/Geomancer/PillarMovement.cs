using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PillarMovement : NetworkBehaviour
{
    [Header("Dependency")]
    [SerializeField] Transform pillarTop;

    [Header("Values")]
    [SerializeField] float forceUpwards;

    GameObject [] ground;

    GameObject playerObject;
    GamestateManager gameState;
    GameObject pillarSmokeEffect;

    Transform pillarPosition;

    bool hasSpawnedEffect = false;

    float heightOfPillar;

    bool goingUp = true;

    Vector3 effectPosition;
  
    void Start()
    {
        ground = GameObject.FindGameObjectsWithTag("Ground");
        CalculatePillarHeight();
        //Gets the players in the game
        List<MyNetworkPlayer> players = ((MyNetworkManager)NetworkManager.singleton).Players;        
        foreach (MyNetworkPlayer player in players)
        {
            if (player.hasAuthority)
            {
                playerObject = player.gameObject;//playerObject is a reference to the player that spawned the object
            }
        }
        pillarPosition = playerObject.transform.Find("PillarSpawnPoint");
        gameState = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
    }

    private void CalculatePillarHeight()
    {
        // pillar has one child in it's object-structure (pillarTop). NOTE: If this changes this might be obsolete.
        heightOfPillar = pillarTop.position.y - gameObject.transform.position.y;
    }

    [Client]
    private void Update()
    {
        Debug.Log(ground[0].transform.position.y);

        if (!hasAuthority)
        {
            return;
        }
        //match is paused occurs when a goal is made, this prevents the objects from affecting the ball during pauses.
        if (gameState.matchIsPaused)
        {
            Destroy(gameObject);
        }

        Vector3 pillarPos = pillarPosition.transform.position - pillarTop.transform.position;

        //pillar X/Z Movement
        gameObject.transform.Translate(pillarPos.x, 0, pillarPos.z);

        //stops movement when the whole pillar is above ground
        if (gameObject.transform.position.y >= ground[0].transform.position.y)
        {
            goingUp = false;
        }

        //pillar Y movement
        if (goingUp)
        {           
            gameObject.transform.Translate(Vector3.up * forceUpwards * Time.deltaTime, Space.World);          
        }
        else
        {

        }

        playerObject.GetComponent<PlayerMovement>().velocity.y = 0;
    }
}
