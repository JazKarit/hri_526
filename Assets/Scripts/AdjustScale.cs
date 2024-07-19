using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class AdjustScale : MonoBehaviour
{
    public GameObject parent;
    public GameObject adjuster;
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
        if (true)
        {
            Vector3 newPos = new Vector3();

            float rawScale = transform.localPosition.y;

            if (rawScale < -0.1f)
            {
                rawScale = -0.1f;
            }

            if (rawScale > 0.1f)
            {
                rawScale = 0.1f;
            }

            int scaleCompare = (int) ((rawScale + 0.1f) * 100.0f);

            if (Math.Abs(scaleCompare - lastScale) > 0.5)
            {
                scale = scaleCompare;

                scale /= 10.0f;

                Debug.Log(scale);
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
        parent.SetActive(flag);
        if (flag)
        {
            Vector3 adjustPos = new Vector3();
            adjustPos.y = adjuster.transform.position.x;
            adjustPos.x = adjuster.transform.position.y + 0.05f;
            adjustPos.z = adjuster.transform.position.z;
            parent.transform.position = adjustPos;
        }

    }
}
