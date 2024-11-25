using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wipeable : MonoBehaviour
{
    public enum WipePointMode {
        DISABLED,
        EEF,
        SURFACE
    };
    public GameObject eef;
    public GameObject wipeSurface;
    public GameObject model;
    public Material unwipedMat;
    public Material wipedMat;
    public bool wiped;
    public float threshhold = 0.5f;
    public WipePointMode mode;
    

    // Start is called before the first frame update
    void Start()
    {
        eef = GameObject.Find("/EEF Controller");
        wipeSurface = GameObject.Find("/ASA/Located Anchor/Wipe Surface");
        model.GetComponent<Renderer>().material = unwipedMat;
    }

    // Update is called once per frame
    void Update()
    {
        switch(mode) {
            case WipePointMode.DISABLED:
                break;
            case WipePointMode.EEF:
                if (Vector3.Distance(transform.position, eef.transform.position) < threshhold) {
                    SetWiped(true);
                }
                break;
            case WipePointMode.SURFACE:
                for (int i = 0; i < wipeSurface.GetComponent<MeshFilter>().mesh.triangles.Length; i+=3)
                {
                    var trianglePt1 = (wipeSurface.transform.rotation * wipeSurface.GetComponent<MeshFilter>().mesh.vertices[wipeSurface.GetComponent<MeshFilter>().mesh.triangles[i]]) * wipeSurface.transform.localScale[0] + wipeSurface.transform.position;
                    var trianglePt2 = (wipeSurface.transform.rotation * wipeSurface.GetComponent<MeshFilter>().mesh.vertices[wipeSurface.GetComponent<MeshFilter>().mesh.triangles[i+1]]) * wipeSurface.transform.localScale[0] + wipeSurface.transform.position;
                    var trianglePt3 = (wipeSurface.transform.rotation * wipeSurface.GetComponent<MeshFilter>().mesh.vertices[wipeSurface.GetComponent<MeshFilter>().mesh.triangles[i+2]]) * wipeSurface.transform.localScale[0] + wipeSurface.transform.position;
                    var distance = DistanceToTriangle.GetDistanceToTriangle(transform.position, trianglePt1, trianglePt2, trianglePt3);

                    if (distance < threshhold)
                    {
                        SetWiped(true);
                    }
                }
                break;
        }
    }

    void SetWiped(bool wiped)
    {
        if (this.wiped != wiped)
        {
            model.GetComponent<Renderer>().material = wiped ? wipedMat : unwipedMat;
        }
    }

    void SetWipeModeDiasabled()
    {
        mode = WipePointMode.DISABLED;
    }

    void SetWipeModeEEF()
    {
        mode = WipePointMode.EEF;
    }

    void SetWipeModeSurface()
    {
        mode = WipePointMode.SURFACE;
    }
}
