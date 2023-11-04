using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


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
public class ShootAIProperties : AIProperties
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

public class AssasinControllerAI : AdvancedFSM
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
    private ShootAIProperties shootAIProperties;

    [SerializeField]
    private MeleeAIProperties meleeAIProperties;

    [SerializeField]
    private IdleAIProperties idleAIProperties;

    private float health;
    public float Health
    {
        get { return health; }
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
        Debug.Log("initialized");
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        health = 100;
        ConstructFSM();
    }

    protected override void FSMUpdate()
    {

        if (CurrentState != null)
        {
            CurrentState.Reason(playerTransform, transform);
            CurrentState.Act(playerTransform, transform);
        }
        StateText.text = "ASSASIN STATE IS: " + GetStateString();
        HealthText.text = "ASSASIN HEALTH IS: " + Health;

        if (debugDraw)
        {
            Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.red);
        }
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
        idleState.AddTransition(Transition.Agrovated, FSMStateID.Chasing);

        //Create Chase state

        ChaseState chaseState = new ChaseState(this, chaseAIProperties, transform, playerTransform);
  
        //add transitions OUT of the chase state

        chaseState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        chaseState.AddTransition(Transition.InRange, FSMStateID.Melee);

        //Create Melee state
        MeleeState meleeState = new MeleeState(this, meleeAIProperties, transform, playerTransform);

        //add transitions OUT of the melee state
        meleeState.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        meleeState.AddTransition(Transition.Agrovated, FSMStateID.Chasing);

        //Create the Dead state
        DeadState deadState = new DeadState(this);
        //there are no transitions out of the dead state


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
        Renderer r = GetComponent<Renderer>();
        r.enabled = false;

        yield return new WaitForSeconds(2.2f);

        Destroy(gameObject);
    }

    public void ChasePlayer(Vector3 moveVector)
    {
        //Get vector and move assasin
        Vector3 position = transform.position + moveVector * chaseAIProperties.speed * Time.deltaTime;
        rb.MovePosition(position);
    }

    public Vector3 GetAimDirection()
    {
        Vector3 aimDir = playerTransform.position - this.transform.position;
        aimDir.y = 0;
        return aimDir;
    }

    public float DistToPlayer()
    {
        float dist = Vector3.Distance(this.transform.position, playerTransform.position);
        return dist;
    }
}