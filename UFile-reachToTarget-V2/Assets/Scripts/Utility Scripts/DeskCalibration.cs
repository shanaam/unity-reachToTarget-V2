using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DeskCalibration : MonoBehaviour
{
    /*
    * Author: Peter Caruana
    * Vision Labs, YorkU (c) 2019
    * 
    * This script is to calibrate a virtual desk in Unity game-space with a real life desk to help with VR immersion.
    * When attached to an object and active in the scene (preferably some immutable object in the scene such as an experiment controller object)
    * It will prompt to calibrate the in game desk, and will save that calibration. If a file exists already containing the calibration, it simply reads
    * that.
    * 
    * The desk requires 2 anchor objects, one in each corner. To calibrate, the user must press (A) in the same places touching the real life desk.
    * After placing 2 spheres, the script will calculate the translation + rotation required to fit the virtual desk in accordance with the calibration
    * spheres
    */
    public GameObject handCursor;
    public GameObject deskAnchorA;
    public GameObject deskAnchorB;
    public GameObject calibrationSphere1;
    public GameObject calibrationSphere2;
    public GameObject desk;

    [SerializeField]
    private OVRInput.Controller m_controller;
    //variables for calibration
    private int press = 0;
    private bool buttonLock = true;
    private string m_path;
    private bool calibrated;
    // Start is called before the first frame update
    void Start()
    {
        //Loads calibration of desk position/rotation. If file exists, calibrates. If not, calibration must be completed
        m_path = Application.dataPath;
        Debug.Log(m_path + " is Active dataPath");

        string filename = m_path + "/Files/deskCalibration.txt";

        if (File.Exists(filename))
        {
            Debug.Log(filename + " exists, calibrating desk");
            string rawText = File.ReadAllText(filename);
            //Format of '.vec' is vector x y z x y z
            string[] vector = rawText.Split(',');

            deskAnchorA.transform.position = new Vector3(float.Parse(vector[0]), float.Parse(vector[1]), float.Parse(vector[2]));
            deskAnchorA.transform.localRotation = Quaternion.Euler(float.Parse(vector[3]), float.Parse(vector[4]), float.Parse(vector[5]));

            UnrenderObject(deskAnchorA);
            UnrenderObject(deskAnchorB);
            UnrenderObject(calibrationSphere1);
            UnrenderObject(calibrationSphere2);

            calibrated = true;
        }
        else
        {
            calibrated = false;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        string filename = m_path + "/Files/deskCalibration.txt";
        if (!calibrated)
        {
            Debug.Log(filename + " does not exist, creating new calibration. Please continue with desk calibration");
            if (buttonLock && OVRInput.Get(OVRInput.RawButton.A, m_controller)) //button is pressed once, button lock must be reset (by releasing button). This is to prevent unity from instantly going through all of the cases
            {
                Vector3 handlocation = handCursor.transform.position;
                if (press == 0)
                {
                    Debug.Log("Calibration1: " + handlocation.ToString());
                    press++;
                    calibrationSphere1.transform.position = handlocation;
                }
                else if (press == 1)
                {
                    Debug.Log("Calibration2: " + handlocation.ToString());
                    press++;
                    calibrationSphere2.transform.position = handlocation;
                }
                else if (press == 2)
                {
                    press++;
                    calibrateDesk(filename);
                    Debug.Log("Desk Calibration Saved");
                    calibrated = true;
                }
            }

            if (OVRInput.Get(OVRInput.RawButton.A, m_controller))
            {
                buttonLock = false;
            }
            else
            {
                buttonLock = true;
            }

        }
    }
    private Vector3 calculateVector(Vector3 A, Vector3 B)
    {
        return B - A;
    }

    private void calibrateDesk(string filename)
    {
        Vector3 v = calculateVector(calibrationSphere1.transform.position, calibrationSphere2.transform.position);
        Vector3 AB = calculateVector(deskAnchorA.transform.position, deskAnchorB.transform.position);
        float vAB = Vector3.Dot(v, AB);
        float scalarProj = vAB / (AB.magnitude * v.magnitude);
        float theta = Mathf.Acos(scalarProj);
        float PI = 3.14159f;
        float degrees = theta * 180 / PI;
        deskAnchorA.transform.localRotation = Quaternion.Euler(0, degrees, 0);
        deskAnchorA.transform.position = calibrationSphere1.transform.position;

        UnrenderObject(deskAnchorA);
        UnrenderObject(deskAnchorB);
        UnrenderObject(calibrationSphere1);
        UnrenderObject(calibrationSphere2);

        System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true);

        string line = deskAnchorA.transform.position.x.ToString() + ',' + 
            deskAnchorA.transform.position.y.ToString() + ',' + 
            deskAnchorA.transform.position.z.ToString() + ',' + 
            deskAnchorA.transform.rotation.eulerAngles.x.ToString() + ',' + 
            deskAnchorA.transform.rotation.eulerAngles.y.ToString() + ',' + 
            deskAnchorA.transform.rotation.eulerAngles.z.ToString();

        Debug.Log("Data: " + line);
        file.WriteLine(line);

        file.Close();
    }

    public void UnrenderObject(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        rend.enabled = false;
    }
}
