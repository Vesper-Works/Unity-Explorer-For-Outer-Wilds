using System;
using UnityEngine;

namespace UnityExplorer.GLDrawHelpers
{
    public static class GLHelper
    {
        static Material defaultGizmosMaterial;
        public static Material GetDefaultMaterial()
        {
            if (!defaultGizmosMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                defaultGizmosMaterial = new Material(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                // Turn on alpha blending
                defaultGizmosMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                defaultGizmosMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                defaultGizmosMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                defaultGizmosMaterial.SetInt("_ZWrite", 0);
            }
            return defaultGizmosMaterial;
        }
        public static void SetDefaultMaterialPass(int pass = 0)
        {
            GetDefaultMaterial().SetPass(pass);
        }
        public static void DrawWithReference(Transform reference, Action drawMethod)
        {
            GL.PushMatrix();
            GL.MultMatrix(reference.localToWorldMatrix);
            drawMethod?.Invoke();
            GL.PopMatrix();
        }
        public static void DrawOnGlobalReference(Action drawMethod)
        {
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one));
            drawMethod?.Invoke();
            GL.PopMatrix();
        }
        public static void DrawWithOrthoProjection(Action drawMethod)
        {
            GL.PushMatrix();
            GL.LoadOrtho();
            drawMethod?.Invoke();
            GL.PopMatrix();
        }

        public static void DrawAxis(float headSize, Color color, Vector3 offset)
        {
            GLDraw.Vector(Vector3.up, headSize, offset, color);
            GLDraw.Vector(Vector3.forward, headSize, offset, color);
            GLDraw.Vector(Vector3.right, headSize, offset, color);
        }
        public static void DrawAxis(float headSize, Color upColor, Color fowardColor, Color rightColor, Vector3 offset)
        {
            GLDraw.Vector(Vector3.up, headSize, offset, upColor);
            GLDraw.Vector(Vector3.forward, headSize, offset, fowardColor);
            GLDraw.Vector(Vector3.right, headSize, offset, rightColor);
        }
        public static void DrawTransform(Transform transform, float headSize, Color color)
        {
            GLDraw.Vector(transform.up * transform.lossyScale.x, headSize, transform.position, color);
            GLDraw.Vector(transform.forward * transform.lossyScale.z, headSize, transform.position, color);
            GLDraw.Vector(transform.right * transform.lossyScale.y, headSize, transform.position, color);
        }
        public static void DrawTransform(Transform transform, float headSize, Color upColor, Color fowardColor, Color rightColor)
        {
            GLDraw.Vector(transform.up * transform.lossyScale.x, headSize, transform.position, upColor);
            GLDraw.Vector(transform.forward * transform.lossyScale.z, headSize, transform.position, fowardColor);
            GLDraw.Vector(transform.right * transform.lossyScale.y, headSize, transform.position, rightColor);
        }
        public static void DrawColliderBoundingBox(Collider collider, Color color)
        {
            Bounds box = collider.bounds;
            GLDraw.WireframeCube(Vector3.forward * box.size.z, Vector3.up * box.size.y, Vector3.right * box.size.z, box.center, color);
        }
        public static void DrawCollider(Collider collider, Color color)
        {
            const int resolution = 12;
            if (collider is BoxCollider)
            {
                BoxCollider box = collider as BoxCollider;
                GLDraw.WireframeCube(Vector3.forward * box.size.z, Vector3.up * box.size.y, Vector3.right * box.size.z, box.center, color);
            }
            else if (collider is SphereCollider)
            {
                SphereCollider sphere = collider as SphereCollider;
                GLDraw.WireframeSphere(sphere.radius, sphere.center, Vector3.forward, Vector3.up, color, resolution);
            }
            else if (collider is CapsuleCollider)
            {
                CapsuleCollider capsule = collider as CapsuleCollider;
                GLDraw.WireframeCapsule(capsule.radius, capsule.center + capsule.height * Vector3.up / 2f, capsule.center - capsule.height * Vector3.up / 2f, color, resolution);
            }
        }
        public static void DrawShape(Shape shape, Color color)
        {
            const int resolution = 12;
            if (shape is SphereShape)
            {
                if (shape is HemisphereShape)
                {
                    HemisphereShape hemisphereShape = shape as HemisphereShape;
                    Vector3 hemisphereCenter = ShapeUtil.Sphere.CalcWorldSpaceCenter(hemisphereShape);
                    float hemisphereRadius = ShapeUtil.Sphere.CalcWorldSpaceRadius(hemisphereShape);
                    Vector3 hemisphereUp = ShapeUtil.Hemisphere.CalcWorldSpaceAxis(hemisphereShape);
                    GLDraw.WireframeHemisphere(hemisphereRadius, hemisphereCenter, hemisphereShape.transform.up, hemisphereUp, color, resolution);
                    return;
                }
                SphereShape sphereShape = shape as SphereShape;
                Vector3 sphereCenter = ShapeUtil.Sphere.CalcWorldSpaceCenter(sphereShape);
                float sphereRadius = ShapeUtil.Sphere.CalcWorldSpaceRadius(sphereShape);
                GLDraw.WireframeSphere(sphereRadius, sphereCenter, sphereShape.transform.forward, sphereShape.transform.up, color, resolution);
            }
            else if (shape is CapsuleShape)
            {
                if (shape is CylinderShape)
                {
                    CylinderShape cylinderShape = shape as CylinderShape;
                    ShapeUtil.Cylinder.CalcWorldSpaceEndpoints(cylinderShape, out float cylinderRadius, out Vector3 cylinderStart, out Vector3 cylinderEnd);
                    GLDraw.WireframeCone(cylinderRadius, cylinderRadius, cylinderStart, cylinderEnd, color, resolution);
                    return;
                }
                CapsuleShape capsuleShape = shape as CapsuleShape;
                ShapeUtil.Capsule.CalcWorldSpaceEndpoints(capsuleShape, out float capsuleRadius, out Vector3 capsuleStart, out Vector3 capsuleEnd);
                GLDraw.WireframeCapsule(capsuleRadius, capsuleStart, capsuleEnd, color, resolution);
            }
            else
            {
                if (shape is BoxShape)
                {
                    BoxShape boxShape = shape as BoxShape;
                    Vector3[] boxAxes = new Vector3[3];
                    Vector3[] verts = new Vector3[8];
                    ShapeUtil.Box.CalcWorldSpaceData(boxShape, out Vector3 boxCenter, out Vector3 boxSize, ref boxAxes, ref verts);
                    GLDraw.WireframeCube(boxAxes[2] * boxSize.z, boxAxes[1] * boxSize.y, boxAxes[0] * boxSize.x, boxCenter, color);
                    return;
                }
                if (shape is ConeShape)
                {
                    ConeShape coneShape = shape as ConeShape;
                    ShapeUtil.Cone.CalcWorldSpaceEndpoints(coneShape, out float coneRadiusStart, out float coneRadiusEnd, out Vector3 coneStart, out Vector3 coneEnd);
                    GLDraw.WireframeCone(coneRadiusStart, coneRadiusEnd, coneStart, coneEnd, color, resolution);
                }
            }
        }
        public static void DrawShapeBoundingSphere(Shape shape, Color color)
        {
            const int resolution = 12;
            SphereBounds bounds = shape.CalcWorldBounds();
            GLDraw.WireframeSphere(bounds.radius, bounds.center, Vector3.forward, Vector3.up, color, resolution);
        }
    }
}
