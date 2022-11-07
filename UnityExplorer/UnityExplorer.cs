using OWML.Common;
using OWML.ModHelper;
using System;
using UnityEngine;

namespace UnityExplorer
{
    public class UnityExplorer : ModBehaviour
    {
        public static UnityExplorer Instance { get; private set; }
        private void Start()
        {
            Instance = this;
            ExplorerStandalone.CreateInstance(
                (message, type) =>
                    ModHelper.Console.WriteLine(message, type switch
                    {
                        LogType.Log => MessageType.Message,
                        LogType.Warning => MessageType.Warning,
                        LogType.Error => MessageType.Error,
                        LogType.Assert => MessageType.Error,
                        LogType.Exception => MessageType.Error,
                        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                    })
            );

            gameObject.AddComponent<ExtendedTransformTools>();
        }
    }
}