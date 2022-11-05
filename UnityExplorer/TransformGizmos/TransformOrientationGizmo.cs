using UnityEngine;

namespace UnityExplorer.TransformGizmos
{
    public class TransformOrientationGizmo : BaseInteractablePositionGizmo
    {
        public override Vector3 GetXDirection(Transform t)
        {
            return t.right;
        }

        public override Vector3 GetYDirection(Transform t)
        {
            return t.up;
        }

        public override Vector3 GetZDirection(Transform t)
        {
            return t.forward;
        }

        public override float LineSegmentDistance()
        {
            return 0.25f;
        }
    }
}
