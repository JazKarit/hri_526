using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public enum SelectorType
    {
        Drag,
        Gaze,
        PointnClick
    }

    public SelectorType mode = SelectorType.Drag;

    public GameObject real;

    public float matchTreshold = 0.05f;

    public bool pinched = false;
    public bool showing = false;

    public GameObject menu;

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
            //Update Line
            GetComponent<LineRenderer>().SetPosition(0, real.transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }
        
    }

    public void Cancel()
    {
        this.Hide();
        transform.position = real.transform.position;
        transform.rotation = real.transform.rotation;
    }

    public void StartManLong()
    {
        this.pinched = true;
        this.Show();
    }

    public void EndManLong()
    {
        this.pinched = false;
        this.FocusMenu();
    }

    public void FocusMenu()
    {
        menu.GetComponent<SelectorMenu>().selector = this.gameObject;
        menu.GetComponent<SelectorMenu>().ApearAboveSelector();
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
