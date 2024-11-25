using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
//using RosMessageTypes.Scripts;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Microsoft.MixedReality.Toolkit.UI;



public class TeleopEEF : MonoBehaviour
{
    public GameObject armOrigin;
    public GameObject gripper;
    public bool armed;
    public float distanceThreshold = 0.01f;
    private Vector3 lastTrans;
    private Quaternion lastRot;
    private Vector3 lastSentTrans;
    private Quaternion lastSentRot;
    public bool calibrating;
    public Vector3 calibrationOffset;
    public Interactable toggleSwitchArm;
    ROSConnection ros;


    public bool debugButton;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ROSConnection.GetOrCreateInstance().RegisterPublisher<PoseMsg>("arm/go_to");
        ROSConnection.GetOrCreateInstance().RegisterPublisher<BoolMsg>("arm/grip");

        lastTrans = transform.position;
        lastRot = transform.rotation;
        lastSentTrans = transform.position;
        lastSentRot = transform.rotation;
        calibrationOffset = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = gameObject.GetComponent<MotionConstrainer>().lastPos;
        Quaternion currentRot = gameObject.GetComponent<MotionConstrainer>().lastRot;
        if (debugButton) {
            debugButton = false;
            Pinch();
        }
        if (armed) {
            if (currentRot != lastSentRot || Vector3.Distance(lastSentTrans, currentPos) > distanceThreshold) {
                Vector3 relTransform = armOrigin.transform.InverseTransformPoint(currentPos - calibrationOffset);
                Quaternion silly = new Quaternion();
                silly.w = currentRot.eulerAngles.y + (((180/Mathf.PI)*Mathf.Atan2(armOrigin.transform.position.z - currentPos.z,armOrigin.transform.position.x - currentPos.x))-180);
                silly.w = silly.w < 0 ? silly.w + 360 : silly.w > 360? silly.w -360 : silly.w;
                ROSPublisher.instance.Pose("arm/go_to", new Vector3(relTransform.x, relTransform.z, relTransform.y),silly);
                lastSentRot = currentRot;
                lastSentTrans = currentPos;

            
            }
        }
    }

    public void arm() {
        if (calibrating == false) {
            this.armed = true;
        }
        
    }

    public void sync() {
        this.transform.position = gripper.transform.position;
    }

    public void Pinch() {
        ros.Publish("arm/grip", new BoolMsg(true));
    }

    public void Release() {
        ros.Publish("arm/grip", new BoolMsg(false));
    }

    public void ToggleCalibration() {
        if (this.calibrating) {
            StopCalibration();
        }else {
            StartCalibration();
        }
    }

    public void StartCalibration() {
        this.calibrating = true;
        this.armed = false;
        calibrationOffset = this.transform.position;
        Debug.Log("Calibration started");
    }

    public void StopCalibration() {
        if (!calibrating) return;
        this.calibrating = false;
        calibrationOffset = this.transform.position - this.calibrationOffset;
        Debug.Log("Calibration Ended");
    }
}
