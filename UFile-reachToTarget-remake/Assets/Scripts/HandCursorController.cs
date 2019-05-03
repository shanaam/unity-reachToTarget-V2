using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * File:    HandCursorController.cs
 * Project: ReachToTarget-Remake
 * Author:  Peter Caruana
 * York University 2019 (c)
 */
public class HandCursorController : MonoBehaviour
{
    //link to the actual hand position object
    public GameObject realHand;
    public TargetContainerController targetContainerController;

    bool isInTarget = false;
    bool isInHome = false;
    public bool collisionHeld = false;
    private float collision_start_time;
    // Use this for initialization
    void Start()
    {
        // disable the whole task initially to give time for the experimenter to use the UI
        // gameObject.SetActive(false);
    }

    // ALL tracking should be done in LateUpdate
    // This ensures that the real object has finished moving (in Update) before the tracking object is moved
    void LateUpdate()
    {
        //transform.localPosition = realHand.transform.position - transform.parent.transform.position;

        Vector3 realHandPosition = realHand.transform.position;
        Vector3 rotatorObjectPosition = transform.parent.transform.position;

        // if trial.setting == aligned
        transform.localPosition = realHandPosition - rotatorObjectPosition;

        // if trial.setting == rotated

        // if trial.setting == clamped

        // if trial.setting == nocursor

        // OR can this be a system? It only happens within the handCursor, we won't ever spawn multiple so it doesn't have to be I think
        // the target on the other hand can be spawned via something attached to the rotator obj
        // homeposition should always be there, just turn off rendering when not needed (prevents the issue of it respawning at trial beginning)
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(">> Collision Detected ");
        collision_start_time = Time.time; 
        if (other.CompareTag("Target"))
        {
            isInTarget = true;

        }
        else if (other.CompareTag("Home"))
        {
            isInHome = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isInTarget = false;
        
    }

    private void OnTriggerStay(Collider other)
    {
        float delta = Time.time - collision_start_time;
        if(delta >= 0.2f)// if the collision is held for longer than 0.2 seconds
        {
            if (isInTarget)
            {
                Debug.Log("Collision Held for " + delta + " seconds");
                collisionHeld = true; //Then the collision was deliberate by the user and held on the location
                string msg = targetCollision();
                Debug.Log(msg);
            }
            else if (isInHome) ;
                
        }
        
    }

    private string homeCollision(Collider other)
    {
        return "Colided with home.";
    }

    private string targetCollision()
    {
        targetContainerController.Destroy();
        return "Colided with target. Destroying all targets";
    }

}
