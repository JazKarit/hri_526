using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point4 : MonoBehaviour
{
    public GameObject parent;
    Vector3 setPoint;
    Vector3 offset;
    public GameObject gripper;
    public GameObject handle;
    Vector3 lastOffset;
    bool beginFlag = false;
    bool arm = true;
    public GameObject point1Handle;
    public GameObject point2Handle;
    public GameObject point3Handle;
    int lengthOfLineRenderer = 5;



    // Start is called before the first frame update
    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        setPoint = transform.position;
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = lengthOfLineRenderer;
    }

    // Update is called once per frame
    void Update()
    {
        if (beginFlag)
        {
            handle.transform.position = new Vector3(gripper.transform.position.x, 0, gripper.transform.position.z); 
            setPoint = gripper.transform.position;
            lastOffset = new Vector3();
            //RobotActions.instance. whatever wiping function

            beginFlag = false;
        }
        offset = TransformTools.InverseTransformPointUnscaled(transform, setPoint);
        handle.transform.position = new Vector3(handle.transform.position.x, 0, handle.transform.position.z);
        //Debug.Log(offset);
        if (Vector3.Distance(lastOffset, offset) > 0.005)
        {
            lastOffset = offset;
        }
    }




    public void Begin(bool set)
    {
        beginFlag = true;
        arm = set;
        parent.SetActive(true);

    }

    public void Done()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.SetPosition(0, point1Handle.transform.position);
        lineRenderer.SetPosition(1, point2Handle.transform.position);
        lineRenderer.SetPosition(2, point3Handle.transform.position);
        lineRenderer.SetPosition(3, handle.transform.position);
        lineRenderer.SetPosition(4, point1Handle.transform.position);

        Vector3 point1 = point1Handle.transform.position;
        Vector3 point2 = point2Handle.transform.position;
        Vector3 point3 = point3Handle.transform.position;
        Vector3 point4 = handle.transform.position;
        Vector3 sum = point1 + point2 + point3 + point4;

        Vector3 center = sum / 4.0f;
        RobotActions.instance.CreateFourthPoint(center);        

        
        //parent.SetActive(false);
    }

    public void Deactivate()
    {
        parent.SetActive(false);
    }
}
