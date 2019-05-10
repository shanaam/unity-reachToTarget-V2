using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File: MovementType.cs
 * License: York University (c) 2019
 * Author: Peter Caruana
 * Desc: Defines the movement of an object with respect to transformations. Abstract class defines the general interface of a movement type
 * Content:
 *      abstract class MovementType
 *          ||
 *          |--- class ClampedMovement
 *          |
 *          --- class MappedMovement
 */
abstract public class MovementType : MonoBehaviour
{
    /*
     * Returns new vector according to the transformType  based on the real transformation
     */
    abstract public Vector3 transformMotion(Vector3 realPosition, Vector3 startPosition); //Returns new position vector based on movementType

    abstract public string getType();

}

public class ClampedMovement : MovementType
{
    // clamped movement type, gives a mapping of the transformation based on the input clamped to 1 plane of movement.
    //Constructor
    public ClampedMovement()
    {

    }
    //Interface Methods
    public override Vector3 transformMotion(Vector3 realPosition, Vector3 startPosition)
    {
        //todo: Implement clamped transformation
        return new Vector3(0, 0, 0);
    }

    public override string getType()
    {
        return "clamped";
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

   
}

public class MappedMovement: MovementType
{
    // Mapped movement will give a 1-1 mapping of input position to output position.
    //Constructor
    public MappedMovement()
    {

    }
    //Interface Methods
    public override Vector3 transformMotion(Vector3 realPosition, Vector3 startPosition)
    {
        //todo: Implement mapped transformation
        return startPosition + realPosition;
    }

    public override string getType()
    {
        return "mapped";
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
