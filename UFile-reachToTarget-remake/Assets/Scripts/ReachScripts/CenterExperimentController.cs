using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterExperimentController : MonoBehaviour
{
    public GameObject RealHand;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            Vector3 cursorPos = RealHand.transform.position;
            transform.position = new Vector3(cursorPos.x, cursorPos.y, cursorPos.z);
            Debug.Log("Home Position Transformed: (" + cursorPos.x + ", " + cursorPos.y + ", " + cursorPos.z + ")");
        }
    }
}
