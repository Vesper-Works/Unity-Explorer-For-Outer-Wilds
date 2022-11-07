using UnityEngine;

using UnityExplorer.GLDrawHelpers;
using UnityExplorer.Inspectors;
using UnityExplorer.TransformGizmos;

namespace UnityExplorer
{
    public class ExtendedTransformTools : MonoBehaviour
    {
        Transform selectedTransform;

        float maxDistanceToSelect = 0.05f;

        LocalPositionGizmo localPositionGizmo = new();
        LocalEulerAngleGizmo localEulerAngleGizmo = new();
        FromCameraViewRotationGizmo fromCameraViewRotationGizmo = new();
        TransformOrientationGizmo transformOrientationGizmo = new();

        //TODO add hotkey to enable gizmos
        //And maybe settings for choosing them on the configs
        private void Update()
        {
            var inspector = InspectorManager.ActiveInspector;

            if (inspector is not GameObjectInspector goInspector)
            {
                localPositionGizmo.Reset();
                localEulerAngleGizmo.Reset();
                transformOrientationGizmo.Reset();
                fromCameraViewRotationGizmo.Reset();
                return;
            }
            if(goInspector.Target == null)
            {
                localPositionGizmo.Reset();
                localEulerAngleGizmo.Reset();
                transformOrientationGizmo.Reset();
                fromCameraViewRotationGizmo.Reset();
                return;
            }

            selectedTransform = goInspector.Target.transform;

            float scale = Vector3.Distance(Locator.GetActiveCamera().transform.position, selectedTransform.position) / 8f;

            localPositionGizmo.SetScale(scale);
            localEulerAngleGizmo.SetScale(scale);
            transformOrientationGizmo.SetScale(scale);
            fromCameraViewRotationGizmo.SetScale(scale);

            localPositionGizmo.Set(selectedTransform);
            localEulerAngleGizmo.Set(selectedTransform);
            transformOrientationGizmo.Set(selectedTransform);
            fromCameraViewRotationGizmo.Set(selectedTransform);


            Ray ray = Locator.GetActiveCamera().ScreenPointToRay(UniverseLib.Input.InputManager.MousePosition);

            if (UniverseLib.Input.InputManager.GetMouseButtonDown(0))
            {
                if (!(localPositionGizmo.IsSelected() || localEulerAngleGizmo.IsSelected() || transformOrientationGizmo.IsSelected() || fromCameraViewRotationGizmo.IsSelected()))
                {
                    
                    localPositionGizmo.CheckSelected(ray, maxDistanceToSelect * scale);
                    //TODO create a way to have selection priority (like in this if)
                    if (!localPositionGizmo.IsSelected())
                    {
                        transformOrientationGizmo.CheckSelected(ray, maxDistanceToSelect * scale);
                    }
                    //TODO create a way to have incompatible gizmos not being selected at the same time
                    if (!transformOrientationGizmo.IsSelected())
                    {
                        localEulerAngleGizmo.CheckSelected(ray, maxDistanceToSelect * scale);
                    }
                    if (!localEulerAngleGizmo.IsSelected())
                    {
                        fromCameraViewRotationGizmo.CheckSelected(ray, maxDistanceToSelect * scale);
                    }
                }
            }
            else if (UniverseLib.Input.InputManager.GetMouseButton(0))
            {
                if (localPositionGizmo.IsSelected())
                {
                    localPositionGizmo.OnSelected(ray);
                }
                if (transformOrientationGizmo.IsSelected())
                {
                    transformOrientationGizmo.OnSelected(ray);
                }
                if (localEulerAngleGizmo.IsSelected())
                {
                    localEulerAngleGizmo.OnSelected(ray);
                }
                if (fromCameraViewRotationGizmo.IsSelected())
                {
                    fromCameraViewRotationGizmo.OnSelected(ray);
                }
            }
            else
            {
                localPositionGizmo.Reset();
                localEulerAngleGizmo.Reset();
                transformOrientationGizmo.Reset();
                fromCameraViewRotationGizmo.Reset();
            }

        }
        public void OnRenderObject()
        {
            if (selectedTransform == null)
                return;
            GL.wireframe = true;

            GLHelper.SetDefaultMaterialPass(0, true);

            localPositionGizmo.OnRender();
            localEulerAngleGizmo.OnRender();
            transformOrientationGizmo.OnRender();
            fromCameraViewRotationGizmo.OnRender();

            GL.wireframe = false;
        }
    }
}
