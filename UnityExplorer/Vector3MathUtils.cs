using UnityEngine;

namespace UnityExplorer
{
    //From https://math.stackexchange.com/questions/511370/how-to-rotate-one-vector-about-another
    public static class Vector3MathUtils
    {
        public static void GetParalelAndPerpendicularComponent(Vector3 axis, Vector3 v, out Vector3 paralel, out Vector3 perpendicular)
        {
            paralel = Vector3.Dot(v, axis) * axis / axis.magnitude;
            perpendicular = v - paralel;
        }
        public static Vector3 GetRotationVector(Vector3 axis, Vector3 perpendicularComponent)
        {
            return Vector3.Cross(axis, perpendicularComponent);
        }
        public static Vector3 GetRotatedVectorComponent(Vector3 rotationVector, Vector3 perpendicularComponent, float angle)
        {
            float x1 = Mathf.Cos(angle) / perpendicularComponent.sqrMagnitude;
            float x2 = Mathf.Sin(angle);

            return perpendicularComponent.sqrMagnitude * (x1 * perpendicularComponent + x2 * rotationVector);
        }

        //From https://codereview.stackexchange.com/questions/43928/algorithm-to-get-an-arbitrary-perpendicular-vector
        public static Vector3 GetArbitraryPerpendicularVector(Vector3 v)
        {
            if (v.magnitude == 0f)
            {
                return Vector3.forward;
            }
            Vector3 firstAttempt = Vector3.Cross(v, Vector3.forward);
            if (firstAttempt.magnitude != 0f)
            {
                return firstAttempt.normalized;
            }
            else
            {
                return Vector3.Cross(v, Vector3.up).normalized;
            }
        }
        //From https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        public static float GetDistanceFromLine(Vector3 point, Vector3 direction, Vector3 lineStartingPoint)
        {
            return Vector3.Cross(point - lineStartingPoint, direction).magnitude / direction.magnitude;
        }
        //From https://en.wikipedia.org/wiki/Skew_lines#Distance
        public static float GetDistanceFromLineToLineSegment(Vector3 direction, Vector3 lineStartingPoint, Vector3 segmentStartingPoint, Vector3 segmentEndPoint)
        {
            Vector3 directionOfSegment = segmentEndPoint - segmentStartingPoint;
            Vector3 n = Vector3.Cross(direction, directionOfSegment);
            Vector3 n2 = Vector3.Cross(directionOfSegment, n);
            Vector3 closestPointFromLineToSegment = lineStartingPoint + Vector3.Dot(segmentStartingPoint - lineStartingPoint, n2) / (Vector3.Dot(direction, n2)) * direction;

            return GetDistanceFromLineSegment(closestPointFromLineToSegment, segmentStartingPoint, segmentEndPoint);
        }
        public static void GetClosestPointFromLines(Vector3 directionOne, Vector3 lineStartingPointOne, Vector3 directionTwo, Vector3 lineStartingPointTwo, out Vector3 closesPointOnLineOne, out Vector3 closesPointOnLineTwo)
        {
            Vector3 n = Vector3.Cross(directionOne, directionTwo);
            Vector3 n2 = Vector3.Cross(directionTwo, n);
            closesPointOnLineOne = lineStartingPointOne + Vector3.Dot(lineStartingPointTwo - lineStartingPointOne, n2) / (Vector3.Dot(directionOne, n2)) * directionOne;

            Vector3 n1 = Vector3.Cross(directionOne, n);
            closesPointOnLineTwo = lineStartingPointTwo + Vector3.Dot(lineStartingPointOne - lineStartingPointTwo, n1) / (Vector3.Dot(directionTwo, n1)) * directionTwo;

        }
        public static float GetDistanceFromLineSegment(Vector3 point, Vector3 startingPoint, Vector3 endPoint)
        {
            float distanceFromStartingPoint = Vector3.Distance(point, startingPoint);
            float distanceFromEndPoint = Vector3.Distance(point, endPoint);
            float distanceFromLine = GetDistanceFromLine(point, endPoint - startingPoint, startingPoint);

            return Mathf.Min(distanceFromStartingPoint, distanceFromEndPoint, distanceFromLine);
        }
        //From https://www.geometrictools.com/Documentation/DistanceToCircle3.pdf
        public static float GetClosestPointToCircle(Vector3 point, Vector3 center, Vector3 normal, float radius,out bool isEquidistant, out Vector3 closestPoint) 
        {
            Vector3 delta = point - center;
            float dotND = Vector3.Dot(normal, delta);
            Vector3 QmC = delta - dotND * normal;
            float lenghtQmC = QmC.magnitude;
            if (lenghtQmC > 0f) 
            {
                closestPoint = center + radius * (QmC / lenghtQmC);
                isEquidistant = false;
                return (point - closestPoint).magnitude;
            }
            else 
            {
                closestPoint = Vector3.zero;
                isEquidistant = true;
                return delta.sqrMagnitude + radius * radius;
            }
        }
        //From https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
        public static float GetClosestPointFromLineToCircle(Vector3 direction, Vector3 point, Vector3 center, Vector3 normal, float radius, out bool lineParalelToCircle, out bool isEquidistant, out Vector3 closestPoint)
        {
            float ln = Vector3.Dot(direction, normal);
            if(ln == 0f) //Yes, I know we are ignoring when the line and the circle are on the same plane, I just don't want to write a proper GetClosestPointFromLineToCircle
            {
                lineParalelToCircle = true;
                isEquidistant = false;
                closestPoint = Vector3.zero;
                return 0f; 
            }

            float d = Vector3.Dot((center - point), normal) / ln;
            Vector3 intersectionPoint = point + direction * d;

            lineParalelToCircle = false;

            return GetClosestPointToCircle(intersectionPoint, center, normal, radius, out isEquidistant, out closestPoint);
        }
    }
}
