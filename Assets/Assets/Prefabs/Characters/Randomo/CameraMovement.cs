using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform headPosition;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position.Set(gameObject.transform.position.x, gameObject.transform.position.y, headPosition.localPosition.z + 1);
    }
}
