using UnityEngine;

namespace UnityExplorer.TransformGizmos
{
    public class TransformOrientationGizmo : BaseInteractablePositionGizmo
    {
        public override void Set(Transform transform)
        {
            LineLenght = 1.1f;
            HeadLenght = 0.25f;
            base.Set(transform);
        }
        public override Vector3 GetXDirection()
        {
            return transform.right;
        }

        public override Vector3 GetYDirection()
        {
            return transform.up;
        }

        public override Vector3 GetZDirection()
        {
            return transform.forward;
        }
    }
}
