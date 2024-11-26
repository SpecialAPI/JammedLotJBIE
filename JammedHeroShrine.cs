using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Collections;
using System.Reflection;

namespace JammedLotJBIE
{
    [HarmonyPatch]
    public class JammedHeroShrine : BraveBehaviour
    {
        public static MethodInfo jhsb_sc_cjc = AccessTools.Method(typeof(JammedHeroShrine), nameof(JammedHeroShrineBehavior_ShrineConversation_CheckJammedCurse));
        public static MethodInfo jhsb_sc_jsoa = AccessTools.Method(typeof(JammedHeroShrine), nameof(JammedHeroShrineBehavior_ShrineConversation_JamShrineOnAccept));
        public static MethodInfo jhsb_sc_jsod = AccessTools.Method(typeof(JammedHeroShrine), nameof(JammedHeroShrineBehavior_ShrineConversation_JamShrineOnDecline));
        public static MethodInfo jhsb_se_stc = AccessTools.Method(typeof(JammedHeroShrine), nameof(JammedHeroShrineBehavior_ShrineEffect_SetTargetCurse));

        public void Awake()
        {
            var trans = new string[]
            {
                "ShrineStatue",
                "shrine_Base",
                "shrine_Base (1)"
            };

            foreach(var t in trans)
            {
                var tr = transform.Find(t);

                if (tr == null)
                    continue;

                var sprite = tr.GetComponent<tk2dBaseSprite>();

                if (sprite == null)
                    continue;

                sprite.usesOverrideMaterial = true;

                var material = sprite.renderer.material;
                material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                material.SetFloat("_PhantomGradientScale", 0.75f);
                material.SetFloat("_PhantomContrastPower", 1.3f);
            }
        }

        [HarmonyPatch(typeof(AdvancedShrineController), nameof(AdvancedShrineController.HandleShrineConversation), MethodType.Enumerator)]
        [HarmonyILManipulator]
        public static void JammedHeroShrineBehavior_ShrineConversation_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchLdcI4(9)))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, jhsb_sc_cjc);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<AdvancedShrineController>(nameof(AdvancedShrineController.DoShrineEffect))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, jhsb_sc_jsoa);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<AdvancedShrineController>(nameof(AdvancedShrineController.ResetForReuse)), 2))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, jhsb_sc_jsod);
        }

        public static int JammedHeroShrineBehavior_ShrineConversation_CheckJammedCurse(int curseReq, IEnumerator num)
        {
            if (num == null)
                return curseReq;

            var shrine = num.EnumeratorGetField<AdvancedShrineController>("$this");

            if (shrine == null || shrine.GetComponent<JammedHeroShrine>() == null)
                return curseReq;

            return 19;
        }

        public static void JammedHeroShrineBehavior_ShrineConversation_JamShrineOnAccept(IEnumerator num)
        {
            if (num == null)
                return;

            var shrine = num.EnumeratorGetField<AdvancedShrineController>("$this");

            if (shrine == null || !shrine.IsLegendaryHeroShrine)
                return;

            shrine.gameObject.GetOrAddComponent<JammedHeroShrine>();
        }

        public static void JammedHeroShrineBehavior_ShrineConversation_JamShrineOnDecline(IEnumerator num)
        {
            if(num == null)
                return;

            var shrine = num.EnumeratorGetField<AdvancedShrineController>("$this");
            var canBeUsed = num.EnumeratorGetField<bool>("canUse");

            if (shrine == null || !shrine.IsLegendaryHeroShrine || canBeUsed)
                return;

            shrine.gameObject.GetOrAddComponent<JammedHeroShrine>();
        }

        [HarmonyPatch(typeof(AdvancedShrineController), nameof(AdvancedShrineController.DoShrineEffect))]
        [HarmonyILManipulator]
        public static void JammedHeroShrineBehavior_ShrineEffect_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchStloc(8)))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Ldloca, 8);
            crs.Emit(OpCodes.Call, jhsb_se_stc);

            if (!crs.JumpToNext(x => x.MatchStloc(8)))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Ldloca, 8);
            crs.Emit(OpCodes.Call, jhsb_se_stc);
        }

        public static void JammedHeroShrineBehavior_ShrineEffect_SetTargetCurse(AdvancedShrineController shrine, ref int curseTarget)
        {
            if (shrine == null || shrine.GetComponent<JammedHeroShrine>() == null)
                return;

            curseTarget = 19;
        }
    }
}
