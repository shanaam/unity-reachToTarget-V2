using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UXF;

/*
 * File: ExperimentController.cs
 * License: York University (c) 2019
 * Author: Peter Caruana
 * Desc:    Experiment Controller is what operates the trials of the loaded block. It is responsible for applying settings
 * of the trial to the Unity game at runtime
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
    public PositionLocCursorController positionLocCursorController;

    public GameObject LeftControllerAnchor, RightControllerAnchor;
    

    //-- Internal Variables
    private float timerStart;
    private float timerEnd;
    private float reachTime;
    [SerializeField]
    private OVRInput.Controller m_controller; //Link to the Oculus controller to read button inputs
    
    public OVRInput.Controller GetController() { return m_controller; }

    public void StartTrial() 
    {
        
        homeCursorController.Disappear(); //Hides homeposition at begining of trial
        session.BeginNextTrial();
        
    }

    //Called 
    public void BeginTrialSteps(Trial trial)
    {
        if(trial.settings.GetString("experiment_mode") == "objectToBox")
        {
            ////pseudo randomized block shapes
            //System.Random rando = new System.Random();
            //int flag = rando.Next(100); // randomizing the shape -- this will change depending on the experiment

            //if (flag % 2 == 0) //even number
            //{
            //    trial.settings.SetValue("object_type", "cube");
            //}
            //else
            //{
            //    trial.settings.SetValue("object_type", "sphere");
            //}

            targetContainerController.IsGrabTrial = true;

            if (trial.settings.GetString("type") == "instruction")
            {
                // jsut wait? for a keypress?
                trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = false;
                instructionAcceptor.SetActive(true);
                instructionAcceptorScript.doneInstruction = false;

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

            if (trial.settings.GetString("type") == "localization")
            {
                trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = false;

                UnrenderObject(handCursor);

                if (GameObject.Find("RayPositionCursor") == null)
                {
                    targetContainerController.SpawnTarget(trial);
                }
            }

            else
            {
                if (trial.settings.GetString("type") == "instruction")
                {
                    // jsut wait? for a keypress?
                    trackerHolderObject.GetComponent<PositionRotationTracker>().enabled = false;

                    instructionAcceptor.SetActive(true);
                    instructionAcceptorScript.doneInstruction = false;

                }

                else
                {
                    RenderObject(handCursor);
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
        if (GameObject.Find("RayPositionCursor") != null)
        {
            positionLocCursorController.Deactivate();
        }

        try
        {
            m_controller = session.NextTrial.settings.GetString("hand") == "r" ? 
                OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;

            handCursor.GetComponent<HandCursorController>().handForNextTrial = session.NextTrial.settings.GetString("hand") == "r" ?
                "r" : "l";
        }
        catch (NoSuchTrialException)
        {
            try
            {
                m_controller = session.GetBlock(session.currentBlockNum + 1).firstTrial.settings.GetString("hand") == "r" ? 
                    OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;

                handCursor.GetComponent<HandCursorController>().handForNextTrial = session.GetBlock(session.currentBlockNum + 1).firstTrial.settings.GetString("hand") == "r" ?
                    "r" : "l";

                Debug.Log("Reached end of block");
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Debug.Log("Reached end of experiment.");
            }
        }

        handCursor.GetComponent<HandCursorController>().ChangeHand(
            m_controller == OVRInput.Controller.RTouch ? RightControllerAnchor : LeftControllerAnchor
        );

        homeCursorController.Appear();

        if (session.CurrentTrial.number == session.LastTrial.number)
        {
            session.End();
        }
        else
        {
            session.CurrentTrial.End();
        }
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
        //Debug.LogFormat("Reach Time: {0}", timerEnd - timerStart);
    }

    public void ClearTime()
    {
        timerStart = 0;
        timerEnd = 0;
    }

    public void CalculateReachTime()
    {
        reachTime = timerEnd - timerStart;
    }

    public float GetReachTime()
    {
        return reachTime;
    }
    //Returns vector between A and B
    //Somewhat redundant, however makes code function easier to read
    private Vector3 CalculateVector(Vector3 A, Vector3 B)
    {
        return B - A;
    }

    
    

    public void UnrenderObject(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        rend.enabled = false;
    }

    public void RenderObject(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        rend.enabled = true;
    }

    ////Unused for now but useful
    //IEnumerator WaitAFrame()
    //{
    //    //returning 0 will make it wait 1 frame
    //    yield return 0;
    //}
}
