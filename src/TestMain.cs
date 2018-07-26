using System;
using HBS.Logging;
using Harmony;
using Newtonsoft.Json;
using System.Reflection;

namespace TestBattletechMod {
    public class TestMain {
        private static ILog Log = HBS.Logging.Logger.GetLogger(typeof(TestMain).Name, LogLevel.Log);
        private static Settings settings;

        // Entry point into the mod, specified in the `mod.json`
        public static void Init(string modDirectory, string modSettings) {
            try {
                Log.Log("Loading TestBattletechMod settings");
                settings = JsonConvert.DeserializeObject<Settings>(modSettings);

                HarmonyInstance harmony = HarmonyInstance.Create("co.uk.cwolf.TestBattletechMod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            } catch (Exception) {
                Log.Log("Error loading mod settings - using defaults.");
                settings = new Settings();
            }
        }
    }
}