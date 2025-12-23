using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ReadableNumbers.Patches.Compatibility;
using System.Collections.Generic;
using System.Reflection;
using UnboundLib;
using UnityEngine;

namespace ReadableNumbers {
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]

    [BepInDependency("com.willuwontu.rounds.tabinfo", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.penial.rounds.Infoholic", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInPlugin(modId, ModName, "1.0.0")]
    [BepInProcess("Rounds.exe")]
    public class ReadableNumbers : BaseUnityPlugin {
        internal const string modId = "com.aalund13.rounds.readable_numbers";
        internal const string ModName = "Readable Numbers";
        internal const string modInitials = "RN";
        
        internal static ReadableNumbers instance;
        internal static AssetBundle assets;

        internal static ManualLogSource ModLogger { get; private set; }
        internal static Harmony Harmony { get; private set; }
        
        void Awake() {
            instance = this;
            ModLogger = Logger;
            Harmony = new Harmony(modId);
            Harmony.PatchAll();

            gameObject.AddComponent<NumberDisplayController>();

            List<BaseUnityPlugin> Plugins = (List<BaseUnityPlugin>)typeof(BepInEx.Bootstrap.Chainloader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            if(Plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.penial.rounds.Infoholic")) {
                InfoholicGameStatusUpdatePatch.Patch(Harmony);
            }

            Unbound.RegisterClientSideMod(modId);
            Debug.Log($"{ModName} loaded!");
        }
        void Start() {
            Debug.Log($"{ModName} started!");

            List<BaseUnityPlugin> Plugins = (List<BaseUnityPlugin>)typeof(BepInEx.Bootstrap.Chainloader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            if(Plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo")) {
                this.ExecuteAfterFrames(5, () => {
                    TabInfoManagerPatch.PatchAllDisplayValue(Harmony);
                });
            }
        }
    }
}