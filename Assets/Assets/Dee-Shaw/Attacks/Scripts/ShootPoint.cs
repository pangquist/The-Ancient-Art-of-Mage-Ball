using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    public Transform firepoint;
    private float shootforce = 0;
    public Rigidbody prefabBullet;
    public float delaytime;
    public float counter;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && counter > delaytime)
        {
            Instantiate(prefabBullet, firepoint.transform.position, firepoint.transform.rotation);

            prefabBullet.AddForce(new Vector3(shootforce, 0, 0));
            counter = 0;

        }
        counter += Time.deltaTime;
       
    }
}
