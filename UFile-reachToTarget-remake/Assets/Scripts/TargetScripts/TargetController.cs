using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject handCursor;
    private ExperimentController experimentController;
    private HandCursorController handCursorController;
    private TargetContainerController targetContainerController;

    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        // get the inputs we need for displaying the cursor
        handCursor = GameObject.FindGameObjectWithTag("Cursor");
        handCursorController = handCursor.GetComponent<HandCursorController>();
        experimentController= handCursorController.experimentController;
        targetContainerController = GameObject.FindGameObjectWithTag("TargetContainer").GetComponent<TargetContainerController>();

        Vector3 centreExpPosition = GameObject.FindGameObjectWithTag("Home").transform.parent.transform.position;

        bool visible = handCursorController.visible;
        bool isPaused = handCursorController.isPaused;
        bool isInHomeArea = handCursorController.isInHomeArea;
        bool taskCompleted = handCursorController.taskCompleted;
        bool isInTarget = handCursorController.isInTarget;
        

        float minDist = 0.1f;
        float actDist = (handCursor.transform.localPosition - centreExpPosition).magnitude;
        bool distThreshold = actDist >= minDist; //Distance cursor has moved from home position
        //Debug.Log("Actual Distance from center: " + actDist);
        //Do things when this thing is in the target (and paused), or far enough away during nocusor
        if (isPaused && !isInHomeArea && ((!taskCompleted && distThreshold && !visible) ^ isInTarget)) //^ is exclusive OR
        {
            // Above only checks if its paused (for case of noCursor), needs to also check for some minimum time or distance travelled etc.
            //End and prepare
            experimentController.PauseTimer();

            experimentController.CalculateReachTime();

            if (experimentController.GetReachTime() < 1.5f)
            {
                targetContainerController.soundActive = true;
            }
            else
            {
                targetContainerController.soundActive = false;

            }

            experimentController.EndAndPrepare();

            handCursorController.taskCompleted = true;
            handCursorController.isInTarget = false;
        }

        
    }
}
