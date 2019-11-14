using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class PlaneController : MonoBehaviour
{
    float plane_rot_angle = -20f;
    Session session;
    public TargetContainerController targetContainerController;


    public void SetTilt(Trial trial)
    {
        if (trial.settings.GetString("plane_setting") == "none")
        {
            SetToNone(trial);
        }
        else if (trial.settings.GetString("plane_setting") == "flat")
        {
            StartCoroutine(SetToFlat(trial));
        }
        else if (trial.settings.GetString("plane_setting") == "tilt")
        {
            StartCoroutine(SetToTiltOnX(trial));
        }

    }


    private void SetToNone(Trial trial)
    {
        MeshRenderer[] rend = gameObject.GetComponentsInChildren<MeshRenderer>();
        rend[0].enabled = false; //there is only one child

        //Create TARGET (give it trial to know the angles and such
        targetContainerController.SpawnTarget(trial);
    }

    private IEnumerator SetToFlat(Trial trial)
    {
        MeshRenderer[] rend = gameObject.GetComponentsInChildren<MeshRenderer>();
        rend[0].enabled = true; //there is only one child


        // get current rotation on the x
        float r_x = transform.eulerAngles.x;
        float r_z = transform.eulerAngles.z;

        Debug.LogFormat("setting to flat. current r_x: {0}", r_x);

        if (r_x > 180)
        {
            // every 0.1s, change the angle to be 1 closer to the desired rotation
            for (float r = r_x; r <= 360f; r += 2f)
            {
                transform.rotation = Quaternion.Euler(r, 0, 0);
                yield return new WaitForSeconds(.02f);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            /*
            for (float r = r_x; r >= 0f; r -= 2f)
            {
                transform.rotation = Quaternion.Euler(r, 0, 0);
                yield return new WaitForSeconds(.02f);
            } */
        }

        //Create TARGET
        targetContainerController.SpawnTarget(trial);
    }

    private IEnumerator SetToTiltOnX(Trial trial)
    {
        MeshRenderer[] rend = gameObject.GetComponentsInChildren<MeshRenderer>();
        rend[0].enabled = true; //there is only one child

        // get current rotation on the x
        float r_x = transform.eulerAngles.x;
        float r_z = transform.eulerAngles.z;

        // every 0.1s, change the angle to be 1 closer to the desired rotation
        for (float r = r_x; r >= plane_rot_angle; r -= 2f)
        {
            transform.rotation = Quaternion.Euler(r, 0, 0);
            yield return new WaitForSeconds(.02f);
        }
        //Create TARGET
        targetContainerController.SpawnTarget(trial);
    }

    /*
    public IEnumerator SetToTiltOnZ()
    {
        GetComponent<MeshRenderer>().enabled = true;

        // get current rotation on the y
        float r_z = transform.eulerAngles.z;

        // every 0.1s, change the angle to be 1 closer to the desired rotation
        for (float r = r_z; r > plane_rot_angle; r -= 2f)
        {

            transform.rotation = Quaternion.Euler(0, 0, r);
            yield return new WaitForSeconds(.02f);
        }

        // instantiate target on the first trial of each block
        if (experimentController.trialInBlock == 1 && experimentController.isInstructionTrial == false)
        {
            //Create TARGET
            targetContainerController.InstantiateTarget();
        }
    }
    */
}
