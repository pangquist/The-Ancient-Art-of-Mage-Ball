using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAbilities : NetworkBehaviour
{
    [SerializeField] KeyCode castAbility1Button;
    [SerializeField] KeyCode castAbility2Button;
    [SerializeField] KeyCode castAbility3Button;
    [SerializeField] KeyCode castAbility4Button;

    [SerializeField] float cooldownAbility1; //Stop the hardcode
    [SerializeField] float cooldownAbility2;
    [SerializeField] float cooldownAbility3;
    [SerializeField] float cooldownAbility4;

    public override void OnStartAuthority()
    {
        enabled = true;

        cooldownAbility1 = 0;
        cooldownAbility2 = 0;
        cooldownAbility3 = 0;
        cooldownAbility4 = 0;
    }

    private void Update()
    {
        cooldownAbility1 -= Time.deltaTime;
        cooldownAbility2 -= Time.deltaTime;

        if (Input.GetKeyDown(castAbility1Button) && cooldownAbility1 <= 0)
        {
            Debug.Log("Trying to cast ability 1!");
            UseAbility1();
        }

        if (Input.GetKeyDown(castAbility2Button) && cooldownAbility2 <= 0)
        {
            Debug.Log("Trying to cast ability 2!");
            UseAbility2();
        }
    }

    void UseAbility1()
    {
        gameObject.GetComponent<GravitySwap>().DoSpell();
        cooldownAbility1 = 5;
    }

    void UseAbility2()
    {
        gameObject.GetComponent<ForceWall>().DoSpell();
        cooldownAbility2 = 10;
    }
}
