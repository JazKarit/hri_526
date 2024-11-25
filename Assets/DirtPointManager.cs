using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DirtPointManager : MonoBehaviour
{
    public GameObject dirtParent;
    public GameObject antiParticleParent;
    public enum WipePointMode {
        DISABLED,
        EEF,
        SURFACE
    };
    public GameObject eef;
    public GameObject wipeSurface;
    public Material unwipedMat;
    public Material wipedMat;
    public float threshhold = 0.5f;
    public WipePointMode mode;
    private bool[] isWiped;
    private bool[] antiParticleIsWiped;
    public int jumpFactor;
    private int offset = 0;


    // Start is called before the first frame update
    void Start()
    {
        isWiped = new bool[dirtParent.transform.childCount];
        // eef = GameObject.Find("/EEF Controller");
        // wipeSurface = GameObject.Find("/Wipe Surface");
    }

    // Update is called once per frame
    void Update()
    {
        if (mode != WipePointMode.DISABLED) {

            for (int i = 0; i < dirtParent.transform.childCount; i++) {
                GameObject dirt = dirtParent.transform.GetChild(i).gameObject;
            }
        }

        switch(mode) {
            case WipePointMode.DISABLED:
                break;
            case WipePointMode.EEF:
                for (int i = offset; i < dirtParent.transform.childCount; i+=jumpFactor) {
                    GameObject dirt = dirtParent.transform.GetChild(i).gameObject;
                    if (Vector3.Distance(dirt.transform.position, eef.transform.position) < threshhold) {
                        SetWiped(i, true, dirt);
                    } 
                }
                for (int i = offset; i < antiParticleParent.transform.childCount; i+=jumpFactor) {
                    GameObject antiParticle = antiParticleParent.transform.GetChild(i).gameObject;
                    if (Vector3.Distance(antiParticle.transform.position, eef.transform.position) < threshhold) {
                        antiParticleIsWiped [i] = true;
                    } 
                }   
                break;
            case WipePointMode.SURFACE:
                for (int i = 0; i < wipeSurface.GetComponent<MeshFilter>().mesh.triangles.Length; i+=3)
                {
                    var trianglePt1 = (wipeSurface.transform.rotation * wipeSurface.GetComponent<MeshFilter>().mesh.vertices[wipeSurface.GetComponent<MeshFilter>().mesh.triangles[i]]) * wipeSurface.transform.localScale[0] + wipeSurface.transform.position;
                    var trianglePt2 = (wipeSurface.transform.rotation * wipeSurface.GetComponent<MeshFilter>().mesh.vertices[wipeSurface.GetComponent<MeshFilter>().mesh.triangles[i+1]]) * wipeSurface.transform.localScale[0] + wipeSurface.transform.position;
                    var trianglePt3 = (wipeSurface.transform.rotation * wipeSurface.GetComponent<MeshFilter>().mesh.vertices[wipeSurface.GetComponent<MeshFilter>().mesh.triangles[i+2]]) * wipeSurface.transform.localScale[0] + wipeSurface.transform.position;
                
                    for (int j = 0; j < dirtParent.transform.childCount; j++) {
                        GameObject dirt = dirtParent.transform.GetChild(j).gameObject;
                        var distance = DistanceToTriangle.GetDistanceToTriangle(dirt.transform.position, trianglePt1, trianglePt2, trianglePt3);
                        if (distance < threshhold)
                        {
                            SetWiped(j, true, dirt);
                        }
                        else
                        {
                            SetWiped(j, false, dirt);
                        }
                    }    

                    for (int j = 0; j < antiParticleParent.transform.childCount; j++) {
                        GameObject dirt = antiParticleParent.transform.GetChild(j).gameObject;
                        var distance = DistanceToTriangle.GetDistanceToTriangle(antiParticleParent.transform.position, trianglePt1, trianglePt2, trianglePt3);
                        if (distance < threshhold)
                        {
                            antiParticleIsWiped[i] = true;
                        }
                        else
                        {
                            antiParticleIsWiped [i] = false;
                        }
                    }      
                }
                break;
        }
        offset = (offset+1)%jumpFactor;
    }

    void SetWiped(int i, bool wiped, GameObject dirt)
    {
        if (isWiped[i] != wiped)
        {
            dirt.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = wiped ? wipedMat : unwipedMat;
            isWiped[i] = wiped;
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

    public void Unwipe()
    {
        for (int i = 0; i < dirtParent.transform.childCount; i++) {
            GameObject dirt = dirtParent.transform.GetChild(i).gameObject;
            SetWiped(i, false, dirt);
        }
    }

    public int GetNumWiped()
    {
        return isWiped.Count(w => w);
    }

    public int GetAntiparticlesWiped()
    {
        return antiParticleIsWiped.Count(w => w);
    }
}
