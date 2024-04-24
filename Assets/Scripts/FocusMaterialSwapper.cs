using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
public class FocusMaterialSwapper : BaseEyeFocusHandler
{
    public Material selected;
    public Material normal;
    public void  Start()
    {
        // Turn on gaze pointer
        PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOn);
    }

    public void Update()
    {
        LogCurrentGazeTarget();
    }

    void LogCurrentGazeTarget()
    {
        if (CoreServices.InputSystem.GazeProvider.GazeTarget)
        {
            if (CoreServices.InputSystem.GazeProvider.GazeTarget == gameObject)
            {
                gameObject.GetComponent<Renderer>().material = selected;
            } else
            {
                gameObject.GetComponent<Renderer>().material = normal;
            }

        } else
        {
            gameObject.GetComponent<Renderer>().material = normal;
        }
    }

}