using UnityEngine;

using UnityExplorer.GLDrawHelpers;
using UnityExplorer.Inspectors;
using UnityExplorer.TransformGizmos;

namespace UnityExplorer
{
    public class ExtendedTransformTools : MonoBehaviour
    {
        private void Awake()
        {
        }

        private void Start()
        {

        }
        Transform selectedTransform;


        float maxDistanceToSelect = 0.05f;

        LocalPositionGizmo localPositionGizmo = new LocalPositionGizmo();
        LocalEulerAngleGizmo localEulerAngleGizmo = new LocalEulerAngleGizmo();
        FromCameraViewRotationGizmo fromCameraViewRotationGizmo = new FromCameraViewRotationGizmo();

        TransformOrientationGizmo transformOrientationGizmo = new TransformOrientationGizmo();

        GlobalEulerAnglesGizmo globalEulerAnglesGizmo = new GlobalEulerAnglesGizmo();

        GlobalPositionGizmo globalPositionGizmo = new GlobalPositionGizmo();


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


            
                Ray ray = Locator.GetActiveCamera().ScreenPointToRay(UniverseLib.Input.InputManager.MousePosition);

                if (UniverseLib.Input.InputManager.GetMouseButtonDown(0))
                {
                    if (!(localPositionGizmo.IsSelected() || localEulerAngleGizmo.IsSelected() || transformOrientationGizmo.IsSelected() || fromCameraViewRotationGizmo.IsSelected()))
                    {
                        if (selectedTransform.parent != null)
                        {
                            localPositionGizmo.CheckSelected(ray, selectedTransform, maxDistanceToSelect);
                        }
                        //TODO create a way to have selection priority (like in this if)
                        if (!localPositionGizmo.IsSelected()) 
                        {
                            transformOrientationGizmo.CheckSelected(ray, selectedTransform, maxDistanceToSelect);
                        }
                        //TODO create a way to have incompatible gizmos not being selected at the same time
                        if (!transformOrientationGizmo.IsSelected() && selectedTransform.parent != null)
                        {
                            localEulerAngleGizmo.CheckSelected(ray, selectedTransform, maxDistanceToSelect); 
                        }
                        if (!localEulerAngleGizmo.IsSelected()) 
                        {
                            fromCameraViewRotationGizmo.CheckSelected(ray, selectedTransform, maxDistanceToSelect);
                        }
                    }
                }
                else if (UniverseLib.Input.InputManager.GetMouseButton(0))
                {
                    if (localPositionGizmo.IsSelected())
                    {
                        localPositionGizmo.OnSelected(ray, selectedTransform);
                    }
                    if (transformOrientationGizmo.IsSelected())
                    {
                        transformOrientationGizmo.OnSelected(ray, selectedTransform);
                    }
                    if (localEulerAngleGizmo.IsSelected())
                    {
                        localEulerAngleGizmo.OnSelected(ray, selectedTransform);
                    }
                    if (fromCameraViewRotationGizmo.IsSelected()) 
                    {
                        fromCameraViewRotationGizmo.OnSelected(ray, selectedTransform);
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

            GLHelper.SetDefaultMaterialPass();

            if (selectedTransform.parent != null)
            {
                localPositionGizmo.OnRender(selectedTransform);
                localEulerAngleGizmo.OnRender(selectedTransform);
                transformOrientationGizmo.OnRender(selectedTransform);
                fromCameraViewRotationGizmo.OnRender(selectedTransform);
            }
            else
            {
                globalPositionGizmo.OnRender(selectedTransform);
                globalEulerAnglesGizmo.OnRender(selectedTransform);
                transformOrientationGizmo.OnRender(selectedTransform);
                fromCameraViewRotationGizmo.OnRender(selectedTransform);
            }
        }
    }
}
