using UnityEngine;
using System.Collections;

public class MeleeState : FSMState
{
    private MeleeAIProperties meleeAIProperties;
    private AssassinControllerAI assassin;
    bool heal = true;
    private Vector3 dir;

    //Constructor
    public MeleeState(AssassinControllerAI controller, MeleeAIProperties meleeAIProperties, Transform trans, Transform playerTransform)
    {
        this.meleeAIProperties = meleeAIProperties;
        assassin = controller;
        stateID = FSMStateID.Melee;
        assassin = controller;
        destPos = playerTransform.position - trans.position;
    }

    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        //Check if Assassin has died
        if (assassin.Health == 0)
        {
            assassin.PerformTransition(Transition.NoHealth);
            return;
        }

        //If player gets too far to attack, chase them 
        if (assassin.DistToPlayer() > meleeAIProperties.chaseDistance)
        {
            assassin.PerformTransition(Transition.Aggravated);
            heal = true;
        }
    }

    //Act
    public override void Act(Transform player, Transform npc)
    {
        //Get position of the player
        dir = assassin.GetAimDirection().normalized;
        //Look at the player
        assassin.rb.MoveRotation(Quaternion.LookRotation(dir));
        //Attack the player
        assassin.animator.SetTrigger("Attack");
        if (heal && assassin.Health < 100) 
        {
            assassin.AddHealth(5);
            heal = false;
        }
    }
}
