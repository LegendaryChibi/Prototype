using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public float ShrinkDuration;

    // The target scale of the shrink
    public Vector3 TargetScale = Vector3.one * .5f;

    // The starting scale
    Vector3 startScale;

    // T is our interpolant for our linear interpolation.
    float t = 0;

    private Renderer rend;

    void Start()
    {
        // initialize stuff in Start
        startScale = transform.localScale;
        t = 0;
        //Get Mesh renderer 
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        StartCoroutine(Shrink());
    }

    IEnumerator Shrink()
    {
        // Divide deltaTime by the duration to stretch out the time it takes for t to go from 0 to 1.
        t += Time.deltaTime / ShrinkDuration;

        // Lerp wants the third parameter to go from 0 to 1 over time. 't' will do that for us.
        Vector3 newScale = Vector3.Lerp(startScale, TargetScale, t);
        transform.localScale = newScale;
        
        //As the barrier shrinks, and gets almost half way, the barrier will flicker
        if (t >= 0.5)
        {
            //Mesh renderer, turns on and off every 0.05 seconds when close to the end of it's life. 
            rend.enabled = false;
            yield return new WaitForSeconds(0.05f);
            rend.enabled = true;
        }

        // After reaching target scale, destroy the barrier.
        if (t > 1)
        {
            Destroy(gameObject);
        }
    }
}
