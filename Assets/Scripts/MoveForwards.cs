using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwards : MonoBehaviour
{
    [SerializeField]
    int movespeed = 1;

    public void Update()
    {
        transform.Translate(transform.forward * movespeed * Time.deltaTime);
    }
}
