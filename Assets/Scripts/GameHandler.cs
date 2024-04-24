using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
//using RosMessageTypes.Scripts;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class GameHandler : MonoBehaviour
{

    public GameObject target;
    public GameObject table;
    public GameObject contender;
    public GameObject contenderSelector;
    public bool running;
    public float trialLength = 60.0f;
    public float elapsed = 0.0f;
    public GameObject pointer;
    public GameObject armOrigin;
    public GameObject robotBase;
    public GameObject blueCup;
    public GameObject grabAdjuster;

    public bool active = false;
    public bool watchingCup = false;

    public bool testButton = false;

    public bool testFlag;

    public GameObject pourDial;
    public GameObject gripper;

    public GameObject pourAdjuster;


    //Statistics (public for debugging)

    public int placments;
    public List<float> errors;

    public static GameHandler instance;

    ROSConnection ros;
    

    // Start is called before the first frame update
    void Start()
    {

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<StringMsg>("/statistics");
        ros.RegisterPublisher<Float32Msg>("/arm/pour");
        ros.RegisterPublisher<BoolMsg>("/arm/pour_toggle");

        if (instance != null)
        {
            Debug.Log("Singleton error game manager");
        }
        instance = this;
        pointer.SetActive(false);
        ROSConnection.GetOrCreateInstance().RegisterPublisher<PoseMsg>("arm/go_to");
        ROSConnection.GetOrCreateInstance().RegisterPublisher<PoseArrayMsg>("arm/pickplace");
        ROSConnection.GetOrCreateInstance().RegisterPublisher<PoseArrayMsg>("arm/pickhold");
        ROSConnection.GetOrCreateInstance().Subscribe<StringMsg>("/unity/commands", CommandRecived);
    }

    public void CommandRecived(StringMsg msg)
    {
        switch (msg.data)
        {
            case "blue_cup grabbed":
                pourAdjuster.SetActive(true);
                break;
            case "arm hover":
                grabAdjuster.GetComponent<GrabAdjuster>().Begin();
                break;
        }
    }

    public void StartTrial()
    {
        elapsed = 0;
        placments = 0;
        errors.Clear();
        running = true;
        PlaceTarget();
        ChooseContender();
    }

    public void FinishTrial() {
        target.SetActive(false);
        pointer.SetActive(false);
        contenderSelector = null;
        contender = null;
        float avgError = 0.0f;
        float minError = float.MaxValue;
        float maxError = float.MinValue;

        foreach (var e in errors)
        {
            if (e < minError) minError = e;
            if (e > maxError) maxError = e;
            avgError += e;
        }
        avgError /= errors.Count;
        errors.Sort();
        if (errors.Count < 1) ;
        float medError = errors[errors.Count / 2];
        string statsString = "";
        statsString += "placements: " + placments;
        statsString += ",  avg error: " + avgError;
        statsString += ",  med error: " + medError;
        statsString += ",  min error: " + minError;
        statsString += ",  max error: " + maxError;
        StringMsg stats = new StringMsg(statsString);
        Debug.Log(statsString);
        ros.Publish("/statistics", stats);
        running = false;
    }

    public void PlaceTarget()
    {
        float newX = Random.Range(table.GetComponent<Renderer>().bounds.min.x, table.GetComponent<Renderer>().bounds.max.x);
        float newZ = Random.Range(table.GetComponent<Renderer>().bounds.min.z, table.GetComponent<Renderer>().bounds.max.z);
        target.transform.position = new Vector3(newX, target.transform.position.y, newZ);
        target.SetActive(true);
    }

    public void ChooseContender()
    {
        List<TFManager.TFObject> options = new List<TFManager.TFObject>();
        foreach (var obj in TFManager.instance.subscribers)
        {
            if (obj.selectable)
            {
                options.Add(obj);
            }
        }
        int index = (int)Random.Range(0, options.Count);
        contender = options[index].gameObject;
        contenderSelector = options[index].selector;
        pointer.transform.position = new Vector3(contender.transform.position.x, contender.transform.position.y + contender.GetComponent<Renderer>().bounds.extents.y + 0.15f, contender.transform.position.z);
        pointer.SetActive(true);
        
    }

    public void PickPlace(Vector3 source, Vector3 target)
    {
        Vector3 relTarget = armOrigin.transform.InverseTransformPoint(target);
        Vector3 rosTarget = new Vector3(relTarget.x, relTarget.z, relTarget.y + 0.05f);

        Vector3 relSource = armOrigin.transform.InverseTransformPoint(source);
        Vector3 rosSource = new Vector3(relSource.x, relSource.z, relSource.y + 0.05f);
        Debug.Log("pickplace");
        ROSPublisher.instance.DoublePose("arm/pickplace", rosSource, rosTarget, new Quaternion(), new Quaternion());
    }

    public void PickHold(Vector3 source, Vector3 target)
    {
        Vector3 relTarget = armOrigin.transform.InverseTransformPoint(target);
        Vector3 rosTarget = new Vector3(relTarget.x, relTarget.z, relTarget.y + 0.05f);

        Vector3 relSource = armOrigin.transform.InverseTransformPoint(source);
        Vector3 rosSource = new Vector3(relSource.x, relSource.z, relSource.y + 0.05f);
        Debug.Log("pickhold");
        ROSPublisher.instance.DoublePose("arm/pickhold", rosSource, rosTarget, new Quaternion(), new Quaternion());
    }



    public void PlaceObject(Vector3 location)
    {
        Debug.Log("placed");
        if (!running)
        {
            Debug.Log("real");
            Vector3 relLocation = armOrigin.transform.InverseTransformPoint(location);
            Debug.Log(relLocation.x + ", "+ relLocation.y + ", " + relLocation.z);
            ROSPublisher.instance.Pose("arm/go_to", new Vector3(relLocation.x, relLocation.z, relLocation.y + 0.10f), new Quaternion());
            return;
        }
        float tx = target.transform.position.x;
        float tz = target.transform.position.z;
        float ox = location.x;
        float oz = location.z;
        float error = Mathf.Sqrt(((tx - ox) * (tx - ox)) + ((tz - oz) * (tz - oz)));
        errors.Add(error);
        placments++;
        PlaceTarget();
        ChooseContender();
    }

    public void SpeechEnough()
    {
        RobotActions.instance.state = RobotActions.RobotState.IDLE;
        PourToggle(false);
    }

    public void SpeechPour()
    {
        PourToggle(true);
    }

    public void PourSet(float angle)
    {
        Debug.Log("Pouring: " + angle);
        ros.Publish("/arm/pour", new Float32Msg(angle));
    }

    public void PourToggle(bool toggle)
    {
        ros.Publish("/arm/pour_toggle", new BoolMsg(toggle));
        pourDial.SetActive(toggle);
        if (toggle)
        {
            pourDial.transform.position = gripper.transform.position;
            pourDial.transform.eulerAngles = new Vector3(0, gripper.transform.eulerAngles.y, pourDial.transform.eulerAngles.z);
        }
    }

    

    public void SpeachTask()
    {
        active = true;
        RobotActions.instance.state = RobotActions.RobotState.PICK_ON_MOVE;
        watchingCup = true;
    }

    public void objectStop()
    {
        Debug.Log("stopped");

    }


    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            elapsed += Time.deltaTime;
            if (elapsed > trialLength)
            {
                FinishTrial();
            }
        }
        if (testFlag)
        {
            //StartTrial();
            grabAdjuster.GetComponent<GrabAdjuster>().Begin();
            testFlag = false;
        }

        if (testButton)
        {
            SpeechPour();
            testButton = false;
        }


    }
}
