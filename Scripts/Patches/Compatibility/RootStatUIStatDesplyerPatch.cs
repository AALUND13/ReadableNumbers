using HarmonyLib;
using Infoholic.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadableNumbers.Patches.Compatibility {
    public class RootStatUIStatDesplyerPatch {
        public static void Patch(Harmony harmony) {
            var original = AccessTools.Method(typeof(RSUI.StatDesplyer), nameof(RSUI.StatDesplyer.Format));
            var prefix = AccessTools.Method(typeof(RootStatUIStatDesplyerPatch), nameof(FormatPrefix));

            harmony.Patch(original, prefix: new HarmonyMethod(prefix));
        }

        public static bool FormatPrefix(ref string __result, float value) {
            if(!NumberDisplayController.isDisable) {
                __result = NumberFormatters.NormalNumberFormatter.DisplayNumber(value, DisplayType.Suffix);
            }
            return NumberDisplayController.isDisable;
        }
    }
}
