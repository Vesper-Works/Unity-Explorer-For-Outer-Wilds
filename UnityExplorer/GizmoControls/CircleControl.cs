using System;
using UnityEngine;
using UnityExplorer.GLDrawHelpers;

namespace UnityExplorer.GizmoControls
{
    public class CircleControl : BaseControl<float>
    {
        private Vector3 selectedPosition;

        private Vector3 selectedFowardDirection;

        public Transform Transform;

        public float DistanceForFullRevolution = 4f;

        public Func<Transform, Vector3> Normal = (t) => Vector3.up;

        public float Radius;

        public float MaxDistanceToSelect;

        public Color Color;
        public Color SelectedColor;


        public override float GetValue(Ray ray)
        {
            Vector3 foward = Vector3MathUtils.ParentTransformDirection(Transform, selectedFowardDirection);

            Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, foward, Transform.position, out _, out Vector3 currentXSelectedPosition);
            currentXSelectedPosition = Vector3MathUtils.ParentInverseTransformPoint(Transform, currentXSelectedPosition);
            float distanceWihReference = Vector3.Dot(currentXSelectedPosition - selectedPosition, selectedFowardDirection);
            return distanceWihReference * 360f / DistanceForFullRevolution;
        }

        public override bool IsSelected(Ray ray)
        {
            Vector3 normal = Normal(Transform);
            if (Vector3MathUtils.GetClosestPointFromLineToCircle(ray.direction, ray.origin, Transform.position, normal, Radius * Scale, out bool lineParalelToCircle, out bool isEquidistant, out Vector3 selectedPoint) <= MaxDistanceToSelect)
            {
                if (!lineParalelToCircle && !isEquidistant)
                {
                    selectedPosition = selectedPoint;
                    selectedPosition = Vector3MathUtils.ParentInverseTransformPoint(Transform, selectedPosition);
                    selectedFowardDirection = Vector3.Cross(normal, (selectedPoint - Transform.position).normalized);
                    selectedFowardDirection = Vector3MathUtils.ParentInverseTransformDirection(Transform, selectedFowardDirection);
                    Selected = true;
                }
            }
            return Selected;
        }
        public override void Draw()
        {
            Vector3 normal = Normal(Transform);
            GLHelper.DrawOnGlobalReference();

            Vector3 randomUp = Vector3MathUtils.GetArbitraryPerpendicularVector(normal);
            Color color = Selected ? SelectedColor : Color;
            GLDraw.WireframeCircle(Radius * Scale, normal, randomUp, Transform.position, color, 16);

            GLHelper.FinishDraw();
        }
    }
}
