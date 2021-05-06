using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreArea : NetworkBehaviour
{
    public GameObject effectObject;
    public GameObject parentName;
    [SerializeField] GamestateManager gmManager;
    GameObject ballStartPos;

    private void Start()
    {
        ballStartPos = GameObject.Find("BallSpawnPosition");
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

    [Server]
    private void AddScore()
    {
        if (parentName.name == "Goal Blue")
        {
            gmManager.RedScore += 1;
        }
        else
        {
            gmManager.BlueScore += 1;
        }
        
    }
    
    private void ResetBallPosition(GameObject ball)
    {
        ball.transform.position = ballStartPos.transform.position;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

}
