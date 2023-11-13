using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using TFMessage = RosMessageTypes.Tf2.TFMessageMsg;
using TransformStamped = RosMessageTypes.Geometry.TransformStampedMsg;
using UnityEngine;

public class TFManager : MonoBehaviour
{
    [System.Serializable]
    public class TFObject
    {
        public string name;
        public GameObject gameObject;
    }

    public TFObject[] subscribers;
    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<TFMessage>("/objects", UpdatePoses);
    }

    void UpdatePoses(TFMessage TFMsg)
    {
        foreach (TFObject tfo in subscribers)
        {
            bool found = false;
            foreach (TransformStamped transformStampedMsg in TFMsg.transforms)
            {
                if (tfo.name.Equals(transformStampedMsg.child_frame_id))
                {
                    found = true;
                    tfo.gameObject.SetActive(true);
                    Quaternion uq = transformStampedMsg.transform.rotation.From<FLU>();
                    tfo.gameObject.transform.localRotation = new Quaternion(-uq.z, uq.y, uq.x, uq.w);
                    tfo.gameObject.transform.localPosition = new Vector3(-(float)transformStampedMsg.transform.translation.x, (float)transformStampedMsg.transform.translation.z, -(float)transformStampedMsg.transform.translation.y);
                    break;
                }
            }
            if (!found)
            {
                tfo.gameObject.SetActive(false);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
