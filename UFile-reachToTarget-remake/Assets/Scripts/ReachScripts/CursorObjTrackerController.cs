using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class CursorObjTrackerController : MonoBehaviour
{

    public bool tracking = false;
    GameObject trackedObject;
    public Session session;

    private void Start()
    {
        gameObject.GetComponent<PositionRotationTracker>().StopRecording();
    }

    void LateUpdate()
    {
        if (tracking)
        {
            if (session.CurrentTrial.settings.GetString("experiment_mode") != "target")
            {
                trackedObject = GameObject.FindGameObjectWithTag("ExperimentObject");
            }
            else
            {
                trackedObject = GameObject.FindGameObjectWithTag("Cursor");
            }

            transform.position = trackedObject.transform.position;
            transform.rotation = trackedObject.transform.rotation;

        }

        else
        {

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
    }
}
