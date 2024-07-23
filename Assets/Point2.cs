using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point2 : MonoBehaviour
{
    public GameObject parent;
    Vector3 setPoint;
    Vector3 offset;
    public GameObject gripper;
    public GameObject handle;
    Vector3 lastOffset;
    bool beginFlag = false;
    bool arm = true;


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
            //RobotActions.instance. whatever wiping function

            beginFlag = false;
        }
        offset = TransformTools.InverseTransformPointUnscaled(transform, setPoint);
        handle.transform.position = new Vector3(handle.transform.position.x, setPoint.y, handle.transform.position.z);
        //Debug.Log(offset);
        if (Vector3.Distance(lastOffset, offset) > 0.005)
        {
            lastOffset = offset;
            
        }
        Debug.Log(offset);
    }




    public void Begin(bool set)
    {
        beginFlag = true;
        arm = set;
        parent.SetActive(true);

    }

    public void Done()
    {
        RobotActions.instance.CreateSecondPoint(offset);
        //parent.SetActive(false);
    }
}
