# Set up Colocated Workspace in Unity
Follow the steps in the [parent repository](https://github.com/lagenuina/hololens_unity_asa) to configure ASA before beginning these steps.

## ROS
After installing the [Unity Ros Hub](https://github.com/Unity-Technologies/ROS-TCP-Endpoint) configure the IP of your ROS computer in the Robotics setting tab.

![image](https://github.com/dsaliba/hololens_unity_workspace/assets/69019487/4bb33d52-c9f0-4be7-9243-128d33f9958e)


## Calibration
Similar to the parent repo a menu can be summoned by raising your left hand. Uppon pressing the create anchor button the project will begin searching for the calibration QR code. Once found a frame will be drawn on the QR code. If this position is correct press the "Create Anchor" button. The resulting anchor ID will be printed on the console.

![image](https://github.com/dsaliba/hololens_unity_workspace/assets/69019487/19903e54-48e5-452b-8a2f-93b443ff3325)


Feed this anchor ID into the "Anchor ID" field of the ASA Script.
Upon rebuilding the unity and ROS enviornments should be succesfully colocated.

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




