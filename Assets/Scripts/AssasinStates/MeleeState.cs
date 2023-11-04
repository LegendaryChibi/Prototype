using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class MeleeState : FSMState
{
    private MeleeAIProperties meleeAIProperties;
    private AssasinControllerAI assasin;

    //Constructor
    public MeleeState(AssasinControllerAI controller, MeleeAIProperties meleeAIProperties, Transform trans, Transform playerTransform)
    {
        this.meleeAIProperties = meleeAIProperties;
        assasin = controller;
        stateID = FSMStateID.Melee;
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

        //If player gets too far to attack, chase them 
        if (assasin.DistToPlayer() > meleeAIProperties.chaseDistance)
        {
            assasin.PerformTransition(Transition.Agrovated);
        }
    }

    //Act
    public override void Act(Transform player, Transform npc)
    {
    }
}
