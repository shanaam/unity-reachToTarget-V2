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

    //link to experiment controller (make a static instance of this?)
    public ExperimentController experimentController;

    bool isInTarget = false;
    bool isInHome = false;
    bool isPaused = false;

    public bool collisionHeld = false;
    private float collision_start_time;

    public CursorMovementType movementType;

    //variables used for checking pause
    List<float> distanceFromLastList = new List<float>();
    Vector3 lastPosition;
    float checkForPauseRate = 0.05f;

    //private Vector3 oldPos; //replace this with local position-based transformations

    // Use this for initialization
    void Start()
    {
        // disable the whole task initially to give time for the experimenter to use the UI
        // gameObject.SetActive(false);
        movementType = new AlignedHandCurosor();
    }


    void Update()
    {
        //movementType should be set based on session settings
    }


    // ALL tracking should be done in LateUpdate
    // This ensures that the real object has finished moving (in Update) before the tracking object is moved
    void LateUpdate()
    {
        // get the inputs we need for displaying the cursor
        Vector3 realHandPosition = realHand.transform.position;
        Vector3 centreExpPosition = transform.parent.transform.position;

        //Update position of the cursor based on mvmt type
        transform.localPosition = movementType.NewCursorPosition(realHandPosition, centreExpPosition);

        //Do things when this thing is in the target (and paused)
        //if cursor is paused AND in target
        if (isInTarget)
        {
            /*
            //disable the tracker script (for the return to home position)
            trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = false;

            isPaused = false;
            isInTarget = false;

            CancelInvoke("CheckForPause");

            targetReached = true;
            */

            //End and prepare
            experimentController.EndAndPrepare();

            //Create homeposition
            //homePosition.SetActive(true);
        }

        //Do things when this this is in home (and pause)
        else if (isInHome)
        {
            experimentController.StartTrial();
        }

    }

    //modifiers
    //updates position of handCursor
    

    private void OnTriggerEnter(Collider other)
    {
        // vibrate the controller
        ShortVibrateController();

        //collision_start_time = Time.time; 
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
        isInHome = false;
        InvokeRepeating("CheckForPause", 0, checkForPauseRate);
    }


    /*
    private void OnTriggerStay(Collider other)
    {
        float delta = Time.time - collision_start_time;
        if(delta >= 0.2f)// if the collision is held for longer than 0.2 seconds
        {
            if (isInTarget)
            {
                Debug.Log("Collision Held for " + delta + " seconds");
                collisionHeld = true; //Then the collision was deliberate by the user and held on the location
            }
            else if (isInHome)
            {

            }
                
        }
        
    }
    */


    public void CheckForPause()
    {

        //calculate the distance from last position
        float distance = Vector3.Distance(lastPosition, transform.position);

        float distanceMean = 1000;

        //add the distance to our List
        distanceFromLastList.Add(distance);

        //if List is over a certain length, check some stuff
        if (distanceFromLastList.Count > 8)
        {
            //check and print the average distance
            //float[] distanceArray = distanceFromLastList.ToArray();
            float distanceSum = 0f;

            for (int i = 0; i < distanceFromLastList.Count; i++)
            {
                distanceSum += distanceFromLastList[i];
            }

            distanceMean = distanceSum / distanceFromLastList.Count;

            distanceFromLastList.RemoveAt(0);
        }

        //replace lastPosition withh the current position
        lastPosition = transform.position;

        if (distanceMean < 0.001)
        {
            isPaused = true;
        }
        else
        {
            isPaused = false;
        }
    }


    // vibrate controller for 0.2 seconds
    void ShortVibrateController()
    {
        // make the controller vibrate
        OVRInput.SetControllerVibration(1, 0.6f);

        // stop the vibration after x seconds
        Invoke("StopVibrating", 0.2f);
    }


    void StopVibrating()
    {
        OVRInput.SetControllerVibration(0, 0);
    }

}
