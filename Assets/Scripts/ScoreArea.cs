using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreArea : MonoBehaviour
{

    public GameObject effectObject;
    public GameObject parentName;
    [SerializeField] GamestateManager gmManager;


    private void Start()
    {
        effectObject.SetActive(false);

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {            
            effectObject.SetActive(true);
            AddScore();            
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
