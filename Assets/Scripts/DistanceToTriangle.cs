using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceToTriangle
{
    // Function to calculate the shortest distance from a point to a triangle
    public static float GetDistanceToTriangle(Vector3 p, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Step 1: Calculate the normal of the triangle's plane
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        Vector3 normal = Vector3.Cross(edge1, edge2).normalized;

        // Step 2: Project the point onto the plane of the triangle
        float distanceToPlane = Vector3.Dot(p - v0, normal);  // Distance from the point to the plane
        Vector3 projectedPoint = p - normal * distanceToPlane; // The projected point on the plane

        // Step 3: Check if the projected point is inside the triangle
        bool isInside = IsPointInTriangle(projectedPoint, v0, v1, v2, normal);

        if (isInside)
        {
            // If inside, the perpendicular distance to the triangle is the distance to the plane
            return Mathf.Abs(distanceToPlane);
        }
        else
        {
            // If outside, compute the distance to the nearest edge of the triangle
            float distanceToEdge1 = DistancePointToLineSegment(p, v0, v1);
            float distanceToEdge2 = DistancePointToLineSegment(p, v1, v2);
            float distanceToEdge3 = DistancePointToLineSegment(p, v2, v0);

            // Return the smallest distance to the edges
            return Mathf.Min(distanceToEdge1, distanceToEdge2, distanceToEdge3);
        }
    }

    // Function to check if a point is inside the triangle using barycentric coordinates
    private static bool IsPointInTriangle(Vector3 p, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 normal)
    {
        // Compute vectors
        Vector3 edge0 = v1 - v0;
        Vector3 edge1 = v2 - v0;
        Vector3 edge2 = p - v0;

        // Compute the cross products to get the barycentric coordinates
        Vector3 c0 = Vector3.Cross(edge0, edge2);
        Vector3 c1 = Vector3.Cross(edge1, edge2);
        Vector3 c2 = Vector3.Cross(edge1, edge0);

        // Check if all the cross products have the same direction as the normal
        return (Vector3.Dot(c0, normal) >= 0) && (Vector3.Dot(c1, normal) >= 0) && (Vector3.Dot(c2, normal) >= 0);
    }

    // Function to calculate the distance from a point to a line segment (edge of the triangle)
    private static float DistancePointToLineSegment(Vector3 p, Vector3 v0, Vector3 v1)
    {
        Vector3 segmentDir = v1 - v0;
        float segmentLength = segmentDir.magnitude;
        segmentDir.Normalize();

        // Project point onto the line defined by the segment
        float projection = Vector3.Dot(p - v0, segmentDir);

        // Check if the projection is outside the segment
        if (projection < 0)
            return (p - v0).magnitude;  // Distance to v0
        if (projection > segmentLength)
            return (p - v1).magnitude;  // Distance to v1

        // Otherwise, calculate the perpendicular distance
        Vector3 closestPoint = v0 + segmentDir * projection;
        return (p - closestPoint).magnitude;
    }
}
