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
    public void DisplayWipeInfo()
    {
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Interface Type: {InterfaceType}");
        Console.WriteLine($"Shape: {Shape}");
        Console.WriteLine($"Time: {Time.Hours} hours {Time.Minutes} minutes {Time.Seconds} seconds");
        Console.WriteLine($"Good Points: {GoodPoints}");
        Console.WriteLine($"Bad Points: {BadPoints}");
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
    public void DisplayInsertInfo()
    {
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Constraint Type: {ConstraintType}");
        Console.WriteLine($"Peg Size: {PegSize}");
        Console.WriteLine($"Peg Color: {PegColor}");
        Console.WriteLine($"Time: {Time.Hours} hours {Time.Minutes} minutes {Time.Seconds} seconds");
        Console.WriteLine($"Collisions: {Collisions}");
        Console.WriteLine($"Resets: {Resets}");
        Console.WriteLine($"Success: {Success}");
        Console.WriteLine($"Constraint Mode Toggles: {ConstraintModeToggles}");
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
    public TaskState state = TaskState.IDLE;
    public MotionConstrainer motionConstrainer;
    public GameObject wipeSquare;
    public GameObject wipePentagon;
    public GameObject wipeL;
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
        wipes = new List<WipeData>();
        inserts = new List<InsertData>();

        sw = new Stopwatch();
        ROSConnection.GetOrCreateInstance().Subscribe<StringMsg>("unity/commands", CommandRecived);

        wipeSquare.SetActive(false);
        wipePentagon.SetActive(false);
        wipeL.SetActive(false);
        EEF.SetActive(false);
        pegBox.SetActive(false);
        wipeSurface.SetActive(false);

    }

    public void CommandRecived(StringMsg msg)
    {
        string[] args = msg.data.Split(' ');
        //Debug.Log(msg.data);


        //Sorry about this v

        if (args[0].Equals("stop")) goto Stop;
        //Start

        if (args[0].Equals("wipe")) goto Wipe;
        //Insertion
        if (args[0].Equals("manual")) goto Insertion_Manual;

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
        wipeSurface.SetActive(true);
        WipePoint.uiPoint.SetActive(true);
        state = TaskState.WIPE_POLYGON;

        wm = DirtPointManager.WipePointMode.SURFACE;
        goto Wipe_General;
    Wipe_Manual:
        EEF.SetActive(true);
        motionConstrainer.constrained = false;
        wm = DirtPointManager.WipePointMode.EEF;
        state = TaskState.WIPE_MANUAL;
    Wipe_General:
        switch (args[2])
        {
            case "square":
                wipeSquare.SetActive(true);
                wipeSquare.transform.position = positionSquare.transform.position;
                wipeSquare.GetComponent<DirtPointManager>().Unwipe();
                wipeSquare.GetComponent<DirtPointManager>().mode = wm;
                break;
            case "pentagon":
                wipePentagon.SetActive(true);
                wipePentagon.transform.position = positionPentagon.transform.position;
                wipePentagon.GetComponent<DirtPointManager>().Unwipe();
                wipePentagon.GetComponent<DirtPointManager>().mode = wm;
                break;
            case "l":
                wipeL.SetActive(true);
                wipeL.transform.position = positionL.transform.position;
                wipeL.GetComponent<DirtPointManager>().Unwipe();
                wipeL.GetComponent<DirtPointManager>().mode = wm;
                break;
        }
        return;

    Stop:
        sw.Stop();
        switch (state)
        {
            case TaskState.IDLE:

                break;
            case TaskState.WIPE_MANUAL:

                wipeL.SetActive(false);
                wipeSquare.SetActive(false);
                wipePentagon.SetActive(false);
                EEF.SetActive(false);

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

                break;
            case TaskState.INSERT:
                EEF.SetActive(false);
                pegBox.SetActive(false);
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
