using UnityEngine;
using UnityExplorer.GLDrawHelpers;

namespace UnityExplorer.TransformGizmos
{
    //TODO recreate this with GizmoControls and fix bug when radius = 0 -> position = NaN
    public class LocalPositionCylindricalCoordinates : BaseTransformGizmo
    {
        // x = radius
        // y = y
        // z = theta

        bool selectedRadius = false;
        bool selectedTheta = false;
        bool selectedY = false;

        float lineSegmentDistance = 0.25f;

        Vector3 selectedRadiusPosition;
        float aditionalThetaRotation;
        Vector3 selectedYPosition;

        Vector3 initialLocalPositionCylindrical;

        protected Vector3 GetPositionWithReferencial(Transform transform, Vector3 position)
        {            
            if (transform.parent == null)
                return position;

            return transform.parent.InverseTransformPoint(position);
        }
        protected Vector3 ReturnPositionFromReferencial(Transform transform, Vector3 position)
        {
            if (transform.parent == null)
                return position;

            return transform.parent.TransformPoint(position);
        }
        protected Vector3 GetDirectionWithReferencial(Transform transform, Vector3 direction)
        {
            if (transform.parent == null)
                return direction;

            return transform.parent.InverseTransformDirection(direction);
        }
        protected Vector3 ReturnDirectionFromReferencial(Transform transform, Vector3 direction)
        {
            if (transform.parent == null)
                return direction;

            return transform.parent.TransformDirection(direction);
        }

        public Vector3 GetRadiusDirection(Transform transform) 
        {
            if (transform.parent == null)
                return new Vector3(transform.position.x, 0f, transform.position.y).normalized;

            return transform.parent.TransformDirection(new Vector3(transform.localPosition.x, 0f, transform.localPosition.z).normalized);
        }
        public Vector3 GetUpDirection(Transform transform)
        {
            Vector3 up = Vector3.up;
            if (transform.parent != null)
            {
                up = transform.parent.up;
            }
            return up;
        }
        public Vector3 GetThetaDirection(Vector3 radiusDirection, Vector3 upDirection)
        {
            return Vector3.Cross(upDirection, radiusDirection);
        }
        

        public static Vector3 FromCylindricalToCartesian(Vector3 cylindrical)
        {
            if(cylindrical.x < 0f) 
            {
                cylindrical.x = 0f;
                //cylindrical.z += Mathf.PI;
            }
            return new Vector3(cylindrical.x * Mathf.Cos(cylindrical.z), cylindrical.y, cylindrical.x * Mathf.Sin(cylindrical.z));
        }
        public static Vector3 FromCartesianToCylindrical(Vector3 cartesian)
        {
            float y = cartesian.y;
            Vector2 polar = new Vector2(cartesian.z, cartesian.x);
            float radius = polar.magnitude;
            float theta = 0f;
            if (!( Mathf.Abs(polar.x) < 1e-2f || Mathf.Abs(polar.y) < 1e-2f))
            {
                theta = Mathf.Atan2(polar.x, polar.y);
            }
            return new Vector3(radius, y, theta);
        }

