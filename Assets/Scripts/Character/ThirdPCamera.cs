﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Third Person Camera
/// This camera controller script is for a third person camera 
/// attached to a third person character controller that also has a 
/// character attached to it. 
/// </summary>

/* Camera Bumper code modified from a SmoothFollowWithCameraBumper Script
 * from Unity 3D Wiki by Daniel P. Rossi (DR9885)
 */

[AddComponentMenu("Camera-Control/Mouse")]

public class ThirdPCamera : MonoBehaviour 
{
    public Transform target;    //target for camera to interact with
    private Vector3 moveAlong;  //vector for camera zoom

    [SerializeField] private float bumperDistanceCheck = 2.0f;  // length of bumper ray
    [SerializeField] private float bumperCameraHeight = 0.5f;   // adjust camera height while bumping
    [SerializeField] private Vector3 bumperRayOffset;           // allows offset of the bumper ray from target origin
    [SerializeField] private float damping = 5.0f;

    //Start, setup the initial camera position with the character
    void Start()
    {
        // Early out if we don't have a target
        if (!target)
        {
            Debug.Log("Camera has no target at Start.");
            return;
        }

    }//end start

    /// <summary>
    /// Called in sync with physics.
    /// Using to check if the camera will collide with an object.
    /// </summary>
    private void FixedUpdate()
    {
        Vector3 wantedPosition = transform.position;

        // check to see if there is anything behind the target
        RaycastHit hit;
        Vector3 back = transform.TransformDirection(-1 * Vector3.forward);

        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(target.TransformPoint(bumperRayOffset), back, out hit, bumperDistanceCheck)
            && hit.transform != target) // ignore ray-casts that hit the user. DR
        {
            // clamp wanted position to hit position
            wantedPosition.x = hit.point.x;
            wantedPosition.z = hit.point.z;
            wantedPosition.y = Mathf.Lerp(hit.point.y + bumperCameraHeight, wantedPosition.y, Time.deltaTime * damping);

            transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
        }

        //keep the camera looking at the character
        transform.LookAt(target);
    }//end fixed update

    /// <summary>
    /// Called once per frame after update, checking if camera has a target.
    /// </summary>
    private void LateUpdate()
    {
        // Early out if we don't have a target
        if (!target)
        {
            Debug.Log("Camera has lost it's target during run time.");
            return;
        }

        //Debug.DrawLine(transform.position, transform.position + (transform.forward * 1.5f), Color.yellow);
    }//end late update

    /// <summary>
    /// Control camera vertical position and zoom.
    /// Camera does not control it's horizontal positon.
    /// </summary>
    /// <param name="rotationY">Camera Y value to rotate by</param>
    /// <param name="zoom">Camera zoom value to move by</param>
    public void moveCamera(float rotationY, float zoom)
    {
        //get a vector between camera and target
        moveAlong = transform.position - target.transform.position;
        
        //bind the angle between 8 and 160 with a range attribute
        //keep the camera bounded between alomst straight overhead and near to underneath
        float angle = Vector3.Angle(moveAlong, Vector3.up);
        //Debug.Log("Camera Angle: " + angle);
        if (angle > 10 && -rotationY > 0)
        {
            transform.RotateAround(target.transform.position, this.transform.right, -rotationY);
        }
        else if (angle < 160 && -rotationY < 0)
        {
            transform.RotateAround(target.transform.position, this.transform.right, -rotationY);
        }

        //zoom the camera in or out
        //draw a line between the camera and it's target, and move the camera along that line
        moveAlong = transform.position - target.transform.position;

        if (moveAlong.magnitude > 1f && zoom > 0)
        {
            moveAlong.Normalize();
            moveAlong *= -zoom;
            transform.position += moveAlong / 2;
        }
        else if (zoom < 0 && moveAlong.magnitude < 15f)
        {
            moveAlong.Normalize();
            moveAlong *= -zoom;
            transform.position += moveAlong / 2;
        }
    }//end move camera
}//end ThirdPCamera Script
