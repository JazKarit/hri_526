using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

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

    public bool testFlag;


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

        if (instance != null)
        {
            Debug.Log("Singleton error game manager");
        }
        instance = this;
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
        string statsString = "";
        statsString += "placements: " + placments;
        statsString += ",  avg error: " + avgError;
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

    public void PlaceObject(Vector3 location)
    {
        if (!running) return;
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
            StartTrial();
            testFlag = false;
        }
    }
}
