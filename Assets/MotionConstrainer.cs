using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MotionConstrainer : MonoBehaviour
{

    public GameObject zone; //Polygon to be referenced for constraints

    public enum ConstraintType  {
        NONE,
        NORMAL,
        GLIDE,
        ROTATION,
        RECTIFY
    }

    public ConstraintType mode = ConstraintType.NONE;

    private Vector3 lastPos;
    private Quaternion lastRot;
    public Vector3 debug;
    public GameObject dot;
    private bool isColliding = false;
    int count = 0;

    public GameObject[] debugPoints;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 normal = zone.GetComponent<MeshFilter>().mesh.normals[0];
        normal = zone.transform.rotation * normal;
        Vector3 unconstrainedMovment = transform.position - lastPos;
        switch (mode)
        {
            case ConstraintType.NONE:
                break;
            case ConstraintType.NORMAL:
                transform.position = (Vector3.Dot(normal, unconstrainedMovment) * normal) + lastPos;
                break;
            case ConstraintType.GLIDE:
                count = count % zone.GetComponent<MeshFilter>().mesh.triangles.Length;
                Vector3 nearestPoint = ProjectPointOntoPlane(normal, zone.transform.position, transform.position);
                dot.transform.position = nearestPoint;
                Vector3 xAxis = Vector3.Normalize((zone.transform.rotation * zone.GetComponent<MeshFilter>().mesh.vertices[1]) * zone.transform.localScale[0] + zone.transform.position - (zone.transform.rotation * zone.GetComponent<MeshFilter>().mesh.vertices[0]) * zone.transform.localScale[0] + zone.transform.position);
                Vector3 yAxis = Vector3.Cross(xAxis, Vector3.Normalize(normal));
                Vector2[] pointsOnPlane = new Vector2[zone.GetComponent<MeshFilter>().mesh.vertices.Length];
                Vector3 offset;
                for (int i = 0; i < pointsOnPlane.Length; i++)
                {
                    Vector3 point = (zone.transform.rotation * zone.GetComponent<MeshFilter>().mesh.vertices[i]) * zone.transform.localScale[0] + zone.transform.position;
                    offset = point - ((zone.transform.rotation * zone.GetComponent<MeshFilter>().mesh.vertices[0]) * zone.transform.localScale[0] + zone.transform.position);
                    pointsOnPlane[i] = new Vector2(Vector3.Dot(offset, xAxis), Vector3.Dot(offset, yAxis));
                }
                offset = nearestPoint - ((zone.transform.rotation * zone.GetComponent<MeshFilter>().mesh.vertices[0]) * zone.transform.localScale[0] + zone.transform.position);
                Vector2 nearestPointOnPlane = new Vector2(Vector3.Dot(offset, xAxis), Vector3.Dot(offset, yAxis));

                bool inside = false;

                //debugPoints[0].transform.position = new Vector3(pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[0]].x, 0.3f, pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[0]].y);
                //debugPoints[1].transform.position = new Vector3(pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[1]].x, 0.3f, pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[1]].y);
                //debugPoints[2].transform.position = new Vector3(pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[2]].x, 0.3f, pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[2]].y);
                
                //dot.transform.position = new Vector3(nearestPointOnPlane.x, 0.3f, nearestPointOnPlane.y);

                for (int i = 0; i < zone.GetComponent<MeshFilter>().mesh.triangles.Length; i+=3)
                {
                    if (IsPointInTriangle(pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[i]], pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[i + 1]], pointsOnPlane[zone.GetComponent<MeshFilter>().mesh.triangles[i + 2]], nearestPointOnPlane))
                    {
                        inside = true;
                        Debug.Log("Point in Plane");
                        debugPoints[0].transform.position = (zone.transform.rotation * zone.GetComponent<MeshFilter>().mesh.vertices[zone.GetComponent<MeshFilter>().mesh.triangles[i]]) * zone.transform.localScale[0] + zone.transform.position;
                        debugPoints[1].transform.position = (zone.transform.rotation * zone.GetComponent<MeshFilter>().mesh.vertices[zone.GetComponent<MeshFilter>().mesh.triangles[i + 1]]) * zone.transform.localScale[0] + zone.transform.position;
                        debugPoints[2].transform.position = (zone.transform.rotation * zone.GetComponent<MeshFilter>().mesh.vertices[zone.GetComponent<MeshFilter>().mesh.triangles[i + 2]]) * zone.transform.localScale[0] + zone.transform.position;
                        break;
                    }
                }
                if (!inside)
                {
                    transform.position = lastPos;
                    break;
                }




                transform.position = (unconstrainedMovment - (Vector3.Dot(normal, unconstrainedMovment) * normal)) + lastPos;
                break;

            case ConstraintType.ROTATION:
                Quaternion deltaRot = Quaternion.Inverse(lastRot)*transform.rotation;
                float angle = 0.0f;
                Vector3 axis = Vector3.zero;
                deltaRot.ToAngleAxis(out angle, out axis);
                angle = angle * Vector3.Dot(axis, normal);
                transform.rotation = lastRot;
                transform.RotateAround(transform.position, normal, angle);
                //deltaRot = Quaternion.AngleAxis(1f, normal);
                //transform.rotation = lastRot * deltaRot;
                transform.position = lastPos;
                break;

            case ConstraintType.RECTIFY:
                transform.rotation = zone.transform.rotation;
                this.mode = ConstraintType.ROTATION;
                break;
        }

        isColliding = false;
        lastPos = transform.position;
        lastRot = transform.rotation;
        count += 3;
    }

    public static bool IsPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 point)
    {
        // Calculate the vectors
        Vector2 v0 = p2 - p1;
        Vector2 v1 = p3 - p1;
        Vector2 v2 = point - p1;

        // Compute dot products
        float dot00 = Vector2.Dot(v0, v0);
        float dot01 = Vector2.Dot(v0, v1);
        float dot02 = Vector2.Dot(v0, v2);
        float dot11 = Vector2.Dot(v1, v1);
        float dot12 = Vector2.Dot(v1, v2);

        // Compute barycentric coordinates
        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }

    //public static bool IsPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 point)
    //{
    //    // Calculate the area of the triangle
    //    float area = 0.5f * (-p2.y * p3.x + p1.y * (-p2.x + p3.x) + p1.x * (p2.y - p3.y) + p2.x * p3.y);

    //    // Calculate the area of the triangle with the point
    //    float area1 = 0.5f * (-p2.y * point.x + p1.y * (-p2.x + point.x) + p1.x * (p2.y - point.y) + p2.x * point.y);
    //    float area2 = 0.5f * (-point.y * p3.x + p1.y * (-point.x + p3.x) + p1.x * (point.y - p3.y) + point.x * p3.y);
    //    float area3 = 0.5f * (-p1.y * p3.x + point.y * (-p1.x + p3.x) + point.x * (p1.y - p3.y) + p1.x * p3.y);

    //    // Check if the sum of area1, area2, and area3 is equal to the area of the triangle
    //    return Mathf.Approximately(area, area1 + area2 + area3);
    //    //return Mathf.Abs(area-(area1 + area2 + area3)) < .000001;
    //}

    //public static bool isInTriangle(Vector2 firstPt, Vector2 secondPt, Vector2 thirdPt, Vector2 check)
    //{
    //    return !isLeftOfLine(firstPt, secondPt, check) &&
    //           !isLeftOfLine(secondPt, thirdPt, check) &&
    //           !isLeftOfLine(thirdPt, firstPt, check);
    //}

    //public static bool isLeftOfLine(Vector2 firstPt, Vector2 secondPt, Vector2 check)
    //{

    //    float A = secondPt.y - firstPt.y;
    //    float B = firstPt.x - secondPt.x;
    //    float C = -1 * firstPt.y * B - secondPt.x * A;

    //    return A * check.x + B * check.y + C < 0;
    //}


    Vector3 ProjectPointOntoPlane(Vector3 normal, Vector3 planePoint, Vector3 point)
    {
        // Normalize the normal vector
        normal.Normalize();

        // Calculate the vector from the plane point to the point to project
        Vector3 planeToPoint = point - planePoint;

        // Calculate the distance from the point to the plane
        float distance = Vector3.Dot(planeToPoint, normal);

        // Project the point onto the plane
        Vector3 projectedPoint = point - distance * normal;

        return projectedPoint;
    }

    public void startGlide()
    {
        this.mode = ConstraintType.GLIDE;
    }

    public void startInsert()
    {
        this.mode = ConstraintType.NORMAL;
    }

    public void startRotation()
    {
        this.mode = ConstraintType.ROTATION;
    }

    public void startRectify()
    {
        this.mode = ConstraintType.RECTIFY;
    }


}
