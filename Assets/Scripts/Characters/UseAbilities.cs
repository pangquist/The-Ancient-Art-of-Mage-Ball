using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAbilities : MonoBehaviour
{
    [SerializeField] KeyCode castAbility1Button;
    [SerializeField] KeyCode castAbility2Button;
    [SerializeField] KeyCode castAbility3Button;
    [SerializeField] KeyCode castAbility4Button;

    [SerializeField] float cooldownAbility1 = 5;
    [SerializeField] float cooldownAbility2;
    [SerializeField] float cooldownAbility3;
    [SerializeField] float cooldownAbility4;

    private void Update()
    {
        cooldownAbility1 -= Time.deltaTime;

        if (Input.GetKeyDown(castAbility1Button) && cooldownAbility1 <= 0)
        {
            UseAbility1();
        }
    }

    void UseAbility1()
    {
        gameObject.GetComponent<GravitySwap>().DoSpell();
    }
}
