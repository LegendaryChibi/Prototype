using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    private Vector3 direction;

    private void Update()
    {
        //Move bullet
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void Fire(Vector3 dir)
    {
        //Direction to fire.
        direction = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Destory if wall is hit.
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        if (other.CompareTag("Assassin"))
        {
            Destroy(gameObject);
        }
    }
}
