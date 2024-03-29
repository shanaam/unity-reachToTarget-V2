﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class GrabableObject : MonoBehaviour
{
    /*
     * GrabableObject.cs
     * Author: Peter Caruana, Mark Voong
     * Vision Labs, YorkU (c) 2019
     * 
     * This script defines functionality for a rigidbody object to be grabed like a realistic object with the vr controller.
     * Objects with this script attached and all of the public references set can be picked up, and even thrown realistically based on
     * the rigidbody mass.
     */

    //References to inGame objects/components must be added before use
    public GameObject handCursor; //reference to the cursor 
    public HandCursorController handCursorController; //controller attached to the cursor
    public Collider collider; //colider of object
    public Rigidbody rigidbody; //rigidbody of the object
    public GameObject home;
    public ExperimentController experimentController;
    //----------------------------------------------------------------

    public bool isKinematic;
    public bool objectGrabbed = false;
    public bool holdUntilZone = true;
    public bool isInBox = false;
    private Vector3 objPosWhenGrabbed;
    private Vector3 handPosWhenGrabbed;

    private Vector3 prevPosition;
    private Vector3 currPosition;
    private Vector3 handVelocity;
    private float timeDelta = 0.01f;
    private float prevTime;
    private float currTime;
    private Stack<Vector3> velocityHistory = new Stack<Vector3>(); //History of the hand cursors velocity. Used to smooth out throwing motions

    private GameObject cursObjTracker;
    public OVRInput.Controller m_controller;
    // Start is called before the first frame update
    void Start()
    {
        //initialize variables and data structures
        handCursor = GameObject.FindGameObjectWithTag("Cursor");
        home = GameObject.FindGameObjectWithTag("Home");
        experimentController = GameObject.FindGameObjectWithTag("ExpCont").GetComponent<ExperimentController>();
        handCursorController = handCursor.GetComponent<HandCursorController>();
        
        prevPosition = handCursor.transform.localPosition;
        currPosition = Vector3.zero;
        prevTime = 0;
        handVelocity = Vector3.zero;
        velocityHistory = new Stack<Vector3>();

        // find the tracker object
        cursObjTracker = GameObject.FindGameObjectWithTag("CursObjTracker");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            if (other.name.Contains("CubeContainer") && name.Contains("Sphere") ||
                other.name.Contains("CylinderContainer") && name.Contains("Cube"))
            {
                isInBox = true;

                // vibrate the controller
                handCursorController.ShortVibrateController();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            isInBox = false;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        OVRInput.Update();
        //Trigger is currently pressed down
        //Oculus unity has weird bug, where button presses arent registered however if you click the home button and go back, then they start working.
        //if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
        //print("" + OVRInput.GetConnectedControllers());


        CalculateVelocity(); //calculates velocity of the hand based on average of previous 6 frames (not based on game frames, but time frames set by the timeDelta)
        
        //Allows to object to be picked up if cursor is touching the colider of the object, trigger is pressed, it isnt grabbed by something else, and the
        //hand isnt already holding some other object.
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, m_controller) && !objectGrabbed & !handCursorController.holdingItem && !isInBox && !handCursorController.isInHomeArea && handCursorController.isPaused)
        {    
            if (collider.bounds.Contains(handCursor.transform.position)) { PickUp(); }
        }
        //Drops the object if the trigger isnt being pressed and the object itself was grabbed previously. This is so all grabable objects do not
        //react when some other object is dropped.
        //!OVRInput.Get(OVRInput.RawButton.RIndexTrigger, m_controller) 
        else if (objectGrabbed)
        {
            if (holdUntilZone)
            {
                //right now, do nothing
                //if (isInBox) { Drop(); } 
            }
            else
            {
                Drop();
            }
        }

        if (handCursorController.movementType.Type == "clamped" & objectGrabbed)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Box");

            // if a target exists
            if (target != null)
            {
                Vector3 targetPosition = target.transform.position;

                //transform.localPosition = realHand.transform.position - transform.parent.transform.position;
                Vector3 centerPos = home.transform.position + (home.transform.forward * 0.1f);

                //project onto a vector pointing toward target
                //transform.localPosition = Vector3.Project(realHandPosition - rotatorObjectPosition, localTargetPosition);

                //project onto a vertical plane intersecting target and home
                Vector3 vectorForPlane = new Vector3(targetPosition.x, targetPosition.y - 1, targetPosition.z);
                Vector3 normalVector = Vector3.Cross(targetPosition - centerPos, vectorForPlane - centerPos);

                //Vector3 translateVector = posWhenGrabbed - projWhenGrabbed;
                Vector3 translateVector = centerPos;
                transform.position = Vector3.ProjectOnPlane(transform.parent.position - centerPos, normalVector) + translateVector;
                transform.position = transform.position + ((objPosWhenGrabbed - centerPos) - (Vector3.ProjectOnPlane(handPosWhenGrabbed - centerPos, normalVector)));

                
            }
        }
    }

    // Only relevent for target trials, where an object is dropped inside of a targetContainerShape. This checks that the object is indeed in the zone.
    //bool isInsideDropZone()
    //{
    //    GameObject[] targetZones = GameObject.FindGameObjectsWithTag("Box");
        
    //    for(int i=0; i < targetZones.Length; i++)
    //    {
    //        Collider tempCol = targetZones[i].GetComponent<Collider>();
    //        if (tempCol.bounds.Contains(handCursor.transform.position)) {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    
    void PickUp()
    {
        handCursorController.holdingItem = true; //semaphore lock for grabable objects
        //Debug.Log(">>  " + gameObject.name + "Trigger Is pressed");
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        rigidbody.isKinematic = true;

        objPosWhenGrabbed = transform.position;
        handPosWhenGrabbed = handCursor.transform.position;

        transform.SetParent(handCursor.transform);

        experimentController.stepTime = Time.time;

        grabed();

        //start tracking object
        cursObjTracker.GetComponent<CursorObjTrackerController>().tracking = true;
        cursObjTracker.GetComponent<PositionRotationTracker>().StartRecording();
    }

    public void Drop()
    {
        handCursorController.holdingItem = false;
        //Debug.Log(">>  " + gameObject.name + " Trigger Released");
        transform.parent = null;
        GetComponent<Rigidbody>().useGravity = true;
        rigidbody.isKinematic = false;
        Vector3 avgVelocity = velocityAverage();

        droped(); //set the objectGrabbed bool to false

        //Average velocity of previous 6 velocity frames for smoothness
        throwObject(avgVelocity * 3); //Multiplied by 3. I have no good answer as to why 3, it simply seems to get the most realistic feel for the force.

    }

    //Calculate hand velocity based on the average velocities of previous frames stored in the velocity stack.
    void CalculateVelocity()
    {
        currTime = Time.fixedTime;
        currPosition = handCursor.transform.position;
        //only pushes another velocity into the velocity history if enough time has elapsed between the last measurement.
        if (currTime - prevTime >= timeDelta)
        {

            velocityHistory.Push((currPosition - prevPosition) / timeDelta);
            prevTime = currTime;
            prevPosition = new Vector3(currPosition.x, currPosition.y, currPosition.z);
        }
    }
    //Calculated the average velocity to smooth out and make throws more consistent
    Vector3 velocityAverage()
    {
        //Averages the top 6 velocity frames in the stack history, or with a TimeDelta of 0.01f, then the average velocity over the last 0.06 seconds
        //Why 6 frames? No idea, it just works. Too little history and it doesnt work, too many and it doesnt work. 6 works. *shrug*
        Vector3 avg = (velocityHistory.Pop() + velocityHistory.Pop() + velocityHistory.Pop() + velocityHistory.Pop() + velocityHistory.Pop() + velocityHistory.Pop()) / 6;
        velocityHistory.Clear();
        return avg;
    }

    void throwObject(Vector3 velocity)
    {
        //Debug.Log("Object Thrown with " + velocity.ToString() + ": " + velocity.magnitude);
        rigidbody.AddForce(velocity, ForceMode.Impulse);
        
    }

    void grabed()
    {
        objectGrabbed = true;

    }

    void droped()
    {
        objectGrabbed = false;
    }
   
}
