using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCursorController : MonoBehaviour
{
    // Start is called before the first frame update
    private float x;
    private float y;
    private float z;
    [SerializeField]
    private OVRInput.Controller m_controller;
    public ExperimentController expCnt;
    private Vector3 defaultPos = new Vector3(0f, 0.015f, 0.05f);
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        x = transform.localPosition.x;
        y = transform.localPosition.y;
        z = transform.localPosition.z;
        

        if (OVRInput.Get(OVRInput.RawButton.B, m_controller))
        {
            moveCursor(0.0003f);
        }
        else if (OVRInput.Get(OVRInput.RawButton.A, m_controller))
        {
            moveCursor(-0.0003f);
        }else if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger, m_controller))
        {
            //expCnt.EndAndPrepare();
            //transform.localPosition = defaultPos;
            //Debug.Log("Ending Localization Trial T_T bye :'(");
        }
    }

    void moveCursor(float pos)
    {
        transform.localPosition = new Vector3(x, y + pos, z);
    }
}
