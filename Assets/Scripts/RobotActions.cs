using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class RobotActions : MonoBehaviour
{
    public enum RobotState
    {
        PICK_ON_MOVE,
        HOLDING,
        IDLE,
        DISABLED
    }

    public static RobotActions instance;
    ROSConnection ros;
    public RobotState state = RobotState.DISABLED;
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<BoolMsg>("/arm/pickup/blue_cup");
        ros.RegisterPublisher<BoolMsg>("/arm/place_in_box");
        ros.RegisterPublisher<PointMsg>("/arm/pour_adjust");
        ros.RegisterPublisher<BoolMsg>("/arm/pickup_toggle");
        ros.RegisterPublisher<PointMsg>("/arm/pickup_adjust");
        ros.RegisterPublisher<StringMsg>("/arm/pickup_prism");
        ros.RegisterPublisher<PointMsg>("/arm/insert_adjust");
        ros.RegisterPublisher<BoolMsg>("/arm/insert_toggle");
        if (instance != null)
        {
            Debug.Log("Uh oh");
        } else
        {
            instance = this;
        }

    }

    /*public void PickUpBlueCup()
    {
        if (this.state == RobotState.PICK_ON_MOVE)
        {
            this.state = RobotState.HOLDING;
            ros.Publish("/arm/pickup/blue_cup", new BoolMsg(true));
        }

    }*/

    /*public void PickUpBlueCup()
    {
        if (this.state == RobotState.PICK_ON_MOVE)
        {
            this.state = RobotState.HOLDING;
            ros.Publish("/arm/pickup/blue_cup", new BoolMsg(true));
        }

    }*/

    public void PickUpPrism()
    {
        Debug.Log("Pickup" + this.state);
        if (this.state == RobotState.PICK_ON_MOVE)
        {
            this.state = RobotState.HOLDING;
            ros.Publish("/arm/pickup_prism", new StringMsg("Red"));
        }
    }

    public void PlaceInBox()
    {
        if (this.state == RobotState.HOLDING)
        {
            this.state = RobotState.IDLE;
            ros.Publish("/arm/place_in_box", new BoolMsg(true));
            this.state = RobotState.IDLE;
        }

    }

    public void PourAdjust(Vector3 transform)
    {
        PointMsg msg = new PointMsg();
        msg.x = transform.x;
        msg.y = transform.y;
        msg.z = transform.z;
        ros.Publish("/arm/pour_adjust", msg);
    }

    public void ToggleArmAdjust(bool state)
    {
        ros.Publish("/arm/pickup_toggle", new BoolMsg(state));
        Debug.Log("TOGGLED " + state);
    }

    public void GrabAdjust(Vector3 transform)
    {
        PointMsg msg = new PointMsg();
        msg.x = transform.x;
        msg.y = transform.y;
        msg.z = transform.z;
        ros.Publish("/arm/pickup_adjust", msg);
    }

    public void ToggleInsertAdjust(bool state)
    {
        ros.Publish("/arm/insert_toggle", new BoolMsg(state));
        Debug.Log("TOGGLED " + state);
    }

    public void InsertAdjust(Vector3 transform)
    {
        PointMsg msg = new PointMsg();
        msg.x = transform.x;
        msg.y = transform.y;
        msg.z = transform.z;
        ros.Publish("/arm/insert_adjust", msg);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
