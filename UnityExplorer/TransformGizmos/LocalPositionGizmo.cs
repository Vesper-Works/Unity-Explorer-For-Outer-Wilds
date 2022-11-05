using UnityEngine;

namespace UnityExplorer.TransformGizmos
{
    public class LocalPositionGizmo : BaseInteractablePositionGizmo
    {
        public override Vector3 GetXDirection(Transform t)
        {
            return t.parent.right;
        }

        public override Vector3 GetYDirection(Transform t)
        {
            return t.parent.up;
        }

        public override Vector3 GetZDirection(Transform t)
        {
            return t.parent.forward;
        }

        public override float LineSegmentDistance()
        {
            return 0.25f;
        }
    }
}
