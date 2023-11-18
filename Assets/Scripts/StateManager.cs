using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using StringMessage = RosMessageTypes.Std.StringMsg;

public class StateManager : MonoBehaviour
{
    //Singleton
    public static StateManager instance;

    //State Info
    public SelectorType selectorType = SelectorType.DragSelect;
    public bool showLables = true;

    //Registrations
    public GameObject[] lables;


    public enum SelectorType
    {
        Drag,
        Gaze,
        DragSelect,
        PointSelect
    }

    public bool lockInterface = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Spinning up singleton");
        ROSConnection.GetOrCreateInstance().Subscribe<StringMessage>("/statemanager/command", ReciveCommand);
        if (instance != null)
        {
            Debug.Log("Too many state managers im unhappy");
        }
        instance = this;
    }

    void ReciveCommand(StringMessage target)
    {
        Debug.Log(target.data);
        string[] args = target.data.ToLower().Split(' ');
        switch (args[0])
        {
            case "selector_type":
                if (lockInterface) break; //Do not allow chaning interface if this semaphore is true
                if (args[1].Equals("drag")) selectorType = SelectorType.Drag;
                if (args[1].Equals("gaze")) selectorType = SelectorType.Gaze;
                if (args[1].Equals("drag_select")) selectorType = SelectorType.DragSelect;
                if (args[1].Equals("point_select")) selectorType = SelectorType.PointSelect;
                break;

            case "hide":
                if (args[1].Equals("lables"))
                {
                    this.showLables = false;
                    foreach (GameObject lable in this.lables)
                    {
                        lable.SetActive(false);
                    }
                }
                break;
            case "show":
                if (args[1].Equals("lables"))
                {
                    this.showLables = true;
                    foreach (GameObject lable in this.lables)
                    {
                        lable.SetActive(true);
                    }
                }
                break;
            default:
                Debug.Log("Unrecognized command...");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
