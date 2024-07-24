using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public GameObject parent;
    Vector3 setPoint;
    public GameObject handle;
    public GameObject gripper;
    Vector3 lastOffset;
    Vector3 lastTransform;
    bool beginFlag = false;
    bool arm = true;
    public GameObject[] points = new GameObject[4];
    Vector3[] verticies;


    // Start is called before the first frame update
    void Start()
    {
        setPoint = transform.position;
        verticies = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            verticies[i] = points[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beginFlag)
        {
            handle.transform.position = gripper.transform.position;
            setPoint = gripper.transform.position;
            lastOffset = new Vector3();
            lastTransform = new Vector3();
            beginFlag = false;
        }
        Vector3 offset = TransformTools.InverseTransformPointUnscaled(transform, setPoint);
        handle.transform.position = new Vector3(handle.transform.position.x, handle.transform.position.y, handle.transform.position.z);
        //Debug.Log(offset);
        if (Vector3.Distance(lastOffset, offset) > 0.005)
        {
            lastOffset = offset;
            if (this.pointInPolygon(handle.transform.position.x, handle.transform.position.y))
            {
                handle.transform.position = new Vector3(handle.transform.position.x, setPoint.y, handle.transform.position.z);
                RobotActions.instance.MoveArm(offset);
                //publish
            }
            else
            {
                handle.transform.position = lastTransform;
            }
            
            
        }
        lastTransform = new Vector3(handle.transform.position.x, handle.transform.position.y, handle.transform.position.z);
        Debug.Log(offset);
    }




    public void Begin(bool set)
    {
        beginFlag = true;
        parent.SetActive(true);
        GameHandler.instance.ScaleToggle(true);

    }

    public void Done()
    {
        beginFlag = true;
    }

    public bool pointInPolygon(float x , float y)
    {
        int n = verticies.Length;
        bool inside = false;
        float xinters = 0.0f;

        float p1x = verticies[0].x;
        float p1y = verticies[0].y;
        for (int i = 0; i < n + 1; i++) 
        {
            float p2x = verticies[i % n].x;
            float p2y = verticies[i % n].y;

            if (y > Math.Min(p1y, p2y))
            {
                if (p2y <= Math.Max(p1y, p2y))
                {
                    if (x <= Math.Max(p1x, p2x))
                    {
                        xinters = (p2y - p1y) * (p2x - p1x) / (p2y = p1y) + p1x;
                    }

                    if ((p1x == p2x) || (xinters <= xinters))
                    {
                        inside = !(inside);
                    }
                }
            }

            p1x = p2x;
            p1y = p2y;
        }
        return inside;
    }
}