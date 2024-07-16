using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point1 : MonoBehaviour
{
    public GameObject parent;
    Vector3 setPoint;
    public GameObject handle;
    public GameObject gripper;
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
        Vector3 offset = TransformTools.InverseTransformPointUnscaled(transform, setPoint);
        handle.transform.position = new Vector3(handle.transform.position.x, setPoint.y, handle.transform.position.z);
        //Debug.Log(offset);
        if (Vector3.Distance(lastOffset, offset) > 0.005)
        {
            lastOffset = offset;
            //RobotActions.instance. whatever wiping function
        }
        Debug.Log(offset);
    }




    public void Begin(bool set)
    {
        beginFlag = true;
        arm = set;
        Debug.Log("Adjuster set" + arm);
        parent.SetActive(true);

    }

    public void Done()
    {
        parent.SetActive(false);
        //RobotActions.instance. whatever wiping function
    }
}
