using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class SelectorMenu : MonoBehaviour
{

    public GameObject selector;
    public GameObject taskStateManager;
    public GameObject cursor;

    //helper function
    private StateManager.SelectorType getInterface()
    {
        return StateManager.instance.selectorType;
    }


    // Start is called before the first frame update
    void Start()
    {
        this.Disappear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  

    public void MoveAndHold()
    {
        if (selector != null)
        {
            TaskStateManager.instance.GetComponent<TaskStateManager>().digitalTwinObject = selector;
            TaskStateManager.instance.GetComponent<TaskStateManager>().SendTargetPosition();
        }
        this.Cancel();
        this.Disappear();
    }

    public void Cancel()
    {
       if (selector != null &&  !(this.getInterface() == StateManager.SelectorType.PointSelect || this.getInterface() == StateManager.SelectorType.Gaze))
        {
            selector.GetComponent<Selector>().Cancel();
        } else
        {
            if (this.getInterface() == StateManager.SelectorType.PointSelect || this.getInterface() == StateManager.SelectorType.Gaze)
            {
                selector.GetComponent<Cursor>().Cancel();
            }
            
        }
    }

    public void Disappear()
    {
        gameObject.SetActive(false);
    }

    public void AppearOnFace()
    {
        gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        Vector3 cameraEulerAngles = Camera.main.transform.rotation.eulerAngles;
        Quaternion desiredRotation = Quaternion.Euler(0, cameraEulerAngles.y, 0);
        gameObject.transform.rotation = desiredRotation;
        gameObject.SetActive(true);
    }

    public void ApearAboveSelector()
    {
        
        gameObject.transform.position = new Vector3(selector.transform.position.x, selector.transform.position.y + 0.1f, +selector.transform.position.z);
        Vector3 cameraEulerAngles = Camera.main.transform.rotation.eulerAngles;
        Quaternion desiredRotation = Quaternion.Euler(cameraEulerAngles.x, cameraEulerAngles.y, cameraEulerAngles.z);
        gameObject.transform.rotation = desiredRotation;
        gameObject.SetActive(true);
    }
}
