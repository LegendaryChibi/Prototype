using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAssassin : MonoBehaviour
{
    [SerializeField]
    private int attackPower = 25;

    //If attacted object touches the assasin, damage them
    private void OnTriggerEnter(Collider other)
    {
        AssassinControllerAI assassin = other.GetComponent<AssassinControllerAI>();
        if (assassin)
        {
            assassin.DecHealth(attackPower);
        }
    }
}
