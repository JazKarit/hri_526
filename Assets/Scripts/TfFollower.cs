using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using TransformStamped = RosMessageTypes.Geometry.TransformStampedMsg;
using UnityEngine;

public class TfFollower : MonoBehaviour
{
    public string name;

    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<TransformStamped>("/objects/" + name, UpdatePose);
        Debug.Log("Listening to " + "/objects/" + name);
    }

    void UpdatePose(TransformStamped transformStampedMsg)
    {

        Debug.Log(transformStampedMsg.transform.translation.x);
        Quaternion uq = transformStampedMsg.transform.rotation.From<FLU>();
        //Quaternion uq = transformStampedMsg.transform.rotation;
        //this.gameObject.transform.localRotation = new Quaternion(uq.y, -uq.x, uq.z, uq.w);
        //this.gameObject.transform.localPosition = new Vector3(-(float)transformStampedMsg.transform.translation.x, (float)transformStampedMsg.transform.translation.z, (float)transformStampedMsg.transform.translation.y);
        this.gameObject.transform.localRotation = new Quaternion(-uq.z, uq.y, uq.x, uq.w);
        this.gameObject.transform.localPosition = new Vector3(-(float)transformStampedMsg.transform.translation.x, (float)transformStampedMsg.transform.translation.z, -(float)transformStampedMsg.transform.translation.y);




    }

    // Update is called once per frame
    void Update()
    {

    }
}