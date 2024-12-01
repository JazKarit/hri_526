using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using System.Diagnostics;
using System;

struct WipeData
{
    // Fields
    public string InterfaceType;
    public string Shape;
    public TimeSpan Time;
    public string Name;
    public int GoodPoints;
    public int BadPoints;
    

    // Constructor
    public WipeData(string interfaceType, string shape, TimeSpan time, string name, int goodPoints, int badPoints)
    {
        InterfaceType = interfaceType;
        Shape = shape;
        Time = time;
        Name = name;
        GoodPoints = goodPoints;
        BadPoints = badPoints;
    }

    // Method to display wipe information
    public string DisplayWipeInfo()
    {
       
        UnityEngine.Debug.Log($"Display Name: {Name}");
        UnityEngine.Debug.Log($"Interface Type: {InterfaceType}");
        UnityEngine.Debug.Log($"Shape: {Shape}");
        UnityEngine.Debug.Log($"Time: {Time.Hours} hours {Time.Minutes} minutes {Time.Seconds} seconds");
        UnityEngine.Debug.Log($"Good Points: {GoodPoints}");
        UnityEngine.Debug.Log($"Bad Points: {BadPoints}");
        string row = $"{Name} {InterfaceType} {Shape} {Time.Minutes} {Time.Seconds} {GoodPoints} {BadPoints}";
        return row;

    }

    public void Clear()
    {
        InterfaceType = "";
        Shape = "";
        Name = "name";
        GoodPoints = 0;
        BadPoints = 0;
    }
}

struct InsertData
{
    // Fields
    public string ConstraintType;
    public int PegSize;
    public string PegColor;
    public string Name;
    public TimeSpan Time;
    public int Collisions;
    public int Resets;
    public int Success;
    public int ConstraintModeToggles;

    // Constructor
    public InsertData(string constraintType, int pegSize, string pegColor, string name, TimeSpan time, int collisions, int resets, int success, int constraintModeToggles)
    {
        ConstraintType = constraintType;
        PegSize = pegSize;
        PegColor = pegColor;
        Name = name;
        Time = time;
        Collisions = collisions;
        Resets = resets;
        Success = success;
        ConstraintModeToggles = constraintModeToggles;
    }

    // Method to display insert information
    public string DisplayInsertInfo()
    {
        
        UnityEngine.Debug.Log($"Insert Name: {Name}");
        UnityEngine.Debug.Log($"Constraint Type: {ConstraintType}");
        UnityEngine.Debug.Log($"Peg Size: {PegSize}");
        UnityEngine.Debug.Log($"Peg Color: {PegColor}");
        UnityEngine.Debug.Log($"Time: {Time.Hours} hours {Time.Minutes} minutes {Time.Seconds} seconds");
        UnityEngine.Debug.Log($"Collisions: {Collisions}");
        UnityEngine.Debug.Log($"Resets: {Resets}");
        UnityEngine.Debug.Log($"Success: {Success}");
        UnityEngine.Debug.Log($"Constraint Mode Toggles: {ConstraintModeToggles}");
        string row = $"{Name} {ConstraintType} {Time.Minutes} {Time.Seconds} {Collisions} {Resets} {Success} {ConstraintModeToggles}";
       return row;
    }

    public void Clear()
    {
        ConstraintType = "";
        PegSize = 0;
        PegColor = "";
        Name = "";
        Collisions = 0;
        Resets = 0;
        Success = 0;
        ConstraintModeToggles = 0;
    }
}

public class TaskManager : MonoBehaviour
{
    public enum TaskState
    {
        IDLE,
        WIPE_POLYGON,
        WIPE_MANUAL,
        INSERT
    }

    ROSConnection ros;
    public TaskState state = TaskState.IDLE;
    public MotionConstrainer motionConstrainer;
    public GameObject wipeSquare;
    public GameObject wipePentagon;
    public GameObject wipeL;
    public DirtPointManager squarePointManager;
    public DirtPointManager pentagonPointManager;
    public DirtPointManager lPointManager;
    public GameObject positionSquare;
    public GameObject positionPentagon;
    public GameObject positionL;
    public GameObject EEF;
    public GameObject pegBox;
    public GameObject wipeSurface;

    private Stopwatch sw;

    private List<WipeData> wipes;
    private List<InsertData> inserts;

    private WipeData currWipe;
    private InsertData currInsert;



    // Start is called before the first frame update
    void Start()
    {
        this.ros = ROSConnection.GetOrCreateInstance();
        wipes = new List<WipeData>();
        inserts = new List<InsertData>();

        sw = new Stopwatch();
        ROSConnection.GetOrCreateInstance().Subscribe<StringMsg>("unity/commands", CommandRecived);
        ROSConnection.GetOrCreateInstance().RegisterPublisher<StringMsg>("unity/metric");

        wipeSquare.SetActive(false);
        wipePentagon.SetActive(false);
        wipeL.SetActive(false);
        EEF.SetActive(false);
        pegBox.SetActive(false);
        wipeSurface.SetActive(false);

        //CommandRecived(new StringMsg("manual"));
        // CommandRecived(new StringMsg("stop"));
    }

