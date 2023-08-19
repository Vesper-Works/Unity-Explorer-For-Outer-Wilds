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
        
        //Hotkeys
        public KeyCode GizmoSelectKey = KeyCode.G;
        public KeyCode SubGizmoSelectKey = KeyCode.B;

        private List<BaseTransformGizmo> EnabledGizmos = new List<BaseTransformGizmo>();

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
            }

            previousSelectedGizmo = selectedGizmo;
        }

        private readonly List<BaseTransformGizmo> PositionGizmos = new List<BaseTransformGizmo>()
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

        private readonly List<List<BaseTransformGizmo>> RotationGizmos = new List<List<BaseTransformGizmo>>()
        {
            new List<BaseTransformGizmo>(){ new LocalEulerAngleGizmo() },
            new List<BaseTransformGizmo>(){ new LocalEulerAngleGizmo(), new FromCameraViewRotationGizmo() },
        };
        private void ChangeRotationGizmos(int rotationGizmoTypes)
        {
            rotationGizmoTypes %= RotationGizmos.Count;

            if (previousSubTypeGizmoOption != rotationGizmoTypes)
            {
                EnabledGizmos.Clear();
            }
            if (RotationGizmos.Count > 0)
            {
                EnabledGizmos.AddRange(RotationGizmos[rotationGizmoTypes]);
            }
            previousSubTypeGizmoOption = rotationGizmoTypes;
        }
        
        private void HotKeysForSwaping() 
        {
            if (Event.current == null)
                return;
            
            int selectedGizmo = previousSelectedGizmo;
            int selectedSubGizmo = previousSubTypeGizmoOption;

            selectedGizmo += UniverseLib.Input.InputManager.GetKeyUp(GizmoSelectKey) ? 1 : 0;
            selectedSubGizmo += UniverseLib.Input.InputManager.GetKeyUp(SubGizmoSelectKey) ? 1 : 0;

            ChangeGizmos(selectedGizmo, selectedSubGizmo);

        }
        private void ResetEnabledGizmos() 
        {
            for (int i = 0; i < EnabledGizmos.Count; i++)
            {
                EnabledGizmos[i].Reset();
            }
        }

        private void Update()
        {
            HotKeysForSwaping();

            var inspector = InspectorManager.ActiveInspector;

            if (!(inspector is GameObjectInspector goInspector))
            {
                ResetEnabledGizmos();
                return;
            }
            if(goInspector.Target == null)
            {
                ResetEnabledGizmos();
                return;
            }

            selectedTransform = goInspector.Target.transform;

            float scale = Vector3.Distance(Locator.GetActiveCamera().transform.position, selectedTransform.position) / 5f;


            for (int i = 0; i < EnabledGizmos.Count; i++)
            {
                BaseTransformGizmo gizmo = EnabledGizmos[i];
                gizmo.SetScale(scale);
                gizmo.Set(selectedTransform);
            }

            Ray ray = Locator.GetActiveCamera().ScreenPointToRay(UniverseLib.Input.InputManager.MousePosition);

            if (UniverseLib.Input.InputManager.GetMouseButtonDown(0))
            {
                bool oneIsSelected = false;
                for (int i = 0; i < EnabledGizmos.Count; i++)
                {
                    oneIsSelected |= EnabledGizmos[i].IsSelected();
                }
                if (!oneIsSelected)
                {
                    for (int i = 0; i < EnabledGizmos.Count; i++)
                    { 
                        EnabledGizmos[i].CheckSelected(ray, maxDistanceToSelect * scale);
                    }
                }
                
            }
            else if (UniverseLib.Input.InputManager.GetMouseButton(0))
            {
                for (int i = 0; i < EnabledGizmos.Count; i++)
                {
                    BaseTransformGizmo gizmo = EnabledGizmos[i];
                    if (gizmo.IsSelected())
                    {
                        gizmo.OnSelected(ray);
                    }
                }
            }
            else
            {
                ResetEnabledGizmos();
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
