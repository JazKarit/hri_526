using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Microsoft.MixedReality.Toolkit.UI;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;

public class ROSPublisher : MonoBehaviour
{
    public static ROSPublisher instance;

    void Start()
    {
        // start the ROS connection
        ROSConnection.GetOrCreateInstance();
        if (instance != null)
        {
            Debug.Log("singleton error");

        }
        instance = this;
    }

    public void Pose(string topicName, Vector3 objectPosition, Quaternion objectOrientation)
    {
        // Decompose Vector3 and Quaternion into their components
        PointMsg position = new PointMsg
        {
            x = objectPosition.x,
            y = objectPosition.y,
            z = objectPosition.z
        };

        QuaternionMsg orientation = new QuaternionMsg
        {
            x = objectOrientation.x,
            y = objectOrientation.y,
            z = objectOrientation.z,
            w = objectOrientation.w
        };

        // Create message
        PoseMsg pose = new PoseMsg
        {
            position = position,
            orientation = orientation
        };

        ROSConnection.GetOrCreateInstance().Publish(topicName, pose);

    }

    public void DoublePose(string topicName, Vector3 objectPosition1, Vector3 objectPosition2, Quaternion objectOrientation1, Quaternion objectOrientation2)
    {
        // Decompose Vector3 and Quaternion into their components
        PointMsg position1 = new PointMsg
        {
            x = objectPosition1.x,
            y = objectPosition1.y,
            z = objectPosition1.z
        };

        QuaternionMsg orientation1 = new QuaternionMsg
        {
            x = objectOrientation1.x,
            y = objectOrientation1.y,
            z = objectOrientation1.z,
            w = objectOrientation1.w
        };

        PointMsg position2 = new PointMsg
        {
            x = objectPosition2.x,
            y = objectPosition2.y,
            z = objectPosition2.z
        };

        QuaternionMsg orientation2 = new QuaternionMsg
        {
            x = objectOrientation2.x,
            y = objectOrientation2.y,
            z = objectOrientation2.z,
            w = objectOrientation2.w
        };

        // Create message
        PoseMsg pose1 = new PoseMsg
        {
            position = position1,
            orientation = orientation1
        };

        PoseMsg pose2 = new PoseMsg
        {
            position = position2,
            orientation = orientation2
        };

        PoseArrayMsg poseArray = new PoseArrayMsg
        {
            header = new HeaderMsg(),
            poses = new PoseMsg[] { pose1, pose2 }
        };

        ROSConnection.GetOrCreateInstance().Publish(topicName, poseArray);

    }

    public void Position(string topicName, Vector3 objectPosition)
    {
        // Create message
        PointMsg position = new PointMsg(
            objectPosition.x,
            objectPosition.y,
            objectPosition.z
            );

        // Publish the message
        ROSConnection.GetOrCreateInstance().Publish(topicName, position);
    }

    /// <summary>
    /// Send a Quaternion Message to server_endpoint.py running in ROS.
    /// The rotation expressed in quaternions of the GameObject is converted to Right-Handed Coordinate System.
    /// <param name="topicName"></param>
    /// <param name="objectToPublish"></param>
    public void RotationMessage(string topicName, GameObject objectToPublish)
    {
        //// Covert to ROS Coordinate Frame
        //QuaternionMsg rotationFLU = objectToPublish.transform.rotation.To<FLU>();

        //// Create message
        //QuaternionMsg rotation = new QuaternionMsg(
        //    rotationFLU.x,
        //    rotationFLU.y,
        //    rotationFLU.z,
        //    rotationFLU.w
        //    );

        // Create message
        QuaternionMsg rotation = new QuaternionMsg(
            objectToPublish.transform.rotation.x,
            objectToPublish.transform.rotation.y,
            objectToPublish.transform.rotation.z,
            objectToPublish.transform.rotation.w
            );

        // Publish the message
        ROSConnection.GetOrCreateInstance().Publish(topicName, rotation);
    }

    public void StringMessage(string topicName, string message)
    {
        StringMsg stringMsg = new StringMsg { data = message };
        ROSConnection.GetOrCreateInstance().Publish(topicName, stringMsg);
    }

    public void Int32Message(string topicName, int message)
    {
        Int32Msg intMsg = new Int32Msg { data = message };
        ROSConnection.GetOrCreateInstance().Publish(topicName, intMsg);
    }

    public void Float32Message(string topicName, float message)
    {
        Float32Msg floatMsg = new Float32Msg { data = message };
        ROSConnection.GetOrCreateInstance().Publish(topicName, floatMsg);
    }

    public void BoolMessage(string topicName, bool message)
    {
        BoolMsg booleanMsg = new BoolMsg { data = message };
        ROSConnection.GetOrCreateInstance().Publish(topicName, booleanMsg);
    }
}