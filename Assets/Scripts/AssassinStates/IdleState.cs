using UnityEngine;
using System.Collections;

public class IdleState : FSMState
{
    private IdleAIProperties idleAIProperties;
    private AssassinControllerAI assassin;

    //Constructor
    public IdleState(AssassinControllerAI controller, IdleAIProperties idleAIProperties, Transform trans, Transform playerTransform)
    {
        this.idleAIProperties = idleAIProperties;
        assassin = controller;
        stateID = FSMStateID.Idle;
        assassin = controller;
        destPos = playerTransform.position - trans.position;
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

        //If player comes within a certain distance, chase them
        if (assassin.DistToPlayer() < idleAIProperties.chaseDistance)
        {
            assassin.PerformTransition(Transition.Aggravated);
        }

    }

    //Act
    public override void Act(Transform player, Transform npc)
    {
    }
}
