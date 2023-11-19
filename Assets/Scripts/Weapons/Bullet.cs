using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    private Vector3 direction;

    private void OnEnable()
    {
        StartCoroutine(SelfDestruct());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

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
            gameObject.SetActive(false);
        }
        if (other.CompareTag("Assassin"))
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false); ;
    }
}
