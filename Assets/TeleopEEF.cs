using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
//using RosMessageTypes.Scripts;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;


public class TeleopEEF : MonoBehaviour
{
    public GameObject armOrigin;
    public bool armed;
    public float distanceThreshold = 0.01f;
    private Vector3 lastTrans;
    private Quaternion lastRot;
    private Vector3 lastSentTrans;
    private Quaternion lastSentRot;
    ROSConnection ros;
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ROSConnection.GetOrCreateInstance().RegisterPublisher<PoseMsg>("arm/go_to");
        lastTrans = transform.position;
        lastRot = transform.rotation;
        lastSentTrans = transform.position;
        lastSentRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (armed) {
            if (Vector3.Distance(lastSentTrans, transform.position) > distanceThreshold) {
                Vector3 relTransform = armOrigin.transform.InverseTransformPoint(transform.position);
                ROSPublisher.instance.Pose("arm/go_to", new Vector3(relTransform.x, relTransform.z, relTransform.y), new Quaternion());
                lastSentRot = transform.rotation;
                lastSentTrans = transform.position;
            }
        }
    }
}
