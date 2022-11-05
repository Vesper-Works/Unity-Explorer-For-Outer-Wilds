using UnityEngine;
using UnityExplorer.GLDrawHelpers;

namespace UnityExplorer.TransformGizmos
{
    public class GlobalEulerAnglesGizmo : BaseTransformGizmo
    {
        public override bool CheckSelected(Ray ray, Transform t, float maxDistanceToSelect)
        {
            return false;
        }

        public override bool IsSelected()
        {
            return false;
        }

        public override void OnRender(Transform t)
        {
            GLHelper.DrawOnGlobalReference(() =>
            {
                //Global position axis
                Vector3 localFowardAxis = t.forward;
                Vector3 localRightAxis = t.right;

                //Local Euler Rotation
                Vector3 yRotationAxis = Vector3.Cross(Vector3.up, localFowardAxis);
                if (yRotationAxis.ApproxEquals(Vector3.zero))
                {
                    yRotationAxis = localRightAxis;
                }

                GLDraw.WireframeCircle(1f, yRotationAxis, localFowardAxis, t.position, Color.red, 16);
                GLDraw.WireframeCircle(1f, Vector3.up, Vector3.forward, t.position, Color.yellow, 16);
                GLDraw.WireframeCircle(1f, localFowardAxis, t.up, t.position, Color.cyan, 16);
            });
        }

        public override void OnSelected(Ray ray, Transform t)
        {
            return;
        }

        public override void Reset()
        {
            return;
        }
    }
}
