using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCursorController : MonoBehaviour
{
    //link to the actual hand position object
    public GameObject realHand;

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
}