    public void CommandRecived(StringMsg msg)
    {

        // manual -> insert manual
        // wipe manual <shape> -> 
        // wipe constrained <shape>
        // stop
        // c
        // r
        string[] args = msg.data.Split(' ');
        //Debug.Log(msg.data);

        
        //Sorry about this v
        if (args[0].Equals("c")) {
            currInsert.Collisions++;
            UnityEngine.Debug.Log("bump");
            return;
        }

         if (args[0].Equals("r")) {
            currInsert.Resets++;
            return;
        }

        bool sendData = true;

        if (args[0].Equals("stop")) goto Stop;
        if (args[0].Equals("discard")) goto Discard;
        //Start

        if (args[0].Equals("wipe")) goto Wipe;
        //Insertion
        if (args[1].Equals("manual")) goto Insertion_Manual;


        //Automatic insertion
        motionConstrainer.constrained = true;
        currInsert.ConstraintType = "constrained";
        goto Insertion_Common;

    Insertion_Manual:
        //Manual Insertion
        motionConstrainer.constrained = false;
        currInsert.ConstraintType = "manual";

    Insertion_Common:
        sw.Restart();
        EEF.SetActive(true);
        pegBox.SetActive(true);
        state = TaskState.INSERT;
        return;

    Wipe:
        sw.Restart();
        DirtPointManager.WipePointMode wm;
        if (args[1].Equals("manual")) goto Wipe_Manual;
        //Poly wipe
        currWipe.InterfaceType = "constrained";
        wipeSurface.SetActive(true);
        WipePoint.uiPoint.SetActive(true);
        state = TaskState.WIPE_POLYGON;

        wm = DirtPointManager.WipePointMode.SURFACE;
        goto Wipe_General;
    Wipe_Manual:
        EEF.SetActive(true);
        currWipe.InterfaceType = "manual";
        motionConstrainer.constrained = false;
        wm = DirtPointManager.WipePointMode.EEF;
        state = TaskState.WIPE_MANUAL;
    Wipe_General:
        switch (args[2])
        {
            case "square":
                currWipe.Shape = "square";
                wipeSquare.SetActive(true);
                wipeSquare.transform.position = positionSquare.transform.position;
                wipeSquare.transform.rotation = positionSquare.transform.rotation;
                wipeSquare.GetComponent<DirtPointManager>().mode = wm;
                wipeSquare.GetComponent<DirtPointManager>().Unwipe();
                break;
            case "pentagon":
                currWipe.Shape = "pentagon";
                wipePentagon.SetActive(true);
                wipePentagon.transform.position = positionPentagon.transform.position;
                wipePentagon.transform.rotation = positionPentagon.transform.rotation;
                wipePentagon.GetComponent<DirtPointManager>().mode = wm;
                wipePentagon.GetComponent<DirtPointManager>().Unwipe();
                
                break;
            case "l":
                currWipe.Shape = "L";
                wipeL.SetActive(true);
                wipeL.transform.position = positionL.transform.position;
                wipeL.transform.rotation = positionL.transform.rotation;
                wipeL.GetComponent<DirtPointManager>().mode = wm;
                wipeL.GetComponent<DirtPointManager>().Unwipe();
                
                break;
        }
        return;


    Discard:
    sendData = false;
    Stop:
        sw.Stop();
        UnityEngine.Debug.Log(state);
        switch (state)
        {
            case TaskState.IDLE:

                break;
            case TaskState.WIPE_MANUAL:

                wipeL.SetActive(false);
                wipeSquare.SetActive(false);
                wipePentagon.SetActive(false);
                EEF.SetActive(false);

                if (currWipe.Shape == "square")
                {
                    currWipe.GoodPoints = squarePointManager.GetNumWiped();
                    currWipe.BadPoints = squarePointManager.GetAntiparticlesWiped();
                } else if (currWipe.Shape == "pentagon")
                {
                    currWipe.GoodPoints = pentagonPointManager.GetNumWiped();
                    currWipe.BadPoints = pentagonPointManager.GetAntiparticlesWiped();
                } else if (currWipe.Shape == "L")
                {
                    currWipe.GoodPoints = lPointManager.GetNumWiped();
                    currWipe.BadPoints = lPointManager.GetAntiparticlesWiped();
                }

                currWipe.Time = sw.Elapsed;
                if (sendData) ros.Publish("unity/metric", new StringMsg(currWipe.DisplayWipeInfo()));
                currWipe.Clear();
                break;
            case TaskState.WIPE_POLYGON:

                wipeL.SetActive(false);
                wipeSquare.SetActive(false);
                wipePentagon.SetActive(false);
                wipeSurface.GetComponent<MeshFilter>().mesh.Clear();
                wipeSurface.SetActive(false);

                foreach (GameObject wipePoint in WipePoint.points)
                {
                    Destroy(wipePoint);
                }
                WipePoint.points.Clear();
                WipePoint.uiPoint.SetActive(false);

                if (currWipe.Shape == "square")
                {
                    currWipe.GoodPoints = squarePointManager.GetNumWiped();
                    currWipe.BadPoints = squarePointManager.GetAntiparticlesWiped();
                } else if (currWipe.Shape == "pentagon")
                {
                    currWipe.GoodPoints = pentagonPointManager.GetNumWiped();
                    currWipe.BadPoints = pentagonPointManager.GetAntiparticlesWiped();
                } else if (currWipe.Shape == "L")
                {
                    currWipe.GoodPoints = lPointManager.GetNumWiped();
                    currWipe.BadPoints = lPointManager.GetAntiparticlesWiped();
                }
                currWipe.Time = sw.Elapsed;
                if (sendData) ros.Publish("unity/metric", new StringMsg(currWipe.DisplayWipeInfo()));
                currWipe.Clear();

                break;
            case TaskState.INSERT:
                EEF.SetActive(false);
                pegBox.SetActive(false);
                currInsert.ConstraintModeToggles = motionConstrainer.GetNumToggles();
                currInsert.Time = sw.Elapsed;
                
                if (sendData) ros.Publish("unity/metric", new StringMsg(currInsert.DisplayInsertInfo()));
                inserts.Add(currInsert);
                currInsert.Clear();
                motionConstrainer.startNone();
                break;
        }
        state = TaskState.IDLE;
        //Debug.Log("stop");


    }

    // Update is called once per frame
    void Update()
    {

    }
}
