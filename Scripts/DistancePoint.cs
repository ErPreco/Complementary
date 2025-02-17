using UnityEngine;

public class DistancePoint {
    private Vector3 point1;
    private Vector3 point2;
    private float distance;

    /// <summary>
    /// </summary>
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    public DistancePoint(Vector3 point1, Vector3 point2) {
        this.point1 = point1;
        this.point2 = point2;
        distance = Vector3.Distance(point1, point2);
    }

    /// <summary>
    /// Returns the first point.
    /// </summary>
    /// <returns>The first point.</returns>
    public Vector3 GetPoint1() {
        return point1;
    }

    /// <summary>
    /// Returns the second point.
    /// </summary>
    /// <returns>The second point.</returns>
    public Vector3 GetPoint2() {
        return point2;
    }

    /// <summary>
    /// Returns the distance between the two points.
    /// </summary>
    /// <returns>The distance between the two points.</returns>
    public float GetDistance() {
        return distance;
    }
}
