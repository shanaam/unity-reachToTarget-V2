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
    public GameObject handCursor;
    public bool isLocalization;
    private Vector3 defaultPos = new Vector3(0f, 0.015f, 0.05f);
    void Start()
    {
        isLocalization = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        x = transform.localPosition.x;
        y = transform.localPosition.y;
        z = transform.localPosition.z;

        if (isLocalization)
        {
            if (OVRInput.Get(OVRInput.RawButton.B, m_controller))
            {
                moveCursor(0.0003f);
            }
            else if (OVRInput.Get(OVRInput.RawButton.A, m_controller))
            {
                moveCursor(-0.0003f);
            }
            else if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger, m_controller))
            {
                calculateLocationDelta();
                //expCnt.EndAndPrepare();
                transform.localPosition = defaultPos;
                Debug.Log("Ending Localization Trial T_T bye :'(");
            }
        }

    }

    void moveCursor(float pos)
    {
        transform.localPosition = new Vector3(x, y + pos, z);
    }

    void calculateLocationDelta()
    {
        Vector3 hand = handCursor.transform.position;
        Vector3 cursor = transform.position;
        Vector3 delta = hand - cursor;
        Debug.Log("Hand: " + hand.ToString());
        Debug.Log("Cursor: " + cursor.ToString());
        Debug.Log("Difference Magnitude: " + delta.magnitude);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        isLocalization = true;
    }

    public void renderHand()
    {
        Renderer rend = handCursor.GetComponent<Renderer>();
        rend.enabled = true;
    }
}
