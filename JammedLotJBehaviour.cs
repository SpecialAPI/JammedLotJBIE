using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JammedLotJBIE
{
    [HarmonyPatch]
    public class JammedLotJBehaviour : BraveBehaviour
    {
        public void Awake()
        {
            sprite.usesOverrideMaterial = true;

            var material = sprite.renderer.material;
            material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
            material.SetFloat("_PhantomGradientScale", 0.75f);
            material.SetFloat("_PhantomContrastPower", 1.3f);

            var lotjc = GetComponent<SuperReaperController>();
            lotjc.MinSpeed *= 1.5f;
            lotjc.MaxSpeed *= 1.5f;
            lotjc.c_particlesPerSecond *= 2;
        }

        [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.RecalculateStatsInternal))]
        [HarmonyPostfix]
        public static void JamLotJ_RecalculateStats_Postfix()
        {
            var reap = SuperReaperController.Instance;

            if (reap == null || reap.gameObject == null)
                return;

            var curse = PlayerStats.GetTotalCurse();

            if (curse < 20f)
                return;

            reap.gameObject.GetOrAddComponent<JammedLotJBehaviour>();
        }

        [HarmonyPatch(typeof(SuperReaperController), nameof(SuperReaperController.Start))]
        [HarmonyPostfix]
        public static void JamLotJ_Start_Postfix(SuperReaperController __instance)
        {
            var curse = PlayerStats.GetTotalCurse();

            if (curse < 20f)
                return;

            __instance.gameObject.GetOrAddComponent<JammedLotJBehaviour>();
        }

        [HarmonyPatch(typeof(AIBulletBank), nameof(AIBulletBank.CreateProjectileFromBank))]
        [HarmonyPostfix]
        public static void JamLotJBullets_Postfix(AIBulletBank __instance, GameObject __result)
        {
            if (__instance == null || __instance.GetComponent<SuperReaperController>() == null || __instance.GetComponent<JammedLotJBehaviour>() == null)
                return;

            if (__result == null || __result.GetComponent<Projectile>() is not Projectile proj)
                return;

            proj.ForceBlackBullet = true;
            proj.BecomeBlackBullet();
        }
    }
}
