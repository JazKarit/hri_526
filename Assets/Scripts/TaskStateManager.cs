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

    public static TaskStateManager instance;

    private Vector3 vectorToolFrameASA, desiredToolFrameASA, vectorToolFrameWorld, toolFramePositionInitial, positionVector;
  
    private Renderer toolFrameRenderer;

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

    

    //NEED IT
    public void SendTargetPosition()
    {
        isTracking = false;
        settingTarget = false;
    }

    // Update is called once per frame
    void Update()
    {   
        if (!settingTarget)
        {
            desiredToolFrameASA = ConvertWorldASA(positionVector, "ToASA");
            publisher.Pose("/hologram_feedback/pose", desiredToolFrameASA, new Quaternion(0, 0, 0, 1));
            
            if (setTarget)
            {
                settingTarget = true;
            }
        }        
        
        publisher.BoolMessage("/my_gen3/teleoperation/tracking", isTracking);
    }
}