        public override bool CheckSelected(Ray ray, float maxDistanceToSelect)
        {
            Vector3 radius = GetRadiusDirection(transform);
            Vector3 up = GetUpDirection(transform);
            Vector3 centerAxis = ReturnPositionFromReferencial(transform, new Vector3(0f, transform.localPosition.y, 0f));
            float radiusValue = ReturnDirectionFromReferencial(transform, new Vector3(transform.localPosition.x, 0f, transform.localPosition.z)).magnitude;

            if (Vector3MathUtils.GetDistanceFromLineToLineSegment(ray.direction, ray.origin, transform.position, transform.position + radius * lineSegmentDistance) <= maxDistanceToSelect)
            {
                selectedRadius = true;
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, radius, transform.position, out _, out selectedRadiusPosition);
                selectedRadiusPosition = GetPositionWithReferencial(transform, selectedRadiusPosition);
            }
            if (Vector3MathUtils.GetClosestPointFromLineToCircle(ray.direction, ray.origin, centerAxis, up, radiusValue, out bool lineParalelToCircle, out _, out _) <= maxDistanceToSelect)
            {
                if (!lineParalelToCircle)
                {
                    selectedTheta = true;
                    aditionalThetaRotation = 0f;
                }
            }
            if (Vector3MathUtils.GetDistanceFromLineToLineSegment(ray.direction, ray.origin, transform.position, transform.position + up * lineSegmentDistance) <= maxDistanceToSelect)
            {
                selectedY = true;
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, up, transform.position, out _, out selectedYPosition);
                selectedYPosition = GetPositionWithReferencial(transform, selectedYPosition);
            }

            initialLocalPositionCylindrical = FromCartesianToCylindrical(GetPositionWithReferencial(transform, transform.position));

            return IsSelected();
        }

        public override void OnRender()
        {
            Vector3 radius = GetRadiusDirection(transform);
            Vector3 up = GetUpDirection(transform);
            Vector3 centerAxis = ReturnPositionFromReferencial(transform, new Vector3(0f, transform.localPosition.y, 0f));

            float radiusValue = ReturnDirectionFromReferencial(transform, new Vector3(transform.localPosition.x, 0f, transform.localPosition.z)).magnitude;

            GLDraw.Vector(radius, lineSegmentDistance, transform.position, selectedRadius ? Color.Lerp(Color.red, Color.white, 0.8f) : Color.red);
            GLDraw.Vector(up, lineSegmentDistance, transform.position, selectedY ? Color.Lerp(Color.yellow, Color.white, 0.8f) : Color.yellow);
            GLDraw.WireframeCircle(radiusValue, up, radius, centerAxis, selectedTheta ? Color.Lerp(Color.cyan, Color.white, 0.8f) : Color.cyan, 16);
            
            GLHelper.FinishDraw();
        }
        public override void OnSelected(Ray ray)
        {
            Vector3 radius = GetRadiusDirection(transform);
            Vector3 up = GetUpDirection(transform);

            float mouseScrollDelta = UniverseLib.Input.InputManager.MouseScrollDelta.y * 0.1f;

            Vector3 cylindricalAditionToPosition = Vector3.zero;
            if (selectedRadius)
            {
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, radius, transform.position, out _, out Vector3 currentRadiusSelectedPosition);
                currentRadiusSelectedPosition = GetPositionWithReferencial(transform, currentRadiusSelectedPosition);

                float distanceWihReference = Vector3.Dot(currentRadiusSelectedPosition - selectedRadiusPosition, GetDirectionWithReferencial(transform, radius));
                
                cylindricalAditionToPosition.x = distanceWihReference;
            }
            if (selectedTheta)
            {
                aditionalThetaRotation += mouseScrollDelta * Mathf.Deg2Rad;
                cylindricalAditionToPosition.z = aditionalThetaRotation;
            }
            if (selectedY)
            {
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, up, transform.position, out _, out Vector3 currentYSelectedPosition);
                currentYSelectedPosition = GetPositionWithReferencial(transform, currentYSelectedPosition);
                float distanceWihReference = Vector3.Dot(currentYSelectedPosition - selectedYPosition, GetDirectionWithReferencial(transform, up));
                cylindricalAditionToPosition.y = distanceWihReference;
            }
            transform.position = ReturnPositionFromReferencial(transform, FromCylindricalToCartesian(initialLocalPositionCylindrical + cylindricalAditionToPosition));
        }
        public override bool IsSelected()
        {
            return selectedRadius || selectedTheta || selectedY;
        }

        public override void Reset()
        {
            aditionalThetaRotation = 0f;

            selectedRadius = false;
            selectedTheta = false;
            selectedY = false;
        }
    }
}
