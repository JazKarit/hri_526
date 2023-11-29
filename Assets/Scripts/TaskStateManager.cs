using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
//using RosMessageTypes.Scripts;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Audio;

public class TaskStateManager : MonoBehaviour
{
    [SerializeField] private ROSPublisher publisher;
    [SerializeField] public GameObject spatialAnchor, digitalTwinObject, endEffectorModel;
    [SerializeField] private bool moveArm = false, settingTarget = true;
    public bool isTracking = false, setTarget = false, calibratingAnchor = false;
    [SerializeField] private Interactable toggleSwitchArm;
    private int gripperState = 0; // gripper is open
    private Vector3 vectorToolFrameASA, desiredToolFrameASA, vectorToolFrameWorld, toolFramePositionInitial, calibrationToolFrameASA;

    //private TextToSpeech textToSpeech;

    public static TaskStateManager instance;


    void Start()
    {
        // Publishers
        ROSConnection.GetOrCreateInstance().RegisterPublisher<PoseMsg>("/hologram_feedback/pose");
        ROSConnection.GetOrCreateInstance().RegisterPublisher<BoolMsg>("/my_gen3/teleoperation/tracking");
        ROSConnection.GetOrCreateInstance().RegisterPublisher<PointMsg>("/my_gen3/calibrate_anchor");
        ROSConnection.GetOrCreateInstance().RegisterPublisher<Int32Msg>("/my_gen3/teleoperation/gripper_state");

        // Subscribers
        ROSConnection.GetOrCreateInstance().Subscribe<PointMsg>("/my_gen3/tf_toolframe", ToolFrameUpdate);

        // Get TextToSpeech and Renderer components of the sphere
        if (instance != null)
        {
            Debug.Log("Singleton error task state manager");
        }else
        {
            instance = this;
        }
    }

    // Callback function for handling tool frame updates
    void ToolFrameUpdate(PointMsg toolFramePos)
    {
        vectorToolFrameASA = new Vector3((float)toolFramePos.x, (float)toolFramePos.y, -(float)toolFramePos.z);
        vectorToolFrameWorld = ConvertWorldASA(vectorToolFrameASA, "ToWorld");
    }

    // Convert position vectors between world and spatial anchor reference frames
    private Vector3 ConvertWorldASA(Vector3 positionVector, string toFrame)
    {
        Vector3 newPositionVector = Vector3.zero; // Initialize the vector

        switch (toFrame)
        {
            case "ToASA":
                newPositionVector = spatialAnchor.transform.InverseTransformDirection(positionVector - spatialAnchor.transform.position);
                break;
            case "ToWorld":
                newPositionVector = spatialAnchor.transform.TransformDirection(positionVector) + spatialAnchor.transform.position;
                break;
        }

        return newPositionVector;
    }

    private void ActivateControl(bool isMoveArm, string startSpeakingMessage)
    {

        if (!isTracking)
        {
            
            //textToSpeech.StartSpeaking($"Tracking on. {startSpeakingMessage}");

            //isTracking = true;
        }
        else
        {
            moveArm = isMoveArm;
            setTarget = !isMoveArm;
            settingTarget = !isMoveArm;
            string controlType = isMoveArm
                ? "Move arm"
                : "Set target";

            if ((isMoveArm && moveArm) || (!isMoveArm && setTarget))
            {
                //textToSpeech.StartSpeaking($"{controlType} control is already active.");
            }
            else
            {
                //textToSpeech.StartSpeaking($"Switched to {controlType} control.");
            }
        }

        
    }

    public void MoveArm()
    {
        ActivateControl(true, "Grab and move the sphere to move the robot arm.");
    }

    public void SetTarget()
    {
        ActivateControl(false, "Put the sphere where you want the robot arm to move.");
    }

    public void SendTargetPosition()
    {
        if (calibratingAnchor)
        {
            UpdateAnchorPosition();
            calibratingAnchor = false;
            //textToSpeech.StartSpeaking("Spatial Anchor was calibrated.");
        }
        else
        {
            settingTarget = false;
        }
    }

    public void ToggleArmControl()
    {

        if (toggleSwitchArm.IsToggled)
        {
            //textToSpeech.StartSpeaking("You can now control the arm.");
            isTracking = true;
        }
        else
        {
            moveArm = false;
            setTarget = false;
            settingTarget = false;
            //textToSpeech.StartSpeaking("Manual control was disabled.");

            isTracking = false;
        }
    }

    public void calibrateAnchor()
    {
        calibratingAnchor = true;
        endEffectorModel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
    }

    public void UpdateAnchorPosition()
    {
        if (calibratingAnchor)
        {
            toolFramePositionInitial = vectorToolFrameASA;
            Vector3 anchorDifference = toolFramePositionInitial - calibrationToolFrameASA;

            publisher.Position("/my_gen3/calibrate_anchor", anchorDifference);
            calibratingAnchor = false;
        }

        
    }

