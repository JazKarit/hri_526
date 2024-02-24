using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TurnEvent : UnityEvent<float>
{
}

public class Dial : MonoBehaviour
{

    private float lastAngle;
    public TurnEvent onRotate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float angle = transform.localRotation.ToEuler().y;
        if (angle != lastAngle)
        {
            
            onRotate.Invoke(angle);
        }
        this.lastAngle = angle;
    }
}
