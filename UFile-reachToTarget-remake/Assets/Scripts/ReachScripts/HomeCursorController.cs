using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class HomeCursorController : MonoBehaviour
{
    public bool visible = true;
    public Session session; // the current session 
    public HandCursorController handCursorController;
    public ExperimentController experimentController;
    public TargetContainerController targetContainerController;



    private void LateUpdate()
    {
        bool isInHome = handCursorController.isInHome;
        bool isPaused = handCursorController.isPaused;
        bool taskCompleted = handCursorController.taskCompleted;
        // ^ for readability
        if (isInHome && isPaused && taskCompleted)
        {
            handCursorController.taskCompleted = false;
            targetContainerController.DestroyBoxes(); //Eliminates previous boxes for ObjectGrab trials
            experimentController.StartTimer();
            experimentController.StartTrial();
        }
    }

    public void Appear()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.enabled = true;
        visible = true;
    }

    public void hideCursor()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.enabled = false;
        visible = false;
    }
}
