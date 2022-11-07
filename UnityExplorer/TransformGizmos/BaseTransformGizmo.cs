using UnityEngine;

namespace UnityExplorer.TransformGizmos
{
    public abstract class BaseTransformGizmo 
    {
        protected Transform transform;

        protected float Scale;
        public virtual void SetScale(float scale)
        {
            Scale = scale;
        }

        public virtual void Set(Transform transform) 
        {
            this.transform = transform;
        }
        public abstract void Reset();

        public abstract bool CheckSelected(Ray ray,float maxDistanceToSelect);

        public abstract void OnSelected(Ray ray);
        public abstract bool IsSelected();
        public abstract void OnRender();
    }
}
