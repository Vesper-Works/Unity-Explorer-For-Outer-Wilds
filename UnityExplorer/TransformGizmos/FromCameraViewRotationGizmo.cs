using UnityEngine;
using UnityExplorer.GLDrawHelpers;

namespace UnityExplorer.TransformGizmos
{
    public class FromCameraViewRotationGizmo : BaseTransformGizmo
    {
        bool selectedWheel = false;

        float deltaRotation;

        Quaternion initialLocalRotation;


        protected Vector3 GetDirectionWithReferencial(Transform t, Vector3 direction)
        {
            if (t.parent == null)
                return direction;

            return t.parent.InverseTransformDirection(direction);
        }

        Vector3 GetCircleDirection(Transform t) 
        {
            return (Locator.GetActiveCamera().transform.position - t.position).normalized;
        }

        public override bool CheckSelected(Ray ray, Transform t, float maxDistanceToSelect)
        {

            if (Vector3MathUtils.GetClosestPointFromLineToCircle(ray.direction, ray.origin, t.position, GetCircleDirection(t), 1.2f, out bool lineParalelToCircle, out _, out _) <= maxDistanceToSelect)
            {
                if (!lineParalelToCircle)
                {
                    selectedWheel = true;
                }
            }

            initialLocalRotation = t.localRotation;
            deltaRotation = 0f;

            return IsSelected();
        }

        public override bool IsSelected()
        {
            return selectedWheel;
        }

        public override void OnRender(Transform t)
        {

            GLHelper.DrawOnGlobalReference(() =>
            {
                Vector3 normal = GetCircleDirection(t);
                
                GLDraw.WireframeCircle(1.2f, normal, Vector3MathUtils.GetArbitraryPerpendicularVector(normal), t.position, selectedWheel ? Color.Lerp(Color.blue, Color.white, 0.8f) : Color.blue, 16);
            });
        }

        public override void OnSelected(Ray ray, Transform t)
        {
            float mouseScrollDelta = UniverseLib.Input.InputManager.MouseScrollDelta.y * 0.1f;
            if (selectedWheel)
            {
                deltaRotation += mouseScrollDelta;
            }

            t.localRotation = initialLocalRotation * Quaternion.AngleAxis(deltaRotation, GetDirectionWithReferencial(t, GetCircleDirection(t)));
        }

        public override void Reset()
        {
            selectedWheel = false;
        }
    }
}

