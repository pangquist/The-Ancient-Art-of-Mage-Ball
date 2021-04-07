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
            other.transform.position = ballStartPos.transform.position;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }     
    }
    private void AddScore()
    {
        if (parentName.name == "Goal Blue")
        {
            gmManager.BlueScore += 1;
        }
        else
        {
            gmManager.RedScore += 1;
        }
        
    }

}
