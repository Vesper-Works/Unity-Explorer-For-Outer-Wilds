using System.Collections.Generic;
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


        private List<BaseTransformGizmo> EnabledGizmos = new();

        int previousSelectedGizmo = 0;
        int previousSubTypeGizmoOption = 0;

        private void ChangeGizmos(int selectedGizmo, int subTypeGizmoOption) 
        {
            selectedGizmo %= 3;

            if (previousSelectedGizmo != selectedGizmo)
            {
                EnabledGizmos.Clear();
            }
            switch (selectedGizmo) 
            {
                case 1:
                    ChangePositionGizmos(subTypeGizmoOption);
                    break;
                case 2:
                    ChangeRotationGizmos(subTypeGizmoOption);
                    break;

                case 0:
                default:
                    break;
            }

            previousSelectedGizmo = selectedGizmo;
        }

        private readonly List<BaseTransformGizmo> PositionGizmos = new()
        {
            new LocalPositionGizmo(),
            new TransformOrientationGizmo(),
        };
        private void ChangePositionGizmos(int positionGizmoTypes)
        {
            positionGizmoTypes %= PositionGizmos.Count;

            if (previousSubTypeGizmoOption != positionGizmoTypes)
            {
                EnabledGizmos.Clear();
            }
            if (PositionGizmos.Count > 0)
            {
                EnabledGizmos.Add(PositionGizmos[positionGizmoTypes]);
            }

            previousSubTypeGizmoOption = positionGizmoTypes;
        }

        private readonly List<BaseTransformGizmo> RotationGizmos = new()
        {
            new LocalEulerAngleGizmo(),
            new FromCameraViewRotationGizmo(),
        };
        private void ChangeRotationGizmos(int rotationGizmoTypes)
        {
            rotationGizmoTypes %= 2;

            if (previousSubTypeGizmoOption != rotationGizmoTypes)
            {
                EnabledGizmos.Clear();
            }
            if (RotationGizmos.Count > 0)
            {
                EnabledGizmos.Add(PositionGizmos[rotationGizmoTypes]);
            }
            previousSubTypeGizmoOption = rotationGizmoTypes;
        }


        //TODO add hotkey to enable gizmos
        //And maybe settings for choosing them on the configs
        private void HotKeysForSwaping() 
        {
            if (UniverseLib.Input.InputManager.GetKeyUp(KeyCode.R)) 
            {
                ChangeGizmos(previousSelectedGizmo + 1, previousSubTypeGizmoOption);
            }
            
            if (UniverseLib.Input.InputManager.GetKeyUp(KeyCode.T)) 
            {
                ChangeGizmos(previousSelectedGizmo, previousSubTypeGizmoOption + 1);
            }
            
        }

        private void Update()
        {
            HotKeysForSwaping();

            var inspector = InspectorManager.ActiveInspector;

            if (inspector is not GameObjectInspector goInspector)
            {
                EnabledGizmos.ForEach((gizmo) => gizmo.Reset());
                return;
            }
            if(goInspector.Target == null)
            {
                EnabledGizmos.ForEach((gizmo) => gizmo.Reset());
                return;
            }

            selectedTransform = goInspector.Target.transform;

            float scale = Vector3.Distance(Locator.GetActiveCamera().transform.position, selectedTransform.position) / 8f;

            EnabledGizmos.ForEach((gizmo) =>
            {
                gizmo.SetScale(scale);
                gizmo.Set(selectedTransform);
            });

            Ray ray = Locator.GetActiveCamera().ScreenPointToRay(UniverseLib.Input.InputManager.MousePosition);

            if (UniverseLib.Input.InputManager.GetMouseButtonDown(0))
            {
                bool noneIsSelected = EnabledGizmos.TrueForAll((gizmo) => !gizmo.IsSelected());

                if (noneIsSelected)
                {
                    EnabledGizmos.ForEach((gizmo) => gizmo.CheckSelected(ray, maxDistanceToSelect * scale));
                }
            }
            else if (UniverseLib.Input.InputManager.GetMouseButton(0))
            {
                EnabledGizmos.ForEach((gizmo) =>
                {
                    if (gizmo.IsSelected())
                    {
                        gizmo.OnSelected(ray);
                    }
                });
            }
            else
            {
                EnabledGizmos.ForEach((gizmo) => gizmo.Reset());
            }

        }
        public void OnRenderObject()
        {
            if (selectedTransform == null)
                return;

            GL.wireframe = true;

            GLHelper.SetDefaultMaterialPass(0, true);

            EnabledGizmos.ForEach((gizmo) => gizmo.OnRender());

            GL.wireframe = false;
        }
    }
}
