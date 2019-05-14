using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File: CursorMovementType.cs
 * License: York University (c) 2019
 * Author: Peter Caruana
 * Desc: Defines the movement of an object with respect to transformations. Abstract class defines the general interface of a movement type
 * Content:
 *      abstract class CursorMovementType
 *          ||
 *          |--- class ClampedHandCursor
 *          |
 *          --- class AlignedHandCurosor
 */
abstract public class CursorMovementType : MonoBehaviour
{
    /*
     * Returns new vector according to the transformType  based on the real transformation
     */
    abstract public Vector3 NewCursorPosition(Vector3 realPosition, Vector3 centreExpPosition); //Returns new position vector based on movementType

    abstract public string Type { get; }
}

public class AlignedHandCurosor : CursorMovementType
{
    // Mapped movement will give a 1-1 mapping of input position to output position.
    //Constructor
    public AlignedHandCurosor()
    {

    }


    //Interface Methods
    public override Vector3 NewCursorPosition(Vector3 realPosition, Vector3 centreExpPosition)
    {
        //todo: Implement mapped transformation
        return realPosition - centreExpPosition;
    }

    public override string Type => "aligned";


    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }
}

public class ClampedHandCursor : CursorMovementType
{
    // clamped movement type, gives a mapping of the transformation based on the input clamped to 1 plane of movement.
    //Constructor
    public ClampedHandCursor()
    {

    }


    //Interface Methods
    public override Vector3 NewCursorPosition(Vector3 realPosition, Vector3 centreExpPosition)
    {
        //todo: Implement clamped transformation
        return new Vector3(0, 0, 0);
    }


    public override string Type => "clamped";


    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }

   
}

