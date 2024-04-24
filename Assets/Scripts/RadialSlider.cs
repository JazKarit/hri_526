using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RadialSlideEvent : UnityEvent<float>
{
}

public class RadialSlider : MonoBehaviour
{
    public GameObject center;
    public float radius = 0.1f;
    private float lastAngle;
    public GameObject readout;

    public RadialSlideEvent onRotate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3();
        //newPos.x = center.transform.position.x;
        
        float angle = Mathf.Atan2(transform.localPosition.x, transform.localPosition.z);
        if (angle != lastAngle)
        {
            onRotate.Invoke(angle - (Mathf.PI/2));
            int read = (int)((angle - (Mathf.PI / 2)) * (180 / Mathf.PI));
            read += read < -180 ? 360 : 0;
            readout.GetComponent<TextMesh>().text = read +"°";
        }
        this.lastAngle = angle;
        newPos.z = radius * Mathf.Cos(angle);
        newPos.x = radius * Mathf.Sin(angle);
        


        transform.localPosition = newPos;
        
    }
}
