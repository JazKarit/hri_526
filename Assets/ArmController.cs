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
            RobotActions.instance.ToggleArmController(true);
            beginFlag = false;
        }

        verticies = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            verticies[i] = points[i].transform.position;
        }

        Vector3 offset = TransformTools.InverseTransformPointUnscaled(transform, setPoint);
        if (Vector3.Distance(lastOffset, offset) > 0.005)
        {
            lastOffset = offset;
            float xValue = handle.transform.position.x + 100.0f;
            float yValue = handle.transform.position.z + 100.0f;
            if (this.pointInPolygon(xValue, yValue))
            {
                Debug.Log("Can Move");
                handle.transform.position = new Vector3(handle.transform.position.x, setPoint.y, handle.transform.position.z);
                lastTransform = new Vector3(handle.transform.position.x, handle.transform.position.y, handle.transform.position.z);
                RobotActions.instance.MoveArm(offset);
                //publish
            }
            else
            {
                handle.transform.position = lastTransform;
            }
            
            
        }
        
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
        int n = 4;
        bool inside = false;
        float xinters = 0.0f;

        float p1x = verticies[0].x + 100.0f;
        float p1y = verticies[0].z + 100.0f;
        for (int i = 0; i < n + 1; i++) 
        {
            float p2x = verticies[i % n].x + 100.0f;
            float p2y = verticies[i % n].z + 100.0f;

            if (y > Math.Min(p1y, p2y))
            {
                if (y <= Math.Max(p1y, p2y))
                {
                    if (x <= Math.Max(p1x, p2x))
                    {
                        xinters = (y - p1y) * (p2x - p1x) / (p2y - p1y) + p1x;
                    }

                    if (Math.Abs(x) <= Math.Abs(xinters))
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

    public void Deactivate()
    {
        parent.SetActive(false);
        GameHandler.instance.ScaleToggle(false);
    }
}