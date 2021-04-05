using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreArea : MonoBehaviour
{

    public GameObject effectObject;
    public GameObject parentName;
    public MatchInfo matchInfo;

    private void Start()
    {
        effectObject.SetActive(false);

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {            
            effectObject.SetActive(true);
            //string name = GetComponent<GameObject>().name.ToString();
            //Debug.Log("hello");
            AddScore();            
        }     
    }
    private void AddScore()
    {
        if (parentName.name == "Goal Blue")
        {
            matchInfo.blueScore += 1;
        }
        else
        {
            matchInfo.redScore += 1;
        }
                  

    }

}
