using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLocCursorController : MonoBehaviour
{
    // Start is called before the first frame update
    private float x;
    private float y;
    private float z;

    [SerializeField]
    private OVRInput.Controller m_controller;

    public ExperimentController experimentController;
    public HandCursorController handCursorController;
    public GameObject handCursor;
    public GameObject expCentre;
    public GameObject centreEye;
    public bool isLocalization;
    private Vector3 defaultPos = new Vector3(0f, 0.05f, 0.05f);


    void Start()
    {
        Deactivate();
    }

    private void Awake()
    {
        transform.localPosition = defaultPos;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition = defaultPos;

        //Calculate forward vector
        Vector3 forward = centreEye.transform.position - transform.parent.position;
        Ray ray = new Ray(transform.parent.position, forward);

        //Plane for intersecting

        Plane planeTOIntersect = new Plane(Vector3.down, expCentre.transform.position.y);

        //Initialise the enter variable
        float enter = 0.0f;

        if (planeTOIntersect.Raycast(ray, out enter))
        {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);

            //Move your cube GameObject to the point where you clicked
            transform.position = hitPoint;
        }

        x = transform.localPosition.x;
        y = transform.localPosition.y;
        z = transform.localPosition.z;

        if (isLocalization)
        {

            if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger, m_controller))
            {
                CalculateLocationDelta();

                Debug.Log("Ending Localization Trial T_T bye :'(");

                experimentController.EndAndPrepare();
                handCursorController.taskCompleted = true;
                isLocalization = false;
            }

            //else if (OVRInput.Get(OVRInput.RawButton.B, m_controller))
            //{
            //    MoveCursor(0.0003f);
            //}
            //else if (OVRInput.Get(OVRInput.RawButton.A, m_controller))
            //{
            //    MoveCursor(-0.0003f);
            //}
        }
    }

    void MoveCursor(float pos)
    {
        transform.localPosition = new Vector3(x, y + pos, z);
    }

    void CalculateLocationDelta()
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

    public void Deactivate()
    {
        isLocalization = false;
        gameObject.SetActive(false);
    }

    public void Appear()
    {
        Renderer rend = handCursor.GetComponent<Renderer>();
        rend.enabled = true;
    }
}
