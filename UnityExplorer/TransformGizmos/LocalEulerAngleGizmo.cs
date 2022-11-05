using UnityEngine;
using UnityExplorer.GLDrawHelpers;

namespace UnityExplorer.TransformGizmos
{
    public class LocalEulerAngleGizmo : BaseTransformGizmo
    {
        bool selectedXWheel = false;
        bool selectedYWheel = false;
        bool selectedZWheel = false;

        float xDeltaLocalRotation;
        float yDeltaLocalRotation;
        float zDeltaLocalRotation;

        Vector3 initialLocalEulerRotation;


        public override bool CheckSelected(Ray ray, Transform t, float maxDistanceToSelect)
        {
            Vector3 localFowardAxis = t.forward;
            Vector3 yRotationAxis = Vector3.Cross(t.parent.up, localFowardAxis);
            if (yRotationAxis.ApproxEquals(Vector3.zero))
            {
                yRotationAxis = t.right;
            }

            if (Vector3MathUtils.GetClosestPointFromLineToCircle(ray.direction, ray.origin, t.position, yRotationAxis, 1f, out bool lineParalelToCircle, out _, out _) <= maxDistanceToSelect)
            {
                if (!lineParalelToCircle)
                {
                    selectedXWheel = true;
                }
            }
            if (Vector3MathUtils.GetClosestPointFromLineToCircle(ray.direction, ray.origin, t.position, t.parent.up, 1f, out lineParalelToCircle, out _, out _) <= maxDistanceToSelect)
            {
                if (!lineParalelToCircle)
                {
                    selectedYWheel = true;
                }
            }
            if (Vector3MathUtils.GetClosestPointFromLineToCircle(ray.direction, ray.origin, t.position, localFowardAxis, 1f, out lineParalelToCircle, out _, out _) <= maxDistanceToSelect)
            {
                if (!lineParalelToCircle)
                {
                    selectedZWheel = true;
                }
            }

            xDeltaLocalRotation = 0f;
            yDeltaLocalRotation = 0f;
            zDeltaLocalRotation = 0f;
            initialLocalEulerRotation = t.localEulerAngles;

            return IsSelected();
        }

        public override bool IsSelected()
        {
            return selectedXWheel || selectedYWheel || selectedZWheel;
        }

        public override void OnRender(Transform t)
        {

            GLHelper.DrawWithReference(t.parent, () =>
            {
                Vector3 localFowardAxis = t.parent.InverseTransformDirection(t.forward);
                Vector3 localRightAxis = t.parent.InverseTransformDirection(t.right);

                //Local Euler Rotation
                Vector3 yRotationAxis = Vector3.Cross(Vector3.up, localFowardAxis);
                if (yRotationAxis.ApproxEquals(Vector3.zero))
                {
                    yRotationAxis = localRightAxis;
                }

                GLDraw.WireframeCircle(1f, yRotationAxis, localFowardAxis, t.localPosition, selectedXWheel ? Color.Lerp(Color.red, Color.white, 0.8f) : Color.red, 16);
                GLDraw.WireframeCircle(1f, Vector3.up, Vector3.forward, t.localPosition, selectedYWheel ? Color.Lerp(Color.yellow, Color.white, 0.8f) : Color.yellow, 16);
                GLDraw.WireframeCircle(1f, localFowardAxis, t.parent.InverseTransformDirection(t.up), t.localPosition, selectedZWheel ? Color.Lerp(Color.cyan, Color.white, 0.8f) : Color.cyan, 16);
            });
        }

        public override void OnSelected(Ray ray, Transform t)
        {
            float mouseScrollDelta = UniverseLib.Input.InputManager.MouseScrollDelta.y * 0.1f;
            if (selectedXWheel)
            {
                xDeltaLocalRotation += mouseScrollDelta ;
            }
            if (selectedYWheel)
            {
                yDeltaLocalRotation += mouseScrollDelta;
            }
            if (selectedZWheel)
            {
                zDeltaLocalRotation += mouseScrollDelta;
            }
            t.localEulerAngles = initialLocalEulerRotation + new Vector3(xDeltaLocalRotation, yDeltaLocalRotation, zDeltaLocalRotation);
        }

        public override void Reset()
        {
            selectedXWheel = false;
            selectedYWheel = false;
            selectedZWheel = false;
        }
    }
}
