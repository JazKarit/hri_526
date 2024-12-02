# Set up Colocated Workspace in Unity
Install this project and open in Unity editor.

## ROS
After installing the [Unity Ros Hub](https://github.com/Unity-Technologies/ROS-TCP-Endpoint) configure the IP of your ROS computer in the Robotics setting tab.

![image](https://github.com/dsaliba/hololens_unity_workspace/assets/69019487/4bb33d52-c9f0-4be7-9243-128d33f9958e)


## Calibration
Similar to the parent repo a menu can be summoned by raising your left hand. Uppon pressing the create anchor button the project will begin searching for the calibration QR code. Once found a frame will be drawn on the QR code. If this position is correct press the "Create Anchor" button. The position will now be cached for the remainder of this session.

1. Publish the command "insert manual" to the /unity/commands topic to put the system into manual insertion mode.
2. Place the peg box in view of cameras
3. With the ROS nodes open, use WASDQE to align the box hologram with the peg box
4. With the ROS nodes open use JKLIUO to align the robot frames with the locobot

### Calibrating End Effector Teleop
1. Publish the command "insert manual" to the /unity/commands topic to put the system into manual insertion mode.
2. Use the voice control "sync" to align the end effector controller with the robot gripper.
3. Use the voice command "manual" to arm teleop control.
4. Move the end effector away from the robot
5. Using the left hand menu press the calibration checkbox enter calibration mode
6. Align the end effector controller with the robot gripper
7. Using the left hand menu press the calibration checkbox exit calibration mode
8. Use the voice command "manual" to rearm teleop control

The system is now fully calibrated.


## Creating Tracked Objects
![image](https://github.com/dsaliba/hololens_unity_workspace/assets/69019487/340a50ef-4c94-4880-800c-13216e7e6b9f)

1. Open an instance of the TFManager script in the inspector (*NOTE:* There should only be one instance of this script in the project)
2. Add a new subscriber to the subscriber list by clicking the + icon
3. Make the "Name" field the name of the frame ID provided by the ROS message
4. Make the "Game Object" field the object of the model to be moved (If a selector is used make this object a paerent of both the model and selector objects)
5. *Optional* Make the "Selector" object a selector to be used for this object. After doing so check the "Selectable" box

The tracked object should now work automatically.

## Velocity Tracking Triggers
The VelocityTracker script can be used in order to invoke actions when an object stops moving. 
The "Scale" property adjusts the scale of the velocity vector drawn on the object. 
The "Thresh" property adjusts the theshold at which an object is considered stopped.

![image](https://github.com/dsaliba/hololens_unity_workspace/assets/69019487/afdb71fb-1f7e-4ff6-89b2-e029077ba30e)

The + icon under the OnStop list can be used to add unity events to invoke when the object is stopped. By adding a script with public methods to an object you can call the methods via this invoke list.






