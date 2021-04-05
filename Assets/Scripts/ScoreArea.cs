using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreArea : MonoBehaviour
{

    public GameObject effectObject;
    public GameObject parentName;
    [SerializeField] GamestateManager gmManager;
    GameObject ballStartPos;


    private void Start()
    {
        effectObject.SetActive(false);
        ballStartPos = GameObject.Find("BallSpawnPosition");

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {            
            effectObject.SetActive(true);
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
