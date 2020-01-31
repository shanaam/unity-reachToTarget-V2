using UnityEngine;

public class PhysicsObjectBase : MonoBehaviour
{
    private GrabableObject grabbable;

    private void Start()
    {
        grabbable = GetComponent<GrabableObject>();
    }

    private void LateUpdate()
    {
        // Pressing R will reset the position of the ball
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var ctrler = GameObject.FindGameObjectWithTag("TargetContainer");
            transform.SetParent(ctrler.transform);
            transform.localPosition = new Vector3(0.0f, 0.0f, 0.10f);
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.SetParent(null);

            if (grabbable.objectGrabbed)
            {
                grabbable.Drop();
            }

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GetComponent<Rigidbody>().Sleep();
        }
    }
}
