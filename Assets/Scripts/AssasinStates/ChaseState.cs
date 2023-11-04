using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class ChaseState : FSMState
{
    private ChaseAIProperties chaseAIProperties;
    private AssasinControllerAI assasin;
    bool moving;
    private Vector3 dir;

    //Constructor
    public ChaseState(AssasinControllerAI controller, ChaseAIProperties chaseAIProperties, Transform trans, Transform playerTransform)
    {
        this.chaseAIProperties = chaseAIProperties;
        assasin = controller;
        stateID = FSMStateID.Chasing;
        assasin = controller;
        curSpeed = chaseAIProperties.speed;
        destPos = playerTransform.position - trans.position;
        moving = true;
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

        //If player gets close enough in range, attack them
        if (assasin.DistToPlayer() < chaseAIProperties.chaseDistance)
        {
            assasin.PerformTransition(Transition.InRange);
        }
    }

    //Act
    public override void Act(Transform player, Transform npc)
    {
        if (moving)
        {
            dir = assasin.GetAimDirection().normalized;
            assasin.rb.MoveRotation(Quaternion.LookRotation(dir));
            assasin.ChasePlayer(dir);

        }
    }
}
