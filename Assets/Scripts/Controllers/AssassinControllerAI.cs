﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

[System.Serializable]
public abstract class AIProperties
{
    public float speed = 10.0f;
    public float chaseDistance = 5;
    public float healthRegenRate = 0;
}

[System.Serializable]   
public class ChaseAIProperties : AIProperties
{

}

[System.Serializable]
public class MeleeAIProperties : AIProperties
{

}

[System.Serializable]
public class IdleAIProperties : AIProperties
{

}

public class AssassinControllerAI : AdvancedFSM
{
    [SerializeField]
    private bool debugDraw;
    [SerializeField]
    private Text StateText;
    [SerializeField]
    private Text HealthText;

    public Rigidbody rb;

    [SerializeField]
    private ChaseAIProperties chaseAIProperties;

    [SerializeField]
    private MeleeAIProperties meleeAIProperties;

    [SerializeField]
    private IdleAIProperties idleAIProperties;

    public Animator animator;

    public ParticleSystem chaseEffect;

    [SerializeField]
    private Transform enemyPosition;

    [SerializeField]
    private GameObject body;

    private float health;
    public float Health
    {
        get { return health; }
    }

    bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
    }

    public void DecHealth(float amount) { health = Mathf.Max(0, health - amount); }
    public void AddHealth(float amount) { health = Mathf.Min(100, health + amount); }

    private string GetStateString()
    {

        string state = "NONE";
        if (CurrentState.ID == FSMStateID.Dead)
        {
            state = "DEAD";
        }
        else if (CurrentState.ID == FSMStateID.Chasing)
        {
            state = "CHASE";
        }
        else if (CurrentState.ID == FSMStateID.Idle)
        {
            state = "IDLE";
        }
        else if (CurrentState.ID == FSMStateID.Melee)
        {
            state = "ATTACK";
        }

        return state;
    }

    protected override void Initialize()
    {
        GameObject objPlayer = GameManager.Player;
        playerTransform = objPlayer.transform;
        health = 100;
        LevelManager.Instance.RegisterEnemy(this);
        ConstructFSM();
    }

    public void Reset()
    {
        isDead = false;
        gameObject.transform.position = enemyPosition.position;
        gameObject.transform.rotation = enemyPosition.rotation;
        body.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        body.gameObject.transform.localScale = Vector3.one;
        health = 100;
        gameObject.SetActive(true);
    }

    protected override void FSMUpdate()
    {

        if (CurrentState != null)
        {
            CurrentState.Reason(playerTransform, transform);
            CurrentState.Act(playerTransform, transform);
        }
/*        StateText.text = "ASSASSIN STATE IS: " + GetStateString();
        HealthText.text = "ASSASSIN HEALTH IS: " + Health;

        if (debugDraw)
        {
            Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.red);
        }*/
    }



    private void ConstructFSM()
    {
        //
        //Create States
        //
        //Create Idle state

        IdleState idleState = new IdleState(this, idleAIProperties, transform, playerTransform);

        //add transitions OUT of the idle state

        idleState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        idleState.AddTransition(Transition.Aggravated, FSMStateID.Chasing);

        //Create Chase state

        ChaseState chaseState = new ChaseState(this, chaseAIProperties, transform, playerTransform);
  
        //add transitions OUT of the chase state

        chaseState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        chaseState.AddTransition(Transition.InRange, FSMStateID.Melee);

        //Create Melee state
        MeleeState meleeState = new MeleeState(this, meleeAIProperties, transform, playerTransform);

        //add transitions OUT of the melee state
        meleeState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        meleeState.AddTransition(Transition.Aggravated, FSMStateID.Chasing);

        //Create the Dead state
        DeadState deadState = new DeadState(this);
        //there are no transitions out of the dead state
        deadState.AddTransition(Transition.Reset, FSMStateID.Idle);


        //Add all states to the state list
        AddFSMState(idleState);
        AddFSMState(chaseState);
        AddFSMState(meleeState);
        AddFSMState(deadState);
    }

    public void StartDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        //Play death animation and destroy
        animator.SetTrigger("Death");
        isDead = true;
        yield return new WaitForSeconds(0.4f);

        gameObject.SetActive(false);
    }

    public void ChasePlayer(Vector3 moveVector)
    {
        //Get vector and move assassin
        Vector3 position = transform.position + moveVector * chaseAIProperties.speed * Time.deltaTime;
        rb.MovePosition(position);
    }

    public Vector3 GetAimDirection()
    {
        //Return vector aiming at the player
        Vector3 aimDir = playerTransform.position - this.transform.position;
        aimDir.y = 0;
        return aimDir;
    }

    public float DistToPlayer()
    {
        //return the distance to the player
        float dist = Vector3.Distance(this.transform.position, playerTransform.position);
        return dist;
    }
}