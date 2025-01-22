using HarmonyLib;
using UnityEngine;

namespace UnityExplorer
{
    [HarmonyPatch]
    public class Patches
    {
        // allow explorer to run in post credits scene
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PostCreditsManager), nameof(PostCreditsManager.Awake))]
        public static void PostCreditsInput(PostCreditsManager __instance)
        {
            GameObject.Find("GlobalManagers").AddComponent<OWInput>();
            GameObject.Find("GlobalManagers").GetComponent<OWInput>().Awake();
        }
    }
}
