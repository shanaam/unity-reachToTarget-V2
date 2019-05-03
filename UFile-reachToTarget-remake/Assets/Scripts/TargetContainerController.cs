﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File:    TargetController.cs
 * Project: ReachToTarget-Remake
 * Author:  Peter Caruana
 * York University (c) 2019 
 * Vision Research Labs
 */


public class TargetContainerController : MonoBehaviour
{
    public GameObject targetPrefab;
    public float targetRadius;
    
    // Start is called before the first frame update
    void Start()
    {
        //Until Game Start is implemented
        gameObject.SetActive(true);
        var target = Instantiate(targetPrefab, transform);
        target.transform.localPosition = new Vector3(0, 0, targetRadius);
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void Destroy()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        gameObject.SetActive(false);
        for (var i = 0; i < targets.Length; i++)
        {
            Destroy(targets[i]);
        }
    }
}