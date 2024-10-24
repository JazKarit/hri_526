using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WipeSurface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    class PointNode
    {
        public Vector2 point;
        public PointNode next;
        public PointNode prev;
        public int index;

        public PointNode(Vector2 point, int index)
        {
            this.point = point;
            this.index = index;
            this.next = null;
            this.prev = null;
        }

        public static bool isLeftOfLine(PointNode leftNode, PointNode rightNode, PointNode check)
        {
            Vector2 left = leftNode.point;
            Vector2 right = rightNode.point;
            float A = right.y - left.y;
            float B = left.x - right.x;
            float C = -1 * left.y * B - right.x * A;

            return A * check.point.x + B * check.point.y + C < 0;
        }

        public static bool isLeftOfLine(PointNode leftNode, PointNode rightNode, Vector2 check)
        {
            Vector2 left = leftNode.point;
            Vector2 right = rightNode.point;
            float A = right.y - left.y;
            float B = left.x - right.x;
            float C = -1 * left.y * B - right.x * A;

            return A * check.x + B * check.y + C < 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        if (WipePoint.points.Count > 2)
        {
            Vector3[] points = new Vector3[WipePoint.points.Count];
            Vector2[] pointsOnPlane = new Vector2[WipePoint.points.Count];
            int[] triangles = new int[(WipePoint.points.Count - 2) * 3];
            int trianglesIndex = 0;
            Vector3 xAxis = Vector3.Normalize(WipePoint.points[1].GetComponent<WipePoint>().ghost.transform.position - WipePoint.points[0].GetComponent<WipePoint>().ghost.transform.position);
            Vector3 yAxis = Vector3.Cross(xAxis, WipePoint.allPointsPlaneNormal);

            for (int i = 0; i < WipePoint.points.Count; i++)
            {
                points[i] = WipePoint.points[i].GetComponent<WipePoint>().ghost.transform.position;
                Vector3 offset = points[i] - WipePoint.points[0].GetComponent<WipePoint>().ghost.transform.position;
                pointsOnPlane[i] = new Vector2(Vector3.Dot(offset, xAxis), Vector3.Dot(offset, yAxis));
                WipePoint.points[i].GetComponent<WipePoint>().debug.transform.position = new Vector3(pointsOnPlane[i].x, 0.5f, pointsOnPlane[i].y);

                // if (i >= 2) {
                //     triangles[trianglesIndex] = 0;
                //     triangles[trianglesIndex+1] = i-1;
                //     triangles[trianglesIndex+2] = i;
                //     trianglesIndex += 3;
                // }

            }

            //Triangulation
            PointNode lastNode = new PointNode(pointsOnPlane[0], 0);
            PointNode zeroNode = lastNode;
            for (int i = 1; i < pointsOnPlane.Length; i++)
            {
                PointNode newNode = new PointNode(pointsOnPlane[i], i);
                newNode.prev = lastNode;
                lastNode.next = newNode;
                if (i == pointsOnPlane.Length - 1)
                {
                    zeroNode.prev = newNode;
                    newNode.next = zeroNode;
                }
                lastNode = newNode;
            }


            int watchDog = 0;
            int order = 0;
            while (trianglesIndex < triangles.Length)
            {
                if (watchDog > points.Length)
                {
                    Debug.Log("watchDog");
                    break;
                }
                watchDog++;
                PointNode leftNode = lastNode.prev;
                PointNode rightNode = lastNode.next;
                Vector2 left = leftNode.point;
                Vector2 right = rightNode.point;
                if (trianglesIndex == triangles.Length - 3)
                {
                    triangles[trianglesIndex] = leftNode.index;
                    triangles[trianglesIndex + 1] = lastNode.index;
                    triangles[trianglesIndex + 2] = rightNode.index;
                    WipePoint.points[lastNode.index].GetComponent<WipePoint>().textMesh.GetComponent<TextMesh>().text = order.ToString();
                    WipePoint.points[leftNode.index].GetComponent<WipePoint>().textMesh.GetComponent<TextMesh>().text = "X";
                    WipePoint.points[rightNode.index].GetComponent<WipePoint>().textMesh.GetComponent<TextMesh>().text = "X";
                        order++;
                    trianglesIndex += 3;
                    break;
                }
                if (!PointNode.isLeftOfLine(leftNode, rightNode, lastNode))
                {
                    Debug.Log("left of line");
                    PointNode other = rightNode.next;
                    bool valid = true;
                    // while (other != leftNode)
                    // {
                    //     if (!PointNode.isLeftOfLine(leftNode, rightNode, other) &&
                    //         !PointNode.isLeftOfLine(rightNode, lastNode, other) &&
                    //         !PointNode.isLeftOfLine(lastNode, leftNode, other))
                    //     {
                    //         valid = false;
                    //         Debug.Log("invalid");
                    //         break;
                    //     }
                    //     other = other.next;
                    // }

                    for (int i = 0; i < pointsOnPlane.Length; i++) {
                        if (i == leftNode.index || i == lastNode.index || i == rightNode.index) continue;
                        if (!PointNode.isLeftOfLine(leftNode, rightNode, pointsOnPlane[i]) &&
                            !PointNode.isLeftOfLine(rightNode, lastNode, pointsOnPlane[i]) &&
                            !PointNode.isLeftOfLine(lastNode, leftNode, pointsOnPlane[i]))
                        {
                            valid = false;
                            Debug.Log("invalid");
                            break;
                        }
                    }
                    
                    if (valid)
                    {

                        triangles[trianglesIndex] = leftNode.index;
                        triangles[trianglesIndex + 1] = lastNode.index;
                        triangles[trianglesIndex + 2] = rightNode.index;
                        WipePoint.points[lastNode.index].GetComponent<WipePoint>().textMesh.GetComponent<TextMesh>().text = order.ToString();
                        order++;
                        trianglesIndex += 3;
                        leftNode.next = rightNode;
                        rightNode.prev = leftNode;
                        lastNode = rightNode;
                        watchDog = 0;

                        
                    }

                }
                lastNode = rightNode;
            }



            // bool[] clipped = new bool[pointsOnPlane.Length];
            // int i = 0;
            // while (trianglesIndex < triangles.Length) {
            //     left = (i + pointsOnPlane.Length - 1) % pointsOnPlane.Length;
            //     right = (i + 1) % pointsOnPlane.Length;
            //     while (clipped[left]) {
            //         left = (left + pointsOnPlane.Length - 1) % pointsOnPlane.Length;
            //     }
            //     while (clipped[right]) {
            //         right = (right + 1) % pointsOnPlane.Length;
            //     }
            //     //Check if vertex is ear vertex
            //     float A = pointsOnPlane[right].y - pointsOnPlane[left].y;
            //     float B = pointsOnPlane[left].x - pointsOnPlane[right].x;
            //     float C = -1 * pointsOnPlane[left].y * B - pointsOnPlane[left].x * A;

            //     if (A*pointsOnPlane[i].x + B*pointsOnPlane[i].y + C > 0) {

            //     }

            // }



            Mesh mesh = new Mesh();
            mesh.vertices = points;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }

}
