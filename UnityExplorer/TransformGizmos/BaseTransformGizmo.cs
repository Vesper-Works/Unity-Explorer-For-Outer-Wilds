using UnityEngine;

namespace UnityExplorer.TransformGizmos
{
    public abstract class BaseTransformGizmo 
    {
        public abstract void Reset();

        public abstract bool CheckSelected(Ray ray, Transform t, float maxDistanceToSelect);

        public abstract void OnSelected(Ray ray, Transform t);
        public abstract bool IsSelected();
        public abstract void OnRender(Transform t);
    }
}
