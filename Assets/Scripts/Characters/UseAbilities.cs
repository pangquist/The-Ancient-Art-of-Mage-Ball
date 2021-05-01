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

    [SerializeField] float cooldownAbility1 = 5;
    [SerializeField] float cooldownAbility2;
    [SerializeField] float cooldownAbility3;
    [SerializeField] float cooldownAbility4;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    private void Update()
    {
        cooldownAbility1 -= Time.deltaTime;

        if (Input.GetKeyDown(castAbility1Button) && cooldownAbility1 <= 0)
        {
            Debug.Log("Trying to cast ability 1!");
            UseAbility1();
        }
    }

    void UseAbility1()
    {
        gameObject.GetComponent<GravitySwap>().DoSpell();
    }
}
