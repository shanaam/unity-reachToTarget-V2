using System.Collections.Generic;
using UnityEngine;
using UXF;

/*
 * File: ArcController.cs
 * Project: ReachToTarget-Remake
 * Author: Mark Voong
 * York University (c) 2019
 * Vision Research Labs
 */

public class ArcController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject handCursor;
    private ExperimentController experimentController;
    private HandCursorController handCursorController;
    private TargetContainerController targetContainerController;

    // True when this object is used as a starting point for reach tasks
    public bool IsSecondaryHome = false;
    public GameObject RealTarget = null;
    public Vector3 SecondaryHomePos;

    public MeshFilter meshFilter;

    // The width of the arc
    public float width;
    private float prevWidth;

    // How tall the arc is
    public float thickness;
    private float prevThickness;

    // The span of the arc in degrees
    public int arcSpan;
    private float prevSpan;

    // Animation variables for expanding the arc to 180 degrees
    private float lerpTimer;
    private float lerpRate = 0.75f;
    public bool useExpand;
    private bool expand;
    private float oldArc;

    private float targetDistance;

    private const float MIN_DIST = 0.05f;

    private void Awake()
    {
        targetDistance = 0.1f;
        oldArc = arcSpan;
        prevWidth = width;
        prevSpan = arcSpan;
        prevThickness = thickness;
        GenerateArc();

        // get the inputs we need for displaying the cursor
        handCursor = GameObject.FindGameObjectWithTag("Cursor");
        handCursorController = handCursor.GetComponent<HandCursorController>();
        experimentController = handCursorController.experimentController;
        targetContainerController = experimentController.targetContainerController;
    }

    /// <summary>
    /// Draws a 3D arc
    /// </summary>
    private void GenerateArc()
    {
        Mesh mesh = new Mesh();

        // Scale is 100.0f which converts editor units from cm to m. Then it is halved.
        float halfWidth = width / 200.0f;
        float halfThickness = thickness / 2.0f;

        // The increment for each vector along the arc
        float delta = Mathf.Deg2Rad * arcSpan / arcSpan;

        // Generates 2 sets of vertices along an arc (One larger, one smaller)
        // making the shape of a circle
        float angle = 0.0f;
        Vector3[] vertices = new Vector3[(arcSpan * 4) + 4];
        for (int i = 0; i < vertices.Length; i += 4)
        {
            float outerX = Mathf.Sin(angle) * (targetDistance + halfWidth);
            float outerY = Mathf.Cos(angle) * (targetDistance + halfWidth);
            float innerX = Mathf.Sin(angle) * (targetDistance - halfWidth);
            float innerY = Mathf.Cos(angle) * (targetDistance - halfWidth);

            vertices[i] = new Vector3(outerX, outerY, -halfThickness);
            vertices[i + 1] = new Vector3(innerX, innerY, -halfThickness);
            vertices[i + 2] = new Vector3(outerX, outerY, halfThickness);
            vertices[i + 3] = new Vector3(innerX, innerY, halfThickness);

            angle += delta;
        }

        List<int> triangles = new List<int>();

        // Generates quads for each face of the 3D arc
        for (int i = 0; i < 4 * arcSpan; i += 4)
        {
            // Top Face
            triangles.Add(i);
            triangles.Add(i + 4);
            triangles.Add(i + 1);

            triangles.Add(i + 1);
            triangles.Add(i + 4);
            triangles.Add(i + 5);

            // Bottom Face
            triangles.Add(i + 2);
            triangles.Add(i + 3);
            triangles.Add(i + 6);

            triangles.Add(i + 3);
            triangles.Add(i + 7);
            triangles.Add(i + 6);

            // Outside Face
            triangles.Add(i);
            triangles.Add(i + 2);
            triangles.Add(i + 4);

            triangles.Add(i + 2);
            triangles.Add(i + 6);
            triangles.Add(i + 4);

            // Inside Face
            triangles.Add(i + 1);
            triangles.Add(i + 5);
            triangles.Add(i + 3);

            triangles.Add(i + 5);
            triangles.Add(i + 7);
            triangles.Add(i + 3);
        }

        // End Pieces
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(2);

        triangles.Add(vertices.Length - 1);
        triangles.Add(vertices.Length - 3);
        triangles.Add(vertices.Length - 2);

        triangles.Add(vertices.Length - 2);
        triangles.Add(vertices.Length - 3);
        triangles.Add(vertices.Length - 4);


        // Assign mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshFilter.mesh = mesh;

        #region 2D_ARC
        /*
        // Create the top half of the arc
        // This arc is at the target distance + half the width, front of the target
        float angle = startingAngle;
        for (int i = 0; i <= arcSpan; i++)
        {
            float x = Mathf.Sin(angle) * (targetDistance + halfWidth);
            float y = Mathf.Cos(angle) * (targetDistance + halfWidth);

            angle -= delta;
            vertices.Add(new Vector2(x, y));
        }
        
        
        // Create bottom half of arc
        // This arc is at the target distance - half the width, behind the target
        angle = 0.0f;
        for (int i = 0; i <= arcSpan; i++)
        {
            float x = Mathf.Sin(angle) * (targetDistance - halfWidth);
            float y = Mathf.Cos(angle) * (targetDistance - halfWidth);

            angle += delta;
            vertices.Add(new Vector2(x, y));
        }

        // Generates triangles for the mesh to render
        Triangulator tri = new Triangulator(vertices.ToArray());
        int[] indices = tri.Triangulate();

        // Convert 2D vectors to 3D coordinate space
        Vector3[] meshVertices = new Vector3[vertices.Count];
        for (int i = 0; i < meshVertices.Length; i++)
        {
            meshVertices[i] = new Vector3(vertices[i].x, vertices[i].y, 0.0f);
        }

        // Assign vertices and triangles to mesh object
        mesh.vertices = meshVertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        */
        #endregion 2D_ARC

        // NOTE: REMEMBER TO REMOVE NEGATIVE WHEN ANGLE BUG IS FIXED
        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            -(arcSpan / 2.0f),
            0.0f
        );
    }

    /// <summary>
    /// Generates an arc at at the target angle which spans n degrees.
    /// </summary>
    /// <param name="trial"></param>
    public void GenerateArc(Trial trial)
    {
        // Convert target distance from cm to m
        targetDistance = trial.settings.GetFloat("target_distance") / 100.0f;
        GenerateArc();
    }

    private void Update()
    {
        // Debugging for testing size and shape
        if (prevWidth != width || prevSpan != arcSpan || prevThickness != thickness)
        {
            prevWidth = width;
            prevSpan = arcSpan;
            prevThickness = thickness;
            GenerateArc();
        }

        // If we want to expand the arc to 180 degrees
        if (expand)
        {
            if (lerpTimer < 1.0f)
            {
                arcSpan = (int)Mathf.Lerp(oldArc, 180.0f, lerpTimer);
                lerpTimer += Time.deltaTime * lerpRate;
                GenerateArc();
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 centreExpPosition = experimentController.homeCursorController.transform.position;

        bool visible = handCursorController.visible;
        bool isPaused = handCursorController.isPaused;
        bool isInHomeArea = handCursorController.isInHomeArea;
        bool taskCompleted = handCursorController.taskCompleted;
        bool isInTarget = handCursorController.isInTarget;

        float actDist = (
            handCursor.transform.position -
            (!IsSecondaryHome ? SecondaryHomePos : centreExpPosition)
        ).magnitude;

        bool distThreshold = actDist >= MIN_DIST; //Distance cursor has moved from home position
                                                  //Debug.Log("Actual Distance from center: " + actDist);

        //Do things when this thing is in the target (and paused), or far enough away during nocusor
        if (isPaused && !isInHomeArea)
        {
            // When IsSecondaryHome is false, target acts as a regular reach target.
            // If true, the participant must touch this "target" to spawn the real one.
            if (!IsSecondaryHome && distThreshold && !taskCompleted)
            {
                if (useExpand)
                {
                    expand = true;
                }
                else
                {
                    GetComponentInChildren<MeshRenderer>().enabled = false;
                }

                // Above only checks if its paused (for case of noCursor), needs to also check for some minimum time or distance travelled etc.
                experimentController.PauseTimer();

                experimentController.CalculateReachTime();

                targetContainerController.soundActive =
                    experimentController.GetReachTime() < 1.5f;

                targetContainerController.experimentController.positionLocCursorController.Activate();

                handCursorController.taskCompleted = true;
                handCursorController.isInTarget = false;
            }
        }
    }
}
