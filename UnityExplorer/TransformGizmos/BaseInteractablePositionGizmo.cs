using System;
using UnityEngine;
using UnityExplorer.GLDrawHelpers;

namespace UnityExplorer.TransformGizmos
{
    public abstract class BaseInteractablePositionGizmo : BaseTransformGizmo
    {
        bool selectedXAxis = false;
        bool selectedYAxis = false;
        bool selectedZAxis = false;

        Vector3 selectedXPosition;
        Vector3 selectedYPosition;
        Vector3 selectedZPosition;

        Vector3 initialPosition;

        public abstract float LineSegmentDistance();
        public abstract Vector3 GetXDirection(Transform t);
        public abstract Vector3 GetYDirection(Transform t);
        public abstract Vector3 GetZDirection(Transform t);

        protected Vector3 GetPositionWithReferencial(Transform t, Vector3 position) 
        {
            if (t.parent == null)
                return position;

            return t.parent.InverseTransformPoint(position);
        }
        protected Vector3 ReturnPositionFromReferencial(Transform t, Vector3 position)
        {
            if (t.parent == null)
                return position;

            return t.parent.TransformPoint(position);
        }
        protected Vector3 GetDirectionWithReferencial(Transform t, Vector3 direction)
        {
            if (t.parent == null)
                return direction;

            return t.parent.InverseTransformDirection(direction);
        }

        public override bool CheckSelected(Ray ray, Transform t, float maxDistanceToSelect)
        {
            Vector3 x = GetXDirection(t);
            Vector3 y = GetYDirection(t);
            Vector3 z = GetZDirection(t);

            if (Vector3MathUtils.GetDistanceFromLineToLineSegment(ray.direction, ray.origin, t.position, t.position + x * LineSegmentDistance()) <= maxDistanceToSelect)
            {
                selectedXAxis = true;
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, x, t.position, out _, out selectedXPosition);
                selectedXPosition = GetPositionWithReferencial(t, selectedXPosition);
            }
            if (Vector3MathUtils.GetDistanceFromLineToLineSegment(ray.direction, ray.origin, t.position, t.position + y * LineSegmentDistance()) <= maxDistanceToSelect)
            {
                selectedYAxis = true;
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, y, t.position, out _, out selectedYPosition);
                selectedYPosition = GetPositionWithReferencial(t, selectedYPosition);
            }
            if (Vector3MathUtils.GetDistanceFromLineToLineSegment(ray.direction, ray.origin, t.position, t.position + z * LineSegmentDistance()) <= maxDistanceToSelect)
            {
                selectedZAxis = true;
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, z, t.position, out _, out selectedZPosition);
                selectedZPosition = GetPositionWithReferencial(t, selectedZPosition);
            }
            initialPosition = GetPositionWithReferencial(t, t.position);

            return IsSelected();
        }

        public override void OnRender(Transform t)
        {
            Vector3 x = GetXDirection(t);
            Vector3 y = GetYDirection(t);
            Vector3 z = GetZDirection(t);

            GLHelper.DrawOnGlobalReference(() =>
            {
                //Local position axis
                GLDraw.Vector(x, LineSegmentDistance(), t.position, selectedXAxis ? Color.Lerp(Color.red, Color.white, 0.8f) : Color.red);
                GLDraw.Vector(y, LineSegmentDistance(), t.position, selectedYAxis ? Color.Lerp(Color.yellow, Color.white, 0.8f) : Color.yellow);
                GLDraw.Vector(z, LineSegmentDistance(), t.position, selectedZAxis ? Color.Lerp(Color.cyan, Color.white, 0.8f) : Color.cyan);
            });
        }
        public override void OnSelected(Ray ray, Transform t)
        {
            Vector3 x = GetXDirection(t);
            Vector3 y = GetYDirection(t);
            Vector3 z = GetZDirection(t);

            Vector3 aditionToPosition = Vector3.zero;
            if (selectedXAxis)
            {
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, x, t.position, out _, out Vector3 currentXSelectedPosition);
                currentXSelectedPosition = GetPositionWithReferencial(t, currentXSelectedPosition);
                float distanceWihReference = Vector3.Dot(currentXSelectedPosition - selectedXPosition, GetDirectionWithReferencial(t, x));
                aditionToPosition += distanceWihReference * GetDirectionWithReferencial(t, x);
            }
            if (selectedYAxis)
            {
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, y, t.position, out _, out Vector3 currentYSelectedPosition);
                currentYSelectedPosition = GetPositionWithReferencial(t, currentYSelectedPosition);
                float distanceWihReference = Vector3.Dot(currentYSelectedPosition - selectedYPosition, GetDirectionWithReferencial(t, y));
                aditionToPosition += distanceWihReference * GetDirectionWithReferencial(t, y);
            }
            if (selectedZAxis)
            {
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, z, t.position, out _, out Vector3 currentZSelectedPosition);
                currentZSelectedPosition = GetPositionWithReferencial(t, currentZSelectedPosition);
                float distanceWihReference = Vector3.Dot(currentZSelectedPosition - selectedZPosition, GetDirectionWithReferencial(t, z));
                aditionToPosition += distanceWihReference * GetDirectionWithReferencial(t, z);
            }
            t.position = ReturnPositionFromReferencial(t, initialPosition + aditionToPosition);
        }
        public override bool IsSelected()
        {
            return selectedXAxis || selectedYAxis || selectedZAxis;
        }

        public override void Reset()
        {
            selectedXAxis = false;
            selectedYAxis = false;
            selectedZAxis = false;
        }
    }
}
