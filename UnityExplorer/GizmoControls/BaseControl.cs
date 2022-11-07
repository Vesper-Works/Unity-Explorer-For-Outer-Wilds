using UnityEngine;

namespace UnityExplorer.GizmoControls
{
    public abstract class BaseControl<T>
    {
        public float Scale = 1f;
        public bool Selected { get; protected set; } = false;
        public abstract bool IsSelected(Ray ray);
        public abstract void Draw();
        public abstract T GetValue(Ray ray);
        public virtual void Reset() 
        {
            Selected = false;
        }
    }
}
