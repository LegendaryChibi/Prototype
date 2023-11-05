using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class ChaseState : FSMState
{
    private ChaseAIProperties chaseAIProperties;
    private AssassinControllerAI assassin;
    bool moving;
    private Vector3 dir;

    //Constructor
    public ChaseState(AssassinControllerAI controller, ChaseAIProperties chaseAIProperties, Transform trans, Transform playerTransform)
    {
        this.chaseAIProperties = chaseAIProperties;
        assassin = controller;
        stateID = FSMStateID.Chasing;
        assassin = controller;
        curSpeed = chaseAIProperties.speed;
        destPos = playerTransform.position - trans.position;
        moving = true;
    }

    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        //Check if Assasin has died
        if (assassin.Health == 0)
        {
            assassin.PerformTransition(Transition.NoHealth);
            return;
        }

        //If player gets close enough in range, attack them
        if (assassin.DistToPlayer() < chaseAIProperties.chaseDistance)
        {
            assassin.PerformTransition(Transition.InRange);
        }
    }

    //Act
    public override void Act(Transform player, Transform npc)
    {
        if (moving)
        {
            //Play chase animation
            assassin.animator.SetTrigger("Chase");
            //Get position of the player
            dir = assassin.GetAimDirection().normalized;
            //Look at the player
            assassin.rb.MoveRotation(Quaternion.LookRotation(dir));
            //Move towards the player
            assassin.ChasePlayer(dir);
        }
    }
}
