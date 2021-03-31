using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vfx : MonoBehaviour
{
    public GameObject Splash;
    public AudioSource HitSource;
    
    void Start()
    {
        HitSource = GetComponent<AudioSource>();

        
    }
    

    void OnCollisionEnter(Collision co)
    {
        ContactPoint contact = co.contacts[0];

        Vector3 pos = contact.point;
        Quaternion rot = Quaternion.FromToRotation(Vector3.down, contact.point);
        HitSource.Play();
        

        GameObject e = Instantiate(Splash);

        e.transform.position = pos;
        e.transform.rotation = rot;
        
        
        


    }

    


}
