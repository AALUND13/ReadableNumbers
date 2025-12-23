using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using TabInfo.Utils;

namespace ReadableNumbers.Patches.Compatibility {
    public class TabInfoManagerPatch {
        public static Regex Regex = new Regex(@"\{\d+:([^}]+)\}");

        public static void PatchAllDisplayValue(Harmony harmony) {
            foreach(StatCategory category in TabInfoManager.Categories.Values) {
                foreach(Stat stat in category.Stats.Values) {
                    Func<Player, string> displayValue = stat.displayValue;
                    MethodInfo method = displayValue.Method;

                    if(stat.name == "Damage") {
                        stat.displayValue = (Player player) => {
                            return NumberDisplayController.DisplayNumber(float.Parse(displayValue(player)));
                        };
                    } else if(stat.name == "HP") {
                        stat.displayValue = (Player player) => {
                            return $"{NumberDisplayController.DisplayNumber(player.data.health)}/{NumberDisplayController.DisplayNumber(player.data.maxHealth)}";
                        };
                    } else {
                        var transpiler = AccessTools.Method(typeof(TabInfoManagerPatch), nameof(PatchTranspiler));
                        harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));

                        ReadableNumbers.ModLogger.LogInfo($"Patched the \"{stat.name}\" display value method from the \"{category.name}\" category");
                    }
                }
            }
        }

        public static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            MethodInfo stringFormat1 = AccessTools.Method(typeof(string), nameof(string.Format), new Type[] { typeof(string), typeof(object) });
            MethodInfo stringFormat2 = AccessTools.Method(typeof(string), nameof(string.Format), new Type[] { typeof(string), typeof(object), typeof(object) });
            MethodInfo stringFormat3 = AccessTools.Method(typeof(string), nameof(string.Format), new Type[] { typeof(string), typeof(object), typeof(object), typeof(object) });

            MethodInfo DisplayNumberFloat = AccessTools.Method(typeof(NumberDisplayController), nameof(NumberDisplayController.DisplayNumber), new Type[] { typeof(float), typeof(string) });
            MethodInfo DisplayNumberInt = AccessTools.Method(typeof(NumberDisplayController), nameof(NumberDisplayController.DisplayNumber), new Type[] { typeof(int), typeof(string) });

            // Collect all string.Format calls (indices)
            var formatCalls = new List<int>();
            for(int i = codes.Count - 1; i >= 0; i--) {
                if(codes[i].Calls(stringFormat1) || codes[i].Calls(stringFormat2) || codes[i].Calls(stringFormat3)) {
                    formatCalls.Add(i);
                }
            }

            // Patch each string.Format call
            foreach(var index in formatCalls) {
                MethodInfo calledMethod = null;
                int argCount = 0;
                var code = codes[index];

                if(code.Calls(stringFormat1)) { calledMethod = stringFormat1; argCount = 1; } 
                else if(code.Calls(stringFormat2)) { calledMethod = stringFormat2; argCount = 2; }
                else if(code.Calls(stringFormat3)) { calledMethod = stringFormat3; argCount = 3; }

                ReadableNumbers.ModLogger.LogInfo($"Found string.Format call at index {index} with {argCount} argument(s)");

                string formatString = "";
                for(int j = index - 1; j >= 0; j--) {
                    if(codes[j].opcode == OpCodes.Ldstr) {
                        formatString = codes[j].operand.ToString();
                        ReadableNumbers.ModLogger.LogInfo($"Format: {codes[j].operand.ToString()}");
                        break;
                    }
                }
                string[] formatParms = Regex.Matches(formatString)
                        .Cast<Match>()
                        .Select(m => m.Groups[1].Value)
                        .ToArray();

                int parmIndex = 0;
                for(int j = index - 1; j >= index - (argCount * 5) && j >= 0; j--) {
                    if(codes[j].opcode == OpCodes.Box) {
                        var boxedType = codes[j].operand as Type;
                        string formatSpec = formatParms.Length > parmIndex
                            ? formatParms[parmIndex]
                            : "";

                        if(boxedType == typeof(float)) {
                            codes[j] = new CodeInstruction(OpCodes.Ldstr, formatSpec);
                            codes.Insert(j + 1, new CodeInstruction(OpCodes.Call, DisplayNumberFloat));
                        } else if(boxedType == typeof(int)) {
                            codes[j] = new CodeInstruction(OpCodes.Ldstr, formatSpec);
                            codes.Insert(j + 1, new CodeInstruction(OpCodes.Call, DisplayNumberInt));
                        }

                        parmIndex++;
                    }
                }
            }

            return codes.AsEnumerable();
        }
    }
}
