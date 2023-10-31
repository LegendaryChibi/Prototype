using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Get 2 directional vectors from standard inputs
    private Vector2 inputVector;
    public Vector2 InputVector { get { return inputVector; } }

    //Layer mask for camera aim
    [SerializeField]
    private LayerMask aimDetectionLayer;

    //Detect gunshots and sword inputs 
    private bool gunshot = false;
    public bool Gunshot {  get { return gunshot; } }

    private bool swordSwing = false;
    public bool SwordSwing { get {  return swordSwing; } }

    void Update()
    {
        //Detect all inputs
        var horz = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");

        CheckForShot();
        CheckForSwing();

        inputVector = new Vector2(horz, vert);
    }

    private void CheckForShot()
    {
        gunshot = Input.GetButton("Fire1");

    }

    private void CheckForSwing()
    {
        swordSwing = Input.GetButton("Fire2");
    }

    public Vector3 GetAimDirection(PlayerBody body)
    {
        //Create a plane and raycast to detect mouse movement on screen accuratlety
        Plane plane = new Plane(Vector3.up, body.Gun.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distanceToPlane;
        plane.Raycast(ray, out distanceToPlane);

        Vector3 mouseWorldPos = ray.GetPoint(distanceToPlane);

        Vector3 aimDir = mouseWorldPos - body.transform.position;
        aimDir.y = 0;
        return aimDir;
    }
}
