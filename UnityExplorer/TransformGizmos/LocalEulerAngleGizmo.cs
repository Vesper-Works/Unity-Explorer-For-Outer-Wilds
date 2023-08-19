using UnityEngine;
using UnityExplorer.GizmoControls;

namespace UnityExplorer.TransformGizmos
{
    internal class LocalEulerAngleGizmo : BaseTransformGizmo
    {
        CircleControl x = new CircleControl();
        CircleControl y = new CircleControl();
        CircleControl z = new CircleControl();

        float distanceForFullRevolution = 4f;

        Vector3 initialLocalEulerRotation;

        public override void SetScale(float scale)
        {
            base.SetScale(scale);
            x.Scale = scale;
            y.Scale = scale;
            z.Scale = scale;
        }
        public override void Set(Transform transform) 
        {
            base.Set(transform);

            x.DistanceForFullRevolution = distanceForFullRevolution;
            x.Color = Color.red;
            x.SelectedColor = Color.Lerp(Color.red, Color.white, 0.8f);
            x.Radius = 1f;
            x.Transform = transform;
            x.Normal = (t) =>
            {
                Vector3 localFowardAxis = t.forward;
                Vector3 localUp = Vector3MathUtils.ParentTransformDirection(t, Vector3.up);
                Vector3 yRotationAxis = Vector3.Cross(localUp, localFowardAxis);
                if (yRotationAxis.ApproxEquals(Vector3.zero))
                {
                    yRotationAxis = t.right;
                }
                return yRotationAxis;
            };

            y.DistanceForFullRevolution = distanceForFullRevolution;
            y.Color = Color.yellow;
            y.SelectedColor = Color.Lerp(Color.yellow, Color.white, 0.8f);
            y.Radius = 1f;
            y.Transform = transform;
            y.Normal = (t) =>
            {
                Vector3 localUp = Vector3MathUtils.ParentTransformDirection(t, Vector3.up);
                return localUp;
            };

            z.DistanceForFullRevolution = distanceForFullRevolution;
            z.Color = Color.cyan;
            z.SelectedColor = Color.Lerp(Color.cyan, Color.white, 0.8f);
            z.Radius = 1f;
            z.Transform = transform;
            z.Normal = (t) =>
            {
                return t.forward;
            };
        }

        public override bool CheckSelected(Ray ray, float maxDistanceToSelect)
        {
            x.MaxDistanceToSelect = maxDistanceToSelect;
            y.MaxDistanceToSelect = maxDistanceToSelect;
            z.MaxDistanceToSelect = maxDistanceToSelect;

            x.IsSelected(ray);
            y.IsSelected(ray);
            z.IsSelected(ray);

            initialLocalEulerRotation = transform.localEulerAngles;

            return IsSelected();
        }

        public override bool IsSelected()
        {
            return x.Selected || y.Selected || z.Selected;
        }

        public override void OnRender()
        {
            x.Draw();
            y.Draw();
            z.Draw();
        }

        public override void OnSelected(Ray ray)
        {
            Vector3 aditionToAngle = Vector3.zero;

            if (x.Selected)
                aditionToAngle.x = x.GetValue(ray);
            if (y.Selected)
                aditionToAngle.y = y.GetValue(ray);
            if (z.Selected)
                aditionToAngle.z = z.GetValue(ray);

            transform.localEulerAngles = initialLocalEulerRotation + aditionToAngle;
        }

        public override void Reset()
        {
            x.Reset();
            y.Reset();
            z.Reset();
        }
    }
}
