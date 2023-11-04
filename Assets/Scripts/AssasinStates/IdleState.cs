using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using System.Threading;

public class IdleState : FSMState
{
    private IdleAIProperties idleAIProperties;
    private AssasinControllerAI assasin;

    //Constructor
    public IdleState(AssasinControllerAI controller, IdleAIProperties idleAIProperties, Transform trans, Transform playerTransform)
    {
        this.idleAIProperties = idleAIProperties;
        assasin = controller;
        stateID = FSMStateID.Idle;
        assasin = controller;
        destPos = playerTransform.position - trans.position;
    }

    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        //Check if Assasin has died
        if (assasin.Health == 0)
        {
            assasin.PerformTransition(Transition.NoHealth);
            return;
        }

        //If player comes within a certain distance, chase them
        if (assasin.DistToPlayer() < idleAIProperties.chaseDistance)
        {
            assasin.PerformTransition(Transition.Agrovated);
        }

    }

    //Act
    public override void Act(Transform player, Transform npc)
    {
    }
}
