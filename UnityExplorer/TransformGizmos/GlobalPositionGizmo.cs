using UnityEngine;
using UnityExplorer.GLDrawHelpers;

namespace UnityExplorer.TransformGizmos
{
    public class GlobalPositionGizmo : BaseTransformGizmo
    {
        public override bool CheckSelected(Ray ray, float maxDistanceToSelect)
        {
            return false;
        }

        public override bool IsSelected()
        {
            return false;
        }

        public override void OnRender()
        {
            GLHelper.DrawOnGlobalReference();
            
            GLDraw.Vector(Vector3.right, 0.25f, transform.position, Color.red);
            GLDraw.Vector(Vector3.up, 0.25f, transform.position, Color.yellow);
            GLDraw.Vector(Vector3.forward, 0.25f, transform.position, Color.cyan);

            GLHelper.FinishDraw();
        }

        public override void OnSelected(Ray ray)
        {
            return;
        }

        public override void Reset()
        {
            return;
        }
    }
}
