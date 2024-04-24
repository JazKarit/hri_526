using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class Cursor : MonoBehaviour
{

    public GameObject menu;
    public GameObject gazeIndicator;

    public static Cursor instance;
    // Start is called before the first frame update
    void Start()
    {
        this.Hide();
        if (instance != null)
        {
            Debug.Log("Singleton bug");
        }
        instance = this;
    }

    private StateManager.SelectorType getInterface()
    {
        return StateManager.instance.selectorType;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.getInterface() == StateManager.SelectorType.Gaze || this.getInterface() == StateManager.SelectorType.GazeSelect)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hitInfo,
                    20.0f,
                    Physics.DefaultRaycastLayers))
            {
                // If the Raycast has succeeded and hit a hologram
                // hitInfo's point represents the position being gazed at
                // hitInfo's collider GameObject represents the hologram being gazed at
                gazeIndicator.transform.position = hitInfo.point;
                gazeIndicator.SetActive(true);
            }else
            {
                gazeIndicator.SetActive(false);
            }
        } else
        {
            gazeIndicator.SetActive(false);
        }
    }

    public void GazeClick()
    {
        if (this.getInterface() == StateManager.SelectorType.Gaze || this.getInterface() == StateManager.SelectorType.GazeSelect)
        {
            Debug.Log("Voice Select");
            RaycastHit hitInfo;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hitInfo,
                    20.0f,
                    Physics.DefaultRaycastLayers))
            {

                if (hitInfo.collider.gameObject.GetComponent<Selector>() != null)
                {
                    hitInfo.collider.gameObject.GetComponent<Selector>().StartManLong();
                } else if (Selector.selectedObject != null)
                {
                    
                    gameObject.transform.position = hitInfo.point;
                    GetComponent<LineRenderer>().SetPosition(0, Selector.selectedObject.transform.position);
                    GetComponent<LineRenderer>().SetPosition(1, transform.position);

                    if (this.getInterface() == StateManager.SelectorType.Gaze) //Select or non select interface
                    {
                        menu.GetComponent<SelectorMenu>().selector = this.gameObject;
                        SelectorMenu.instance.MoveAndDropForce();
                        this.Hide();
                    }
                    else
                    {
                        this.Show();
                        this.FocusMenu();
                    }
                    
                }
                // If the Raycast has succeeded and hit a hologram
                // hitInfo's point represents the position being gazed at
                // hitInfo's collider GameObject represents the hologram being gazed at
                
            }
        }
        
    }

    public void Cancel()
    {
        this.Hide();
    }

    public void Show()
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<LineRenderer>().enabled = true;
    }

    public void Hide()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<LineRenderer>().enabled = false;
    }

    public void FocusMenu()
    {
        menu.GetComponent<SelectorMenu>().selector = this.gameObject;
        menu.GetComponent<SelectorMenu>().AppearOnFace();
    }

    public void TableTap()
    {
        //Dont make cursor if there isnt an active selection
        if (Selector.selectedObject == null || (this.getInterface() != StateManager.SelectorType.Point && this.getInterface() != StateManager.SelectorType.PointSelect))
        {
            return;
        }
        foreach (var source in MixedRealityToolkit.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == Microsoft.MixedReality.Toolkit.Input.InputSourceType.Hand)
            {
                foreach (var p in source.Pointers)
                {
                    if (p is IMixedRealityNearPointer)
                    {
                        // Ignore near pointers, we only want the rays
                        continue;
                    }
                    if (p.Result != null)
                    {
                        var startPoint = p.Position;
                        var endPoint = p.Result.Details.Point;
                        var hitObject = p.Result.Details.Object;
                        
                        if (hitObject)
                        {
                            
                            gameObject.transform.position = endPoint;
                            GetComponent<LineRenderer>().SetPosition(0, Selector.selectedObject.transform.position);
                            GetComponent<LineRenderer>().SetPosition(1, transform.position);
                            if (this.getInterface() == StateManager.SelectorType.Point) //Select or non select interface
                            {
                                menu.GetComponent<SelectorMenu>().selector = this.gameObject;
                                SelectorMenu.instance.MoveAndDropForce();
                                this.Hide();

                            }
                            else
                            {
                                this.Show();
                                this.FocusMenu();
                            }
                        }
                    }

                }
            }
        }
    }
}

