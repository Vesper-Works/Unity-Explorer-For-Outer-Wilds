using UnityEngine;
using UnityExplorer.GizmoControls;

namespace UnityExplorer.TransformGizmos
{
    public class FromCameraViewRotationGizmo : BaseTransformGizmo
    {
        CircleControl control = new CircleControl();

        float distanceForFullRevolution = 4f;

        Vector3 rotationAxis;

        Quaternion initialLocalRotation;

        public override void SetScale(float scale)
        {
            base.SetScale(scale);
            control.Scale = scale;
        }

        public override void Set(Transform transform)
        {
            base.Set(transform);

            control.DistanceForFullRevolution = distanceForFullRevolution;
            control.Color = Color.blue;
            control.SelectedColor = Color.Lerp(Color.blue, Color.white, 0.8f);
            control.Radius = 1.5f;
            control.Transform = transform;
            control.Normal = (t) => rotationAxis;
        }

        public override bool CheckSelected(Ray ray, float maxDistanceToSelect)
        {
            control.MaxDistanceToSelect = maxDistanceToSelect;
            control.IsSelected(ray);

            rotationAxis = (Locator.GetActiveCamera().transform.position - transform.position).normalized;
            initialLocalRotation = transform.localRotation;

            return IsSelected();
        }

        public override bool IsSelected()
        {
            return control.Selected;
        }

        public override void OnRender()
        {
            if (!control.Selected) 
            {
                rotationAxis = (Locator.GetActiveCamera().transform.position - transform.position).normalized;
            }

            control.Draw();
        }

        public override void OnSelected(Ray ray)
        {
            float angle = control.GetValue(ray);

            transform.localRotation = initialLocalRotation * Quaternion.AngleAxis(angle, Vector3MathUtils.ParentInverseTransformDirection(transform, rotationAxis));
        }


        public override void Reset()
        {
            control.Reset();
        }
    }
}

