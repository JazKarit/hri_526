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
        public GameObject selector;
        public bool selectorLocked = false;
        public bool selectable = false;
    }

    public TFObject[] subscribers;

    public bool mute;

    public static TFManager instance; //This is a singleton
    private TFMessage TFMsg;


    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<TFMessage>("/objects", UpdatePoses);
        if (instance != null)
        {
            Debug.Log("Multiple of this singleton");
        }
        instance = this;
    }

    void UpdatePoses(TFMessage TFMsg)
    {
        if (mute) return;
        this.TFMsg = TFMsg;
        


        // foreach (TFObject tfo in subscribers)
        // {
        //     if (tfo.gameObject == null) continue;
        //     bool found = false;
        //     foreach (TransformStamped transformStampedMsg in TFMsg.transforms)
        //     {
        //         Debug.Log(transformStampedMsg.child_frame_id);
        //         if (tfo.name.Equals(transformStampedMsg.child_frame_id))
        //         {
                    
        //             Debug.Log(tfo.name);
        //             found = true;
        //             tfo.gameObject.SetActive(true);
        //             if (tfo.selector != null && !tfo.selectorLocked)
        //             {
        //                 tfo.selector.SetActive(true);
        //                 tfo.selectable = true;
        //             }
        //             Quaternion uq = transformStampedMsg.transform.rotation.From<FLU>();
        //             tfo.gameObject.transform.localRotation = new Quaternion(-uq.z, uq.y, uq.x, uq.w);
        //             tfo.gameObject.transform.localPosition = new Vector3(-(float)transformStampedMsg.transform.translation.x, (float)transformStampedMsg.transform.translation.z, -(float)transformStampedMsg.transform.translation.y);
        //             if (tfo.gameObject.GetComponent<VelocityTracker>() != null)
        //             {
        //                 tfo.gameObject.GetComponent<VelocityTracker>().PositionUpdated();
        //             }
        //             break;
        //         }
        //     }
        //     if (!found)
        //     {
        //         tfo.gameObject.SetActive(false);
        //         tfo.selectable = false;
        //         if (tfo.selector != null)
        //         {
        //             tfo.selector.SetActive(false);
        //         }
        //     }
        // }

    }

    //Disable all selectors besides specified
    public void LockSelectors(GameObject selector)
    {
        foreach (TFObject tfo in subscribers)
        {
            if (tfo.selector == null) continue;
            if (tfo.selector != selector)
            {
                tfo.selectorLocked = true;
                tfo.selector.SetActive(false);
            }
        }
    }


    //Enable all selectors
    public void UnlockSelectors()
    {
        foreach (TFObject tfo in subscribers)
        {
            tfo.selectorLocked = false;
        }
    }

    public void ToggleMute() {
        mute = !mute;
    }

    // Update is called once per frame
    void Update()
    {
        if (TFMsg != null) {
            foreach (TFObject tfo in subscribers)
        {
            if (tfo.gameObject == null) continue;
            bool found = false;
            foreach (TransformStamped transformStampedMsg in TFMsg.transforms)
            {
                Debug.Log(transformStampedMsg.child_frame_id);
                if (tfo.name.Equals(transformStampedMsg.child_frame_id))
                {
                    
                    Debug.Log(tfo.name);
                    found = true;
                    tfo.gameObject.SetActive(true);
                    if (tfo.selector != null && !tfo.selectorLocked)
                    {
                        tfo.selector.SetActive(true);
                        tfo.selectable = true;
                    }
                    Quaternion uq = transformStampedMsg.transform.rotation.From<FLU>();
                    tfo.gameObject.transform.localRotation = new Quaternion(-uq.z, uq.y, uq.x, uq.w);
                    tfo.gameObject.transform.localPosition = new Vector3(-(float)transformStampedMsg.transform.translation.x, (float)transformStampedMsg.transform.translation.z, -(float)transformStampedMsg.transform.translation.y);
                    if (tfo.gameObject.GetComponent<VelocityTracker>() != null)
                    {
                        tfo.gameObject.GetComponent<VelocityTracker>().PositionUpdated();
                    }
                    break;
                }
            }
            if (!found)
            {
                tfo.gameObject.SetActive(false);
                tfo.selectable = false;
                if (tfo.selector != null)
                {
                    tfo.selector.SetActive(false);
                }
            }
        }
        TFMsg = null;
        }
        
    }
}
