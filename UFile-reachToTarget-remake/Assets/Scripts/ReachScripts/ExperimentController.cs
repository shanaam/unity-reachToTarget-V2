using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UXF;

/*
 * File: ExperimentController.cs
 * License: York University (c) 2019
 * Author: Peter Caruana
 * Desc:    Experiment Controller is what operates the trials of the loaded block. It is responsible for applying settings of the trial to the Unity game at runtime
 */
public class ExperimentController : MonoBehaviour
{
    public Session session;
    public TextMeshPro Instruction; //Text
    public GameObject handCursor;
    public GameObject homeCursor;
    public TargetContainerController targetContainerController;
    public HomeCursorController homeCursorController;
    public GameObject trackerHolderObject;
    public PlaneController planeController;
    public GameObject instructionAcceptor;
    public InstructionAcceptor instructionAcceptorScript;
    public GameObject deskAnchorA;
    public GameObject deskAnchorB;
    public GameObject calibrationSphere1;
    public GameObject calibrationSphere2;

    //-- Internal Variables
    private float timerStart;
    private float timerEnd;
    private float reachTime;
    [SerializeField]
    private OVRInput.Controller m_controller;
    //variables for calibration
    private int press = 0;
    private bool buttonLock = true;
    
    public void Start()
    {
        
    }

    public void Update()
    {
        //Calibration of the desk with the real environment


        if (buttonLock && OVRInput.Get(OVRInput.RawButton.A, m_controller)) //button is pressed once, button lock must be reset (by releasing button). This is to prevent unity from instantly going through all of the cases
        {
            Vector3 handlocation = handCursor.transform.position;
            if (press == 0)
            {
                Debug.Log("Calibration1: " + handlocation.ToString());
                press++;
                calibrationSphere1.transform.position = handlocation;
            }
            else if (press == 1)
            {
                Debug.Log("Calibration2: " + handlocation.ToString());
                press++;
                calibrationSphere2.transform.position = handlocation;
            }
            else if (press == 2)
            {
                press++;
                calibrateDesk();
            }
        }

        if (OVRInput.Get(OVRInput.RawButton.A, m_controller))
        {
            buttonLock = false;
        }
        else
        {
            buttonLock = true;
        }
    }

    

    public void StartTrial() //run when cursor is in home and some booleans are right
    {
        //Debug.LogFormat("starting trial {0}.", session.NextTrial.number);
        homeCursorController.Remove();

        ////enable the tracker script (to track the reach)
        //trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = true;

        session.BeginNextTrial();

        //Debug.LogFormat("started trial {0}.", session.CurrentTrial.number);
    }

    public void BeginTrialSteps(Trial trial)
    {
        if(trial.settings.GetString("experiment_mode") == "objectToBox")
        {
            //pseudo randomized block shapes
            System.Random rando = new System.Random();
            int flag = rando.Next(100);
            if (flag % 2 == 0) //even number
            {
                trial.settings.SetValue("object_type", "cube");
            }
            else
            {
                trial.settings.SetValue("object_type", "sphere");
            }

            targetContainerController.IsGrabTrial = true;

            if (trial.settings.GetString("type") == "instruction")
            {
                // jsut wait? for a keypress?
                trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = false;
                instructionAcceptor.SetActive(true);
                instructionAcceptorScript.doneInstruction = false;

            }
            else if (trial.settings.GetString("type") == "localization")
            {
                //todo implement localization tests
            }
            else
            {

                trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = true;

                // do the plane thing at the start of each block 
                if (trial.numberInBlock == 1)
                {
                    planeController.SetTilt(trial);
                }
                else
                {
                    targetContainerController.SpawnTarget(trial);
                }
            }
        }
        else if(trial.settings.GetString("experiment_mode") == "target")
        {


            targetContainerController.IsGrabTrial = false;

            if (trial.settings.GetString("type") == "instruction")
            {
                // jsut wait? for a keypress?
                trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = false;
                instructionAcceptor.SetActive(true);
                instructionAcceptorScript.doneInstruction = false;

            }
            else if (trial.settings.GetString("type") == "localization")
            {
                //todo implement localization tests
            }
            else
            {

                trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = true;

                // do the plane thing at the start of each block 
                if (trial.numberInBlock == 1)
                {
                    planeController.SetTilt(trial);
                }
                else
                {
                    targetContainerController.SpawnTarget(trial);
                }
            }
        }
        

    }
    // end session or begin next trial (This should ideally be called via event system)
    // destroys the the current target and starts next trial
    public void EndAndPrepare()
    {

        session.CurrentTrial.result["home_x"] = homeCursor.transform.position.x;
        session.CurrentTrial.result["home_y"] = homeCursor.transform.position.y;
        session.CurrentTrial.result["home_z"] = homeCursor.transform.position.z;

        //Debug.Log("ending reach trial...");
        // destroy the target, spawn home?
        targetContainerController.DestroyTargets();
        homeCursorController.Appear();

        if (session.CurrentTrial.number == session.LastTrial.number)
        {
            session.End();
        }

        else
        {
        session.CurrentTrial.End();
        }

        /*
        else
        {
            session.BeginNextTrial();
        }
        */
    }

    //-----------------------------------------------------
    // Modifiers
    private void UpdateInstruction(string instruction)
    {
        Instruction.text = instruction;
    }
    private void HideInstruction()
    {
        Instruction.gameObject.SetActive(false);
    }
    private void ShowInstruction()
    {
        Instruction.gameObject.SetActive(true);
    }

    //Start timer when home Disapears, End when target disapears
    public void StartTimer()
    {
        ClearTime();
        timerStart = Time.fixedTime;
        //Debug.Log("Timer started : " + timerStart);
    }

    public void PauseTimer()
    {
        timerEnd = Time.fixedTime;
        //Debug.Log("Timer end : " + timerEnd);
        Debug.LogFormat("Reach Time: {0}", timerEnd - timerStart);
    }

    public void ClearTime()
    {
        timerStart = 0;
        timerEnd = 0;
    }

    public void calculateReachTime()
    {
        reachTime = timerEnd - timerStart;
    }

    public float getReachTime()
    {
        return reachTime;
    }

    private Vector3 calculateVector(Vector3 A, Vector3 B)
    {
        return B - A;
    }

    /*
     * Aligns the desk with the calibration spheres
     *  
     *  v->     
     *   ^     AB->
     *   | th /
     *   |---/
     *   |  /
     *   | /  
     *   |/
     *   
     *   
     */
    private void calibrateDesk()
    {
        Vector3 v = calculateVector(calibrationSphere1.transform.position, calibrationSphere2.transform.position);
        Vector3 AB = calculateVector(deskAnchorA.transform.position, deskAnchorB.transform.position);
        float vAB = Vector3.Dot(v, AB);
        float scalarProj = vAB / (AB.magnitude * v.magnitude);
        float theta = Mathf.Acos(scalarProj);
        float PI = 3.14159f;
        float degrees = theta * 180 / PI;
        deskAnchorA.transform.localRotation = Quaternion.Euler(0, degrees, 0);
        deskAnchorA.transform.position = calibrationSphere1.transform.position;

        UnrenderObject(deskAnchorA);
        UnrenderObject(deskAnchorB);
        UnrenderObject(calibrationSphere1);
        UnrenderObject(calibrationSphere2);


    }

    public void UnrenderObject(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        rend.enabled = false;
    }

    ////Unused for now but useful
    //IEnumerator WaitAFrame()
    //{
    //    //returning 0 will make it wait 1 frame
    //    yield return 0;
    //}
}
