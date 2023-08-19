using UnityEngine;
using UnityExplorer.GizmoControls;

namespace UnityExplorer.TransformGizmos
{
    public abstract class BaseInteractablePositionGizmo : BaseTransformGizmo
    {
        ArrowControl x = new ArrowControl();
        ArrowControl y = new ArrowControl();
        ArrowControl z = new ArrowControl();

        Vector3 initialPosition;

        protected float LineLenght;
        protected float HeadLenght;

        public override void SetScale(float scale)
        {
            base.SetScale(scale);
            x.Scale = scale;
            y.Scale = scale;
            z.Scale = scale;
        }

        public void SetLineLenght(float lenght, float headLenght)
        {
            LineLenght = lenght;
            HeadLenght = headLenght;

            x.Length = lenght;
            x.HeadLength = headLenght;
            y.Length = lenght;
            y.HeadLength = headLenght;
            z.Length = lenght;
            z.HeadLength = headLenght;
        }
        public abstract Vector3 GetXDirection();
        public abstract Vector3 GetYDirection();
        public abstract Vector3 GetZDirection();

        public override void Set(Transform transform)
        {
            //TODO make it not have to always change and do all this for every ArrowControl
            //maybe have the gizmos have a reference for their BaseTransformGizmo 
            // and then they can just get the selectedTransform from it
            //and create a Init method to not call non Set stuff here (like x.Color or x.Direction)

            base.Set(transform);

            x.Length = LineLenght;
            x.HeadLength = HeadLenght;
            x.Transform = transform;
            x.Color = Color.red;
            x.SelectedColor = Color.Lerp(Color.red, Color.white, 0.8f);
            x.Direction = (t) => GetXDirection();

            y.Length = LineLenght;
            y.HeadLength = HeadLenght;
            y.Transform = transform;
            y.Color = Color.yellow;
            y.SelectedColor = Color.Lerp(Color.yellow, Color.white, 0.8f);
            y.Direction = (t) => GetYDirection();

            z.Length = LineLenght;
            z.HeadLength = HeadLenght;
            z.Transform = transform;
            z.Color = Color.cyan;
            z.SelectedColor = Color.Lerp(Color.cyan, Color.white, 0.8f);
            z.Direction = (t) => GetZDirection();
        }

        public override bool CheckSelected(Ray ray, float maxDistanceToSelect)
        {
            x.MaxDistanceToSelect = maxDistanceToSelect;
            y.MaxDistanceToSelect = maxDistanceToSelect;
            z.MaxDistanceToSelect = maxDistanceToSelect;

            x.IsSelected(ray);
            y.IsSelected(ray);
            z.IsSelected(ray);

            initialPosition = Vector3MathUtils.ParentInverseTransformPoint(transform, transform.position);

            return IsSelected();
        }

        public override void OnRender()
        {
            x.Draw();
            y.Draw();
            z.Draw();
        }
        public override void OnSelected(Ray ray)
        {
            Vector3 aditionToPosition = Vector3.zero;

            if (x.Selected)
                aditionToPosition += x.GetValue(ray);
            if (y.Selected)
                aditionToPosition += y.GetValue(ray);
            if (z.Selected)
                aditionToPosition += z.GetValue(ray);

            transform.position = Vector3MathUtils.ParentTransformPoint(transform, initialPosition + aditionToPosition);
        }
        public override bool IsSelected()
        {
            return x.Selected || y.Selected || z.Selected;
        }

        public override void Reset()
        {
            x.Reset();
            y.Reset();
            z.Reset();
        }
    }
}
