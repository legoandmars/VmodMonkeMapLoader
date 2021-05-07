using HarmonyLib;
using VmodMonkeMapLoader.Behaviours;
using System;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(PhotonNetworkController))]
    [HarmonyPatch("GetRegionWithLowestPing", MethodType.Normal)]
    internal class ForceRegionPatch
    {
        public static string forcedRegion;
        private static void Postfix(PhotonNetworkController __instance, ref string __result)
        {
            if(!String.IsNullOrWhiteSpace(forcedRegion) && __result != forcedRegion)
            {
                __result = forcedRegion;
                forcedRegion = null;
            }
        }
    }
}