    public void ManageGripper()
    {
        // Switch gripper state (open/close)
        gripperState = gripperState ^ 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (isTracking || calibratingAnchor)
        {
            toggleSwitchArm.IsToggled = true;
        }
        else
        {
            toggleSwitchArm.IsToggled = false;
            //digitalTwinObject.transform.position = vectorToolFrameWorld;
        }
        if (calibratingAnchor)
        {
            calibrationToolFrameASA = ConvertWorldASA(endEffectorModel.transform.position, "ToASA");
        }
        else
        {
            endEffectorModel.transform.position = vectorToolFrameWorld;
        }
        
        if (digitalTwinObject != null)
        {
            desiredToolFrameASA = ConvertWorldASA(digitalTwinObject.transform.position, "ToASA");

            if (!settingTarget)
            {
                Debug.Log("Sending Position: "+ desiredToolFrameASA.x + ", " + desiredToolFrameASA.y + ", " + desiredToolFrameASA.z);
                publisher.Pose("/hologram_feedback/pose", desiredToolFrameASA, new Quaternion(0, 0, 0, 1));

                if (setTarget)
                {
                    settingTarget = true;
                    digitalTwinObject.GetComponent<Selector>().Cancel();
                    digitalTwinObject = null;
                }
            }
        }
        

        publisher.BoolMessage("/my_gen3/teleoperation/tracking", isTracking);
        publisher.Int32Message("/my_gen3/teleoperation/gripper_state", gripperState);
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Microsoft.MixedReality.Toolkit.UI;

public class TaskStateManager : MonoBehaviour
{
    [SerializeField] private ROSPublisher publisher;
    [SerializeField] public GameObject spatialAnchor, digitalTwinObject;
    [SerializeField] private bool settingTarget = false;
    public bool isTracking = false, setTarget = false;
    public bool sendOnce = false;

    

    public static TaskStateManager instance;

    private Vector3 vectorToolFrameASA, desiredToolFrameASA, vectorToolFrameWorld, toolFramePositionInitial, positionVector;
  
    private Renderer toolFrameRenderer;

    public GameObject eef;

    void Start()
    {
        // Publishers
        ROSConnection.GetOrCreateInstance().RegisterPublisher<PoseMsg>("/hologram_feedback/pose");
        ROSConnection.GetOrCreateInstance().RegisterPublisher<BoolMsg>("/my_gen3/teleoperation/tracking");

        // Subscribers
        ROSConnection.GetOrCreateInstance().Subscribe<PointMsg>("/my_gen3/tf_toolframe", ToolFrameUpdate);
        if (instance != null)
        {
            Debug.Log("Something is very wrong");
        }
        instance = this;

        }

    //NEED IT
    // Callback function for handling tool frame updates
    void ToolFrameUpdate(PointMsg toolFramePos)
    {
        vectorToolFrameASA = new Vector3((float)toolFramePos.x, (float)toolFramePos.y, -(float)toolFramePos.z);
        vectorToolFrameWorld = ConvertWorldASA(vectorToolFrameASA, "ToWorld");
        
        if (setTarget)
        {
            positionVector = digitalTwinObject.transform.position; //Add your hologram transform
        }
        else
        {
            positionVector = vectorToolFrameWorld;
        }
    }

    //NEED IT
    // Convert position vectors between world and spatial anchor reference frames
    private Vector3 ConvertWorldASA(Vector3 positionVector, string toFrame)
    {
        Vector3 newPositionVector = Vector3.zero; // Initialize the vector

        switch (toFrame)
        {
            case "ToASA":
                newPositionVector = spatialAnchor.transform.InverseTransformDirection(positionVector - spatialAnchor.transform.position);
                break;
            case "ToWorld":
                newPositionVector = spatialAnchor.transform.TransformDirection(positionVector) + spatialAnchor.transform.position;
                break;
        }

        return newPositionVector;
    }

    

    public void InitializeTracking()
    {
        setTarget = true;
        settingTarget = true;
    }

    //NEED IT
    public void StartTracking()
    {
        sendOnce = false;
        settingTarget = false;
    }

    public void StopTracking()
    {
        settingTarget = true;
    }

    public void GoTo()
    {
        sendOnce = true;
        settingTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (vectorToolFrameWorld != null)
        {
            eef.transform.position = vectorToolFrameWorld;
        }
        if (!settingTarget)
        {
            desiredToolFrameASA = ConvertWorldASA(positionVector, "ToASA");
            publisher.Pose("/hologram_feedback/pose", desiredToolFrameASA, new Quaternion(0, 0, 0, 1));
            
            if (sendOnce)
            {
                settingTarget = true;
            }
        }        
        
        publisher.BoolMessage("/my_gen3/teleoperation/tracking", isTracking);
    }
}
*/