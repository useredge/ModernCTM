using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SpinCore.UI;
using SpinCore.Translation;
using SpinCore.Utility;
using UnityEngine;

namespace ModernCTM
{
    [BepInPlugin("com.useredge.modernctm", "ModernCTM", "1.0.0")]
    [BepInDependency("srxd.raoul1808.spincore", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger;

        private static bool _enabled = true;

        private void Awake()
        {
            Logger = base.Logger;
            Harmony.CreateAndPatchAll(typeof(WheelPatch));

            var keysStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ModernCTM.Resources.keys.json");
            TranslationHelper.LoadTranslationsFromStream(keysStream);

            UIHelper.RegisterGroupInQuickModSettings(qmPanel =>
            {
                var section = UIHelper.CreateGroup(qmPanel, "ModernCTM");
                UIHelper.CreateSectionHeader(section.Transform, "ModernCTMHeader", "ModernCTM_Header", false);
                UIHelper.CreateSmallToggle(section.Transform, "ModernCTMToggle", "ModernCTM_Toggle", true,
                    v => { _enabled = v; });
            });
        }

        class WheelPatch
        {
            [HarmonyPatch(typeof(PlayState), nameof(PlayState.UpdateGameplay)),
             HarmonyPostfix]
            private static void UpdateWheel_Postfix(PlayState __instance)
            {
                if (!_enabled) return;
                if (!__instance.wheelState.IsBeingHeld && !__instance.wheelState.isSpinning)
                {
                    __instance.wheelState.rotationSpeed = 0f;
                }
                
            }
        }
    }
}