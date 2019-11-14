using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class CursorObjTrackerController : MonoBehaviour
{

    public bool tracking = false;
    GameObject trackedObject;

    private void Start()
    {
        gameObject.GetComponent<PositionRotationTracker>().StopRecording();
    }

    void LateUpdate()
    {
        if (tracking)
        {
            trackedObject = GameObject.FindGameObjectWithTag("ExperimentObject");

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
