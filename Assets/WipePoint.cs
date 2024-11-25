using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WipePoint : MonoBehaviour
{
    public static List<GameObject> points = new List<GameObject>();
    public static Vector3 allPointsPlaneNormal;
    public static GameObject uiPoint;
    public bool isUI = true;
    public GameObject prefab;
    public TextMesh textMesh;

    public GameObject ghost;


    public int debugSpawnCounter;



    private static int nextID = 0;
    public int id = nextID++;

    public bool button = false;
    // Start is called before the first frame update
    void Start()
    {
        if (this.isUI) {
            uiPoint = gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (this.isUI) {
            this.transform.position = GameObject.Find("Wipe Point Spawner").transform.position;
        }
        if (button) {
            button = false;
            Manifest();
            Normalize();
        }

        if (isUI) {
            Normalize();
        }
    }

    public void setIsUI(bool isUI)
    {
        this.isUI = isUI;
        if (this.isUI) {
            uiPoint = gameObject;
        }
    }

    public void Manifest() {


        if (!this.isUI) return;
        this.isUI = false;
        points.Add(this.gameObject);
        GameObject uiPoint = Instantiate(prefab, GameObject.Find("Wipe Point Spawner").transform.position, GameObject.Find("Wipe Point Spawner").transform.rotation);
        uiPoint.transform.SetParent(transform.parent, true);
        uiPoint.GetComponent<WipePoint>().isUI = true;
    }

    public void Normalize() {
        Vector3 planePosition;
        Vector3 planeNormal;
        FitPlane(points, out planePosition, out planeNormal);
        allPointsPlaneNormal = planeNormal;
        foreach (var point in points) {
             Vector3 toPoint = point.transform.position - planePosition; // Vector from plane position to the point
        float distanceToPlane = Vector3.Dot(toPoint, planeNormal); // Project onto the normal

            point.GetComponent<WipePoint>().ghost.transform.position = point.transform.position - distanceToPlane * planeNormal;
        }
    }

    private void FitPlane(List<GameObject> gameObjects, out Vector3 planePosition, out Vector3 planeNormal)
    {
        List<Vector3> points = new List<Vector3>();
        foreach (var gameObject in gameObjects){
            points.Add(gameObject.transform.position);
        }
         int n = points.Count;
        Vector3 sum = new Vector3();
        foreach (Vector3 point in points)
        {
            sum += point;
        }

        Vector3 centroid = sum * (1.0f / ((float)n));

        float xx = 0, xy = 0, xz = 0;
        float yy = 0, yz = 0, zz = 0;

        foreach (Vector3 point in points)
        {
            Vector3 r = point - centroid;
            xx += r.x * r.x;
            xy += r.x * r.y;
            xz += r.x * r.z;
            yy += r.y * r.y;
            yz += r.y * r.z;
            zz += r.z * r.z;
        }

        float det_x = yy * zz - yz * yz;
        float det_y = xx * zz - xz * xz;
        float det_z = xx * yy - xy * xy;

        float det_max = Mathf.Max(det_x, det_y, det_z);

        Vector3 normal = new Vector3();

        if (det_max == det_x)
        {
            float a = (xz * yz - xy * zz) / det_x;
            float b = (xy * yz - xz * yy) / det_x;

            normal = new Vector3(1.0f, a, b);
        }
        else if (det_max == det_y)
        {
            float a = (yz * xz - xy * zz) / det_y;
            float b = (xy * yz - xz * yy) / det_y;

            normal = new Vector3(x: a, y: 1.0f, z: b);
        }
        else
        {
            float a = (yz * xy - xz * yy) / det_z;
            float b = (xz * xy - yz * xx) / det_z;

            normal = new Vector3(x: a, y: b, z: 1.0f);
        }

        Plane plane = new Plane(normal.normalized, centroid);
        

        planePosition = centroid;
        planeNormal = normal.normalized;

        
    }

    }
