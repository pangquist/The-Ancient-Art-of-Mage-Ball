using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreArea : NetworkBehaviour
{
    public GameObject effectObject;
    [SerializeField] string teamOwnership;
    [SerializeField] GamestateManager gmManager;
    GameObject ballStartPos;

    public static event Action ClientOnGoal;

    private void Start()
    {
        ballStartPos = GameObject.Find("BallSpawnPosition");
        gmManager = GameObject.Find("GamestateManager").GetComponent<GamestateManager>();
    }

    //Triggers an effect when the ball hits the goal, and resets the ball to start position
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            GameObject effect = Instantiate(effectObject, gameObject.transform.position, gameObject.transform.rotation);
            NetworkServer.Spawn(effect);
            AddScore();
            ResetBallPosition(other.gameObject);
        }     
    }
    
    private void AddScore()
    {
        if (teamOwnership == "Blue")
        {
            gmManager.RedScore += 1;
        }
        else if (teamOwnership == "Red")
        {
            gmManager.BlueScore += 1;
        }
        //gmManager.OnGoal();
        Debug.Log("TRIGGERING CLIENT ON GOAL");
        ClientOnGoal?.Invoke();
    }
    
    private void ResetBallPosition(GameObject ball)
    {
        ball.transform.position = ballStartPos.transform.position;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

}
