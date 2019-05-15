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
    public TargetContainerController targetContainer;
    public HomeCursorController homeCursorController;


    public void StartTrial()
    {
        //Debug.LogFormat("starting trial {0}.", session.NextTrial.number);
        homeCursorController.Remove();
        session.BeginNextTrial();

        //Debug.LogFormat("started trial {0}.", session.CurrentTrial.number);
    }

    // end session or begin next trial (This should ideally be called via event system)
    // destroys the the current target and starts next trial
    public void EndAndPrepare()
    {
        //Debug.Log("ending reach trial...");
        // destroy the target, spawn home?
        targetContainer.DestroyTargets();
        homeCursorController.Appear();

        session.CurrentTrial.End();

        if (session.CurrentTrial == session.LastTrial)
        {
            session.End();
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

    ////Unused for now but useful
    //IEnumerator WaitAFrame()
    //{
    //    //returning 0 will make it wait 1 frame
    //    yield return 0;
    //}
}
