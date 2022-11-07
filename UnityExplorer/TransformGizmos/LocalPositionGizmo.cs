using UnityEngine;

namespace UnityExplorer.TransformGizmos
{
    public class LocalPositionGizmo : BaseInteractablePositionGizmo
    {
        public override void Set(Transform transform)
        {
            LineLenght = 1.1f;
            HeadLenght = 0.25f;
            base.Set(transform);
        }
        public override Vector3 GetXDirection() 
        {
            if (transform.parent == null)
                return Vector3.right;

            return transform.parent.right;
        }

        public override Vector3 GetYDirection()
        {
            if (transform.parent == null)
                return Vector3.up;

            return transform.parent.up;
        }

        public override Vector3 GetZDirection()
        {
            if (transform.parent == null)
                return Vector3.forward;

            return transform.parent.forward;
        }
    }
}
