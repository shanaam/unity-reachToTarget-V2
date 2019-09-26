using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class HandRenderController : MonoBehaviour
{
    public Session session;
    public HandCursorController handCursorController;

    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        try
        {
            RenderWhenNeeded(session.CurrentTrial);

            if (handCursorController.holdingItem | handCursorController.taskCompleted)
            {
                rend.enabled = false;
            }
        }

        catch
        {

        }

    }

    private void RenderWhenNeeded (Trial trial)
    {

        string expMode = trial.settings.GetString("experiment_mode");
        string hand = trial.settings.GetString("hand");
        string name = gameObject.name;


        if (expMode == "objectToBox" & (name == "hands:Rhand" ? hand == "r" : hand == "l"))
        {
            handCursorController.visible = false;
            rend.enabled = true;
        }
        else
        {
            rend.enabled = false;
        }

    }
}
