using TMPro;
using UnityEngine;

public class InstructionController : MonoBehaviour
{
    TextMeshPro text;
    public GameObject hand;
    HandCursorController cntrler;

    private void Start()
    {
        text = GetComponent<TextMeshPro>();
        cntrler = hand.GetComponent<HandCursorController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cntrler.HandForNextTrial == string.Empty)
        {
            text.text = "Reach to Target";
        }
        else
        {
            text.text = cntrler.HandForNextTrial == "r" ? "Right Hand" : "Left Hand";
        }
        
    }
}
