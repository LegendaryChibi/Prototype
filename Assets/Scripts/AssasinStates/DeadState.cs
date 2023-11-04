using UnityEngine;
using System.Collections;

public class DeadState : FSMState
{
    private bool deathStarted = false;

    private AssasinControllerAI assasin;

    //Constructor
    public DeadState(AssasinControllerAI controller)
    {
        stateID = FSMStateID.Dead;
        assasin = controller;
        curSpeed = 0.0f;
        curRotSpeed = 0.0f;
    }

    //Reason
    public override void Reason(Transform player, Transform npc)
    {
    }

    //Act
    public override void Act(Transform player, Transform npc)
    {
        if (!deathStarted)
        {
            assasin.StartDeath();
        }
    }
}
