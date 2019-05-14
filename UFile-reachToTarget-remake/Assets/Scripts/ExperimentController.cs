using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UXF;

/*
 * File: ExperimentController.cs
 * License: York University (c) 2019
 * Author: Peter Caruana
 * Desc:    Experiment Controller is what operates the trials of the loaded block. It is responsible for applying settings of the trial to the Unity game at runtime
 */
public class ExperimentController : MonoBehaviour
{
    public TextMeshPro Instruction; //Text
    public GameObject handCursor;
    public GameObject homeCursor;
    
    // Start is called before the first frame update
    void Start()
    {
     
    }

    public void startTrial(Trial trial)
    {

    }
    //-----------------------------------------------------
    // Modifiers
    private void updateInstruction(string instruction)
    {
        Instruction.text = instruction;
    }
    private void hideInstruction()
    {
        Instruction.gameObject.SetActive(false);
    }
    private void showInstruction()
    {
        Instruction.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
     
    }


}
