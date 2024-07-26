using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point1 : MonoBehaviour
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
        RobotActions.instance.CreateFirstPoint(offset);
        //parent.SetActive(false);
    }

    public void Deactivate()
    {
        parent.SetActive(false);
    }
}
