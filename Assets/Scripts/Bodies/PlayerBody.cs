using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerBody : MonoBehaviour
{
    //Gun Fields
    [SerializeField]
    private GameObject Bullet;

    [SerializeField]
    private Transform gun;
    public Transform Gun { get { return gun; } }

    private Transform fireLocation;

    private bool FiredShot = false;

    //Sword Fields
    private bool SwordSwung = false;

    public GameObject sword;

    //Barrier Fields
    [SerializeField]
    private GameObject barrier;


    //Other Fields
    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    private float movementSpeed = 5f;

    private Vector3 moveVector;

    private Rigidbody rb;

    [SerializeField]
    private ParticleSystem rifleBurstEffect;

    private void Awake()
    {
        //Find the firing location and initialize the rigidbody
        fireLocation = Gun.Find("FireLocation");
        rb = GetComponent<Rigidbody>();
        Debug.Assert(controller != null, "Missing controller on " + gameObject.name);
    }

    private void Update()
    {
        if (controller.Gunshot)
        {
            StartCoroutine(FireShot());
        }

        if (controller.SwordSwing)
        {
            StartCoroutine(SwordSwing());
        }

        //Get aim direction for the camera and move the rigidbody to follow
        Vector3 aimDir = controller.GetAimDirection(this);
        rb.MoveRotation(Quaternion.LookRotation(aimDir));

    }

    private void FixedUpdate()
    {
        //Normalize the movement speed for all movement inlcuding diagonal and move the player either vertcially or horizontally
        controller.InputVector.Normalize();
        rb.velocity = controller.InputVector * movementSpeed * Time.deltaTime;

        moveVector = new Vector3(controller.InputVector.x, 0, controller.InputVector.y);

        Move(moveVector);
    }

    private void Move(Vector3 moveVector)
    {
        //Get vector and move player
        Vector3 position = transform.position + moveVector * movementSpeed * Time.deltaTime;
        rb.MovePosition(position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If Barrier power up collided, destroy it and activate a barrier on the player
        if (collision.gameObject.CompareTag("Barrier"))
        {
            collision.gameObject.SetActive(false);
            BarrierPower();
        }
    }

    IEnumerator FireShot()
    {
        //If bullet exists and shot hasn't been fired since last interval, run.
        if (Bullet && !FiredShot)
        {
            //Get direction
            Vector3 direction = fireLocation.position - Gun.position;
            direction.y = 0f;
            direction.Normalize();
            //Instantiate the bullet, fire it, then set the Fireshot to true to add a .5 second delay before next shot.

            //GameObject bulletFired = GameObject.Instantiate(Bullet, fireLocation.position, Quaternion.identity);

            GameObject bulletFired = ObjectPoolManager.Instance.GetPooledObject(ObjectPoolManager.PoolTypes.Bullet);
            bulletFired.transform.position = fireLocation.position;
            bulletFired.transform.rotation = Quaternion.identity;
            rifleBurstEffect.Play();

            Bullet bullet = bulletFired.GetComponent<Bullet>();
            bulletFired.SetActive(true);
            bullet.Fire(direction);
            FiredShot = true;
            yield return new WaitForSeconds(0.5f);
            FiredShot = false;
        }
    }

    IEnumerator SwordSwing()
    {
        //If sword exists and sword hasn't been swung since last interval, run.
        if (sword && !SwordSwung)
        {
            //Play sword animation, return to base, add delay before next swing.
            sword.GetComponent<Animator>().Play("SwordSwing");
            sword.GetComponent<Animator>().Play("Default State");
            SwordSwung = true;
            yield return new WaitForSeconds(1.1f);
            SwordSwung = false;
        }
    }

    private void BarrierPower()
    {
        if (barrier)
        {
            //Instantiate barrier and create it as a child of the player. 
            GameObject barrierActivated = GameObject.Instantiate(barrier, rb.transform);
            Barrier barrierGo = barrierActivated.GetComponent<Barrier>();
            barrierActivated.SetActive(true);
        }
    }
}


