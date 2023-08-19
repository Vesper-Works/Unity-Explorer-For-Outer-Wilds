using OWML.Common;
using OWML.ModHelper;
using System;
using UnityEngine;

namespace UnityExplorer
{
    public class UnityExplorer : ModBehaviour
    {
        public static UnityExplorer Instance { get; private set; }
        private ExtendedTransformTools extendedTransformTools;
        private void Start()
        {
            Instance = this;
            ExplorerStandalone.CreateInstance(
                (message, type) =>
                    ModHelper.Console.WriteLine(message, 
                        type == LogType.Log ? MessageType.Message :
                        type == LogType.Warning ? MessageType.Warning :
                        type == LogType.Error ? MessageType.Error :
                        type == LogType.Assert ? MessageType.Error :
                        type == LogType.Exception ? MessageType.Error :
                        throw new ArgumentOutOfRangeException(nameof(type), type, null))
            );

            extendedTransformTools = gameObject.AddComponent<ExtendedTransformTools>();
        }
        
        /*public override void Configure(IModConfig config)
        {
            extendedTransformTools.GizmoSelectKey = 
                (KeyCode)Enum.Parse(typeof(KeyCode), config.GetSettingsValue<string>("Gizmo Select Key"));
            extendedTransformTools.SubGizmoSelectKey = 
                (KeyCode)Enum.Parse(typeof(KeyCode), config.GetSettingsValue<string>("Sub Gizmo Select Key"));

            ModHelper.Console.WriteLine($"{extendedTransformTools.GizmoSelectKey.ToString()}");
            ModHelper.Console.WriteLine($"{extendedTransformTools.SubGizmoSelectKey.ToString()}");
        }*/
    }
}