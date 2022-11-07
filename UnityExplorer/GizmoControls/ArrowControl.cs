using System;
using UnityEngine;
using UnityExplorer.GLDrawHelpers;
namespace UnityExplorer.GizmoControls
{
    internal class ArrowControl : BaseControl<Vector3>
    {
        private Vector3 selectedPosition;

        public Transform Transform;

        public Func<Transform, Vector3> Direction = (t) => Vector3.up;

        public float Lenght;
        public float HeadLenght;

        public float MaxDistanceToSelect;

        public Color Color;
        public Color SelectedColor;


        public override Vector3 GetValue(Ray ray)
        {
            Vector3 direction = Direction(Transform);
            Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, direction, Transform.position, out _, out Vector3 currentSelectedPosition);
            currentSelectedPosition = Vector3MathUtils.GetPositionWithReferencial(Transform, currentSelectedPosition);
            float distanceWihReference = Vector3.Dot(currentSelectedPosition - selectedPosition, Vector3MathUtils.GetDirectionWithReferencial(Transform, direction));
            
            return distanceWihReference * Vector3MathUtils.GetDirectionWithReferencial(Transform, direction);
        }

        public override bool IsSelected(Ray ray)
        {
            Vector3 direction = Direction(Transform);

            if (Vector3MathUtils.GetDistanceFromLineToLineSegment(ray.direction, ray.origin, Transform.position, Transform.position + direction * Lenght * Scale) <= MaxDistanceToSelect)
            {
                Selected = true;
                Vector3MathUtils.GetClosestPointFromLines(ray.direction, ray.origin, direction, Transform.position, out _, out selectedPosition);
                selectedPosition = Vector3MathUtils.GetPositionWithReferencial(Transform, selectedPosition);
            }
            return Selected;
        }
        public override void Draw()
        {
            Vector3 direction = Direction(Transform);

            GLHelper.DrawOnGlobalReference(() =>
            {
                Color color = Selected ? SelectedColor : Color;
                GLDraw.Vector(direction * Lenght * Scale, HeadLenght * Scale, Transform.position, color);
            });
        }
    }
}
