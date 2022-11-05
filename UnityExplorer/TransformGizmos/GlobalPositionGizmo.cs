using UnityEngine;
using UnityExplorer.GLDrawHelpers;

namespace UnityExplorer.TransformGizmos
{
    public class GlobalPositionGizmo : BaseTransformGizmo
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
                GLDraw.Vector(Vector3.right, 0.25f, t.position, Color.red);
                GLDraw.Vector(Vector3.up, 0.25f, t.position, Color.yellow);
                GLDraw.Vector(Vector3.forward, 0.25f, t.position, Color.cyan);
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
