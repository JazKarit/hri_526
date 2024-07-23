using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class AdjustScale : MonoBehaviour
{
    public GameObject parent;
    public GameObject adjuster;
    public GameObject readout;
    private float scale = 0;
    private int lastScale;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Vector3 newPos = new Vector3();

            float rawScale = transform.localPosition.y;

            if (rawScale < -1.0f)
            {
                rawScale = -1.0f;
            }

            if (rawScale > 1.0f)
            {
                rawScale = 1.0f;
            }

            int scaleCompare = (int) ((rawScale + 1.0f) * 10.0f);

            if (Math.Abs(scaleCompare - lastScale) > 0.5)
            {
                scale = scaleCompare;

                scale /= 10.0f;

                readout.GetComponent<TextMesh>().text = "Sensitivity: " + scale + "x";
                RobotActions.instance.SetScale(scale);
            }

            lastScale = scaleCompare;

            newPos.y = rawScale;

            transform.localPosition = newPos;
        }
    }

    public void set(bool flag)
    {
        active = flag;
        if (flag)
        {
            Vector3 adjustPos = new Vector3();
            adjustPos.x = adjuster.transform.position.x - 0.05f;
            adjustPos.y = adjuster.transform.position.y;
            adjustPos.z = adjuster.transform.position.z;
            parent.transform.position = adjustPos;
        }

        parent.SetActive(flag);

    }
}
