using UnityEngine;
using UXF;

public class TargetController : MonoBehaviour
{
    private GameObject handCursor;
    private GameObject cursObjTracker;
    private ExperimentController experimentController;
    private HandCursorController handCursorController;
    private TargetContainerController targetContainerController;

    // True when this object is used as a starting point for reach tasks
    public bool IsSecondaryHome = false;
    public GameObject RealTarget = null;
    public Vector3 SecondaryHomePos;

    private const float MIN_DIST = 0.05f;

    void Awake()
    {
        // get the inputs we need for displaying the cursor
        handCursor = GameObject.FindGameObjectWithTag("Cursor");
        cursObjTracker = GameObject.FindGameObjectWithTag("CursObjTracker");
        handCursorController = handCursor.GetComponent<HandCursorController>();
        experimentController = handCursorController.experimentController;
        targetContainerController = experimentController.targetContainerController;   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 centreExpPosition = experimentController.homeCursorController.transform.position;

        bool visible = handCursorController.visible;
        bool isPaused = handCursorController.isPaused;
        bool isInHomeArea = handCursorController.isInHomeArea;
        bool taskCompleted = handCursorController.taskCompleted;
        bool isInTarget = handCursorController.isInTarget;

        float actDist = (
            handCursor.transform.position -
            (!IsSecondaryHome ? SecondaryHomePos : centreExpPosition)
        ).magnitude;

        bool distThreshold = actDist >= MIN_DIST; //Distance cursor has moved from home position
                                                  //Debug.Log("Actual Distance from center: " + actDist);

        //Do things when this thing is in the target (and paused), or far enough away during nocusor
        if (isPaused && !isInHomeArea)
        {
            // When IsSecondaryHome is false, target acts as a regular reach target.
            // If true, the participant must touch this "target" to spawn the real one.
            if (!IsSecondaryHome && ((!taskCompleted && distThreshold && !visible) ^ isInTarget))
            {
                // Above only checks if its paused (for case of noCursor), needs to also check for some minimum time or distance travelled etc.
                //End and prepare
                experimentController.PauseTimer();

                experimentController.CalculateReachTime();

                if (experimentController.GetReachTime() < 1f)
                {
                    targetContainerController.soundActive = true;
                }
                else
                {
                    targetContainerController.soundActive = false;

                }

                //
                cursObjTracker.GetComponent<CursorObjTrackerController>().tracking = false;
                cursObjTracker.GetComponent<PositionRotationTracker>().StopRecording();

                experimentController.EndAndPrepare();

                handCursorController.taskCompleted = true;
                handCursorController.isInTarget = false;
            }
            else if (isInTarget)
            {
                if (RealTarget != null)
                {
                    // Fix hand cursor
                    handCursorController.isInTarget = false;

                    // Hide cursor if the type is nocursor
                    if (experimentController.session.CurrentTrial.settings.GetString("type") == "nocursor" ||
                        experimentController.session.CurrentTrial.settings.GetString("type") == "localization")
                    {
                        handCursorController.SetCursorVisibility(false);
                    }

                    handCursorController.ResetPause();

                    // Set the correct movement type for this trial
                    //handCursorController.SetMovementType(experimentController.session.CurrentTrial);

                    // set step time
                    experimentController.stepTime = Time.time;

                    cursObjTracker.GetComponent<CursorObjTrackerController>().tracking = true;
                    cursObjTracker.GetComponent<PositionRotationTracker>().StartRecording();

                    // Re-enable the real target and disable this one
                    RealTarget.SetActive(true);
                    Destroy(gameObject);


                }
                else { Debug.LogWarning("This isn't supposed to happen"); }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (handCursor != null)
        {
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(!IsSecondaryHome ? SecondaryHomePos : 
                experimentController.homeCursorController.transform.position, handCursor.transform.position);
        }
    }
}
