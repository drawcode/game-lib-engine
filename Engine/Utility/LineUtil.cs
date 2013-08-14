using System;
using UnityEngine;

public static class LineUtil {

    /// <summary>
    /// Calculate the point on the line a-b nearest to given point
    /// </summary>
    /// <param name="a">
    /// Starting point of line
    /// </param>
    /// <param name="b">
    /// Ending point of line
    /// </param>
    /// <param name="point">
    /// Point to search for nearest
    /// </param>
    /// <returns>
    /// Point one the line a-b nearest to point
    /// </returns>
    public static Vector3 NearestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
        var dist = NearestDistanceOnLine(lineStart, lineEnd, point);
        return lineStart + ((lineEnd - lineStart).normalized * dist);
    }

    /// <summary>
    /// Calculate the nearest point on the defined line segment, and its associated distance along the segment, nearest to the given point
    /// </summary>
    /// <param name="segmentStart">
    /// The start of the line segment
    /// </param>
    /// <param name="segmentEnd">
    /// The end of the line segment
    /// </param>
    /// <param name="point">
    /// The point to project onto the line segment
    /// </param>
    /// <param name="nearestDistance">
    /// The distance along the line segment nearest to point
    /// </param>
    /// <returns>
    /// The point on the line segment nearest to point
    /// </returns>
    ///
    public static Vector3 NearestPointOnSegment(Vector3 segmentStart, Vector3 segmentEnd, Vector3 point, out float nearestDistance) {
        var norm = segmentEnd - segmentStart;
        var length = norm.magnitude;
        norm *= 1 / length;
        return NearestPointOnSegment(segmentStart, norm, length, point, out nearestDistance);
    }

    /// <summary>
    /// Calculate the point on the defined segment, and its associated distance along the segment, that is nearest to the givent point
    /// </summary>
    /// <param name="segmentStart">
    /// The start of the line segment
    /// </param>
    /// <param name="segmentNormal">
    /// The direction, or normal, of the segment
    /// </param>
    /// <param name="segmentLength">
    /// The length of the line segment
    /// </param>
    /// <param name="point">
    /// The point to project onto the line segment
    /// </param>
    /// <param name="nearestDistance">
    /// The distance along the line segment nearest to point
    /// </param>
    /// <returns>
    /// The point on the line segment nearest to point
    /// </returns>
    public static Vector3 NearestPointOnSegment(Vector3 segmentStart, Vector3 segmentNormal, float segmentLength, Vector3 point, out float nearestDistance) {
        nearestDistance = NearestDistanceOnSegment(segmentStart, segmentNormal, segmentLength, point);
        return segmentStart + (segmentNormal * nearestDistance);
    }

    /// <summary>
    /// Calculates the distance on the defined segment nearest to the given point
    /// </summary>
    /// <param name="segmentStart">
    /// The start of the line segment
    /// </param>
    /// <param name="segmentNormal">
    /// The direction, or normal, of the line segment
    /// </param>
    /// <param name="segmentLength">
    /// The length of the line segment
    /// </param>
    /// <param name="point">
    /// The point to project onto the line segment
    /// </param>
    /// <returns>
    /// The distance along the segment nearest to point
    /// </returns>
    ///
    public static float NearestDistanceOnSegment(Vector3 segmentStart, Vector3 segmentNormal, float segmentLength, Vector3 point) {

        //Convert point into a space local to the segment start
        var localPoint = point - segmentStart;

        //Project the local version of point onto the normal of the segment
        var segProj = Vector3.Dot(segmentNormal, localPoint);

        //Return the distance along the segment (clamped to the segment extents)
        return Mathf.Clamp(segProj, 0f, segmentLength);
    }

    /// <summary>
    /// Calculates the distance along line from a starting point that is nearest to the given point
    /// </summary>
    /// <param name="lineStart">
    /// The start of the line segment
    /// </param>
    /// <param name="lineNormal">
    /// The direction, or normal, of the line segment
    /// </param>
    /// <param name="point">
    /// The point to project onto the line segment
    /// </param>
    /// <returns>
    /// The distance along the line nearest to point
    /// </returns>
    ///
    public static float NearestDistanceOnLine(Vector3 lineStart, Vector3 lineNormal, Vector3 point) {

        //Convert point into a space local to the segment start
        var localPoint = point - lineStart;

        //Project the local version of point onto the normal of the segment
        return Vector3.Dot(lineNormal, localPoint);
    }

    /// <summary>
    /// Calculate the percetages of the points a-b, forming a line segment, that produce the nearest point to given point. The
    /// results apct and bpct of this function can be used with the equation (la * apct) + (lb + bpct) to find the
    /// actual nearest point on segment a-b.
    /// </summary>
    /// <param name="la">
    /// Point a on line a-b
    /// </param>
    /// <param name="lb">
    /// Point b on line a-b
    /// </param>
    /// <param name="point">
    /// Search point to find nearest to
    /// </param>
    /// <param name="apct">
    /// Percentage of a to use to calculate the nearest point on segment a-b
    /// </param>
    /// <param name="bpct">
    /// Percentage of b to use to calculate the nearest point on segmnet a-b
    /// </param>
    public static void NearestPercentagesOnSegment(Vector3 la, Vector3 lb, Vector3 point, out float apct, out float bpct) {
        NearestPercentagesOnLine(la, lb, point, out apct, out bpct);
        apct = MathUtil.UnitClamp(apct);
        bpct = MathUtil.UnitClamp(bpct);
    }

    /// <summary>
    /// Calculate the percetages of the points a-b, forming a line, that produce the nearest point to given point. The
    /// results apct and bpct of this function can be used with the equation (la * apct) + (lb + bpct) to find the
    /// actual nearest point on a-b.
    /// </summary>
    /// <param name="la">
    /// Point a on line a-b
    /// </param>
    /// <param name="lb">
    /// Point b on line a-b
    /// </param>
    /// <param name="point">
    /// Search point to find nearest to
    /// </param>
    /// <param name="apct">
    /// Percentage of a to use to calculate the nearest point on a-b
    /// </param>
    /// <param name="bpct">
    /// Percentage of b to use to calculate the nearest point on a-b
    /// </param>
    public static void NearestPercentagesOnLine(Vector3 la, Vector3 lb, Vector3 point, out float apct, out float bpct) {
        var pa = point - la;
        var ba = lb - la;
        var a = pa.magnitude * Vector3.Dot(pa.normalized, ba.normalized);

        var pb = point - lb;
        var ab = la - lb;
        var b = pb.magnitude * Vector3.Dot(pb.normalized, ab.normalized);

        var dist = ba.magnitude;
        apct = b / dist;
        bpct = a / dist;
    }
}