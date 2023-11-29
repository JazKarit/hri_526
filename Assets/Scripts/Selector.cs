using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{




    public GameObject real;

    public float matchTreshold = 0.05f;

    public bool pinched = false;
    public bool showing = false;

    public bool keepAboveTable = true;

    private bool tracking = false;

    public GameObject menu;

    public Material digitalTwinMat;
    public Material digitalTwinSelectedMat;


    public static GameObject selectedObject = null;


    // Start is called before the first frame update
    void Start()
    {
        this.Hide();

    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, real.transform.position);
        if (!pinched && dist < matchTreshold)
        {
            this.Cancel();
        }

        if (showing)
        {
            GameObject table = GameObject.Find("Table");
            float midOffset = GetComponent<Renderer>().bounds.extents.y;
            if (transform.position.y - midOffset < table.transform.position.y)
            {
                transform.position = new Vector3(transform.position.x, table.transform.position.y + midOffset, transform.position.z);
            }
            //Update Line
            GetComponent<LineRenderer>().SetPosition(0, real.transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);

            if (tracking)
            {
                TaskStateManager.instance.GetComponent<TaskStateManager>().digitalTwinObject = gameObject;
                //TaskStateManager.instance.GetComponent<TaskStateManager>().SendTargetPosition();
            }

        }else
        {
            transform.position = real.transform.position;
            transform.rotation = real.transform.rotation;
        }
        
    }

    public void SetSelected(bool state)
    {
        if (state)
        {
            if (selectedObject != null)
            {
                selectedObject.GetComponent<Selector>().SetSelected(false);
            }
            selectedObject = gameObject;
            real.GetComponent<Renderer>().material = digitalTwinSelectedMat;
        }
        else
        {
            if (selectedObject == gameObject)
            {
                selectedObject = null;
            }
            real.GetComponent<Renderer>().material = digitalTwinMat;
        }
    }

    //helper function
    private StateManager.SelectorType getInterface()
    {
        return StateManager.instance.selectorType;
    }

    public void Cancel()
    {
        this.Hide();
        transform.position = real.transform.position;
        transform.rotation = real.transform.rotation;
    }

    public void Lock()
    {
        TFManager.instance.LockSelectors(gameObject);
        StateManager.instance.lockInterface = true; // Do not allow interface changing during selector actions for saftey
    }

    public void Unlock()
    {
        TFManager.instance.UnlockSelectors();
        StateManager.instance.lockInterface = false;
    }

    public void StartManLong()
    {
        
        
        if (this.getInterface() == StateManager.SelectorType.Drag || this.getInterface() == StateManager.SelectorType.DragSelect)
        {
            this.pinched = true;

            this.Show();
            this.tracking = true;
        }
        if (this.getInterface() == StateManager.SelectorType.PointSelect || this.getInterface() == StateManager.SelectorType.Gaze)
        {
            this.Cancel();
            if (selectedObject == gameObject)
            {
                this.SetSelected(false);
                menu.GetComponent<SelectorMenu>().Cancel();
                this.Unlock();
            } else
            {
                this.SetSelected(true);
                
            }
        }
    }

    public void EndManLong()
    {
        this.pinched = false;
        
        if (this.getInterface() == StateManager.SelectorType.DragSelect)
        {
            this.FocusMenu();
        }
            if (this.getInterface() == StateManager.SelectorType.Drag)
        {
            this.tracking = false;
            this.Cancel();
        }
        this.Unlock();
    }

    public void FocusMenu()
    {
        menu.GetComponent<SelectorMenu>().selector = this.gameObject;
        menu.GetComponent<SelectorMenu>().AppearOnFace();
    }

    public void Show()
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<LineRenderer>().enabled = true;
        this.showing = true;
    }

    public void Hide()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<LineRenderer>().enabled = false;
        this.showing = false;
    }


}
