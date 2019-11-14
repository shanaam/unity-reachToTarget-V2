using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class InstructionAcceptor : MonoBehaviour
{

    public bool doneInstruction = false;
    public ExperimentController experimentController;
    public HandCursorController handCursorController;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            Debug.Log("instruction acceptor: running end and prepare");

            doneInstruction = true;

            experimentController.EndAndPrepare();
            handCursorController.taskCompleted = true;

            gameObject.SetActive(false);
        }
    }
}
