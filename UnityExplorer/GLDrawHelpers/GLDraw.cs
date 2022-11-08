using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityExplorer.GLDrawHelpers
{

    public static class GLDraw
    {

        /// <summary>
        /// Draws a cube that follows the Vector.foward, Vector.up and Vector.right axis with center in offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        public static void SimpleWireframeCube(Vector3 offset, Color color)
        {
            WireframeCube(Vector3.forward, Vector3.up, Vector3.right, offset, color);
        }

        /// <summary>
        /// Draws a cube that follows the specified axis with center in offset.
        /// </summary>
        /// <param name="foward"></param>
        /// <param name="up"></param>
        /// <param name="right"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        public static void WireframeCube(Vector3 foward, Vector3 up, Vector3 right, Vector3 offset, Color color)
        {
            //There is probabilly a better way to do this
            Vector3 cubeSideFR = (foward + right) / 2f;
            Vector3 cubeSideMFR = (-foward + right) / 2f;
            Vector3 cubeSideMFMR = (-foward - right) / 2f;
            Vector3 cubeSideFMR = (foward - right) / 2f;

            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            GL.Vertex(cubeSideFR + offset - up / 2f);
            GL.Vertex(cubeSideMFR + offset - up / 2f);
            GL.Vertex(cubeSideMFMR + offset - up / 2f);
            GL.Vertex(cubeSideFMR + offset - up / 2f);

            GL.Vertex(cubeSideFR + offset - up / 2f);
            GL.End();

            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);

            GL.Vertex(cubeSideFR + offset + up / 2f);
            GL.Vertex(cubeSideMFR + offset + up / 2f);
            GL.Vertex(cubeSideMFMR + offset + up / 2f);
            GL.Vertex(cubeSideFMR + offset + up / 2f);

            GL.Vertex(cubeSideFR + offset + up / 2f);
            GL.End();

            GL.Begin(GL.LINES);

            GL.Vertex(cubeSideFR + offset - up / 2f);
            GL.Vertex(cubeSideFR + offset + up / 2f);

            GL.Vertex(cubeSideMFR + offset - up / 2f);
            GL.Vertex(cubeSideMFR + offset + up / 2f);

            GL.Vertex(cubeSideMFMR + offset - up / 2f);
            GL.Vertex(cubeSideMFMR + offset + up / 2f);

            GL.Vertex(cubeSideFMR + offset - up / 2f);
            GL.Vertex(cubeSideFMR + offset + up / 2f);

            GL.End();
        }

        /// <summary>
        /// Draws a circle on a plane facing the specified normal, centered in offset with the angle 0 meaning the same direction as up.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="normal"></param>
        /// <param name="up"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        /// <param name="resolution"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="isWholeCircle">If it is a 2 PI circle you then need to set this to true</param>
        public static void WireframeCircle(float radius, Vector3 normal, Vector3 up, Vector3 offset, Color color, int resolution = 3, float startAngle = 0f, float endAngle = 2f * Mathf.PI, bool isWholeCircle = true)
        {
            if (resolution < 3 || radius <= 0f)
                return;
            normal = normal.normalized;
            up = up.normalized;

            GL.Begin(GL.LINE_STRIP);

            float angleStep = (endAngle - startAngle) / resolution;
            int aditionalSteps = isWholeCircle ? 1 : 0;

            GL.Color(color);
            Vector3 rotationVector = Vector3MathUtils.GetRotationVector(normal, up);
            for (int i = 0; i <= resolution + aditionalSteps; i++)
            {
                Vector3 radiusVector = Vector3MathUtils.GetRotatedVectorComponent(rotationVector, up, angleStep * i + startAngle);
                GL.Vertex(radiusVector * radius + offset);
            }
            GL.End();
        }

        /// <summary>
        /// Draws a sphere centered on offset following the Vector.foward, Vector.up and Vector.right axis.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        /// <param name="resolution"></param>
        public static void SimpleWireframeSphere(float radius, Vector3 offset, Color color, int resolution)
        {
            WireframeCircle(radius, Vector3.up, Vector3.forward, offset, color, resolution);
            WireframeCircle(radius, Vector3.forward, Vector3.right, offset, color, resolution);
            WireframeCircle(radius, Vector3.right, Vector3.up, offset, color, resolution);
        }

        /// <summary>
        /// Draws a sphere centered on offset following the specified axis.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="offset"></param>
        /// <param name="foward"></param>
        /// <param name="up"></param>
        /// <param name="color"></param>
        /// <param name="resolution"></param>
        public static void WireframeSphere(float radius, Vector3 offset, Vector3 foward, Vector3 up, Color color, int resolution = 3)
        {
            Vector3 right = Vector3.Cross(foward, up);
            WireframeCircle(radius, up, foward, offset, color, resolution);
            WireframeCircle(radius, foward, right, offset, color, resolution);
            WireframeCircle(radius, right, up, offset, color, resolution);
        }

        /// <summary>
        /// Draws half a sphere centered on offset facing the up direction.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="offset"></param>
        /// <param name="foward"></param>
        /// <param name="up"></param>
        /// <param name="color"></param>
        /// <param name="resolution"></param>
        public static void WireframeHemisphere(float radius, Vector3 offset, Vector3 foward, Vector3 up, Color color, int resolution = 3)
        {
            Vector3 right = Vector3.Cross(foward, up);
            WireframeCircle(radius, up, foward, offset, color, resolution, -Mathf.PI / 2f, Mathf.PI / 2f, false);
            WireframeCircle(radius, foward, right, offset, color, resolution);
            WireframeCircle(radius, right, foward, offset, color, resolution, -Mathf.PI / 2f, Mathf.PI / 2f, false);
        }

        /// <summary>
        /// Draws a capsule with the startPoint and endPoint being the center of the spheres on its extreme.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="color"></param>
        /// <param name="resolution"></param>
        public static void WireframeCapsule(float radius, Vector3 startPoint, Vector3 endPoint, Color color, int resolution = 3)
        {
            Vector3 direction = startPoint - endPoint;
            Vector3 randomUpVector = Vector3MathUtils.GetArbitraryPerpendicularVector(direction);

            //Top and bottom Spheres
            WireframeHemisphere(radius, startPoint, direction, randomUpVector, color, resolution);
            WireframeHemisphere(radius, endPoint, -direction, -randomUpVector, color, resolution);

            GL.Begin(GL.LINES);
            float angleStep = 2f * Mathf.PI / resolution;
            Vector3 rotationVector = Vector3MathUtils.GetRotationVector(direction.normalized, randomUpVector);
            for (int i = 0; i <= resolution; i++)
            {
                Vector3 radiusVector = Vector3MathUtils.GetRotatedVectorComponent(rotationVector, randomUpVector, angleStep * i);
                Vector3 vertex1 = radiusVector * radius + startPoint;
                Vector3 vertex2 = radiusVector * radius + endPoint;

                GL.Color(color);
                GL.Vertex(vertex1);
                GL.Vertex(vertex2);
            }
            GL.End();
        }

        /// <summary>
        /// Draws a capsule oriented with the Vector.up direction.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="up"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        /// <param name="resolution"></param>
        public static void SimpleWireframeCapsule(float radius, float height, Vector3 up, Vector3 offset, Color color, int resolution = 3)
        {
            Vector3 randomFowardVector = Vector3MathUtils.GetArbitraryPerpendicularVector(up);
            //Top and bottom Spheres
            WireframeSphere(radius, offset - up * height / 2f, randomFowardVector, up, color, resolution);
            WireframeSphere(radius, offset + up * height / 2f, randomFowardVector, up, color, resolution);

            GL.Begin(GL.LINES);
            float angleStep = 2f * Mathf.PI / resolution;
            Vector3 rotationVector = Vector3MathUtils.GetRotationVector(up.normalized, randomFowardVector);
            for (int i = 0; i <= resolution; i++)
            {
                Vector3 radiusVector = Vector3MathUtils.GetRotatedVectorComponent(rotationVector, randomFowardVector, angleStep * i);
                Vector3 vertex1 = radiusVector * radius + offset - up * height / 2f;
                Vector3 vertex2 = radiusVector * radius + offset + up * height / 2f;

                GL.Color(color);
                GL.Vertex(vertex1);
                GL.Vertex(vertex2);
            }
            GL.End();
        }

        /// <summary>
        /// Draws a cone with specifiend star and end positions as well as start and end radius (for cone trunks). It can be used to draw cylinder too.
        /// </summary>
        /// <param name="coneRadiusStart"></param>
        /// <param name="coneRadiusEnd"></param>
        /// <param name="coneStart"></param>
        /// <param name="coneEnd"></param>
        /// <param name="color"></param>
        /// <param name="resolution"></param>
        public static void WireframeCone(float coneRadiusStart, float coneRadiusEnd, Vector3 coneStart, Vector3 coneEnd, Color color, int resolution = 3)
        {
            Vector3 direction = coneEnd - coneStart;
            Vector3 randomFowardVector = Vector3MathUtils.GetArbitraryPerpendicularVector(direction);
            //Start Circle
            WireframeCircle(coneRadiusStart, direction, randomFowardVector, coneStart, color, resolution);
            //End Circle
            WireframeCircle(coneRadiusEnd, direction, randomFowardVector, coneEnd, color, resolution);
            //Connecting Lines
            GL.Begin(GL.LINES);
            float angleStep = 2f * Mathf.PI / resolution;
            Vector3 rotationVector = Vector3MathUtils.GetRotationVector(direction.normalized, randomFowardVector);
            for (int i = 0; i <= resolution; i++)
            {
                Vector3 radiusVector = Vector3MathUtils.GetRotatedVectorComponent(rotationVector, randomFowardVector, angleStep * i);
                Vector3 vertex1 = radiusVector * coneRadiusStart + coneStart;
                Vector3 vertex2 = radiusVector * coneRadiusEnd + coneEnd;

                GL.Color(color);
                GL.Vertex(vertex1);
                GL.Vertex(vertex2);
            }
            GL.End();

        }

        /// <summary>
        /// Draws the direction of a vector as a 3d arrow starting on the offset.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="headSize"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        public static void Vector(Vector3 vector, float headSize, Vector3 offset, Color color)
        {
            if (vector.magnitude <= 0f)
                return;

            GL.Begin(GL.LINES);
            GL.Color(color);
            //Main arrow body
            GL.Vertex(offset);
            Vector3 endPoint = vector + offset;
            GL.Vertex(endPoint);
            //Head of the arrow
            Vector3 direction = vector.normalized;
            Vector3 firstHeadDirection = Vector3MathUtils.GetArbitraryPerpendicularVector(vector);
            Vector3 secondHeadDirection = Vector3.Cross(direction, firstHeadDirection);

            GL.Vertex(endPoint);
            GL.Vertex((firstHeadDirection - direction).normalized * headSize + endPoint);

            GL.Vertex(endPoint);
            GL.Vertex(-(firstHeadDirection + direction).normalized * headSize + endPoint);

            GL.Vertex(endPoint);
            GL.Vertex((secondHeadDirection - direction).normalized * headSize + endPoint);

            GL.Vertex(endPoint);
            GL.Vertex(-(secondHeadDirection + direction).normalized * headSize + endPoint);

            GL.End();
        }
    }
}
