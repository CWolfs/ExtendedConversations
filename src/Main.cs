using UnityEngine;

using System;
using System.Collections.Generic;

using HBS.Logging;

using Harmony;

using Newtonsoft.Json;

using System.Reflection;

using ExtendedConversations.Core;
using ExtendedConversations.Utils;

namespace ExtendedConversations {
  public class Main {
    public static ILog Logger;
    private static Settings settings;
    public static string Path { get; private set; }

    public static AssetBundle ECAssetsBundle { get; set; }

    public static void InitLogger(string modDirectory) {
      Dictionary<string, LogLevel> logLevels = new Dictionary<string, LogLevel> {
        ["ExtendedConversations"] = LogLevel.Debug
      };
      LogManager.Setup(modDirectory + "/output.log", logLevels);
      Logger = LogManager.GetLogger("ExtendedConversations");
      Path = modDirectory;
    }

    public static void LoadAssetBundles() {
      ECAssetsBundle = AssetBundle.LoadFromFile($"{Main.Path}/bundles/ec-assets-bundle");
      ECAssetsBundle.LoadAllAssets();

      AssetBundleLoader.AssetBundles.Add("ec-assets-bundle", ECAssetsBundle);
    }

    // Entry point into the mod, specified in the `mod.json`
    public static void Init(string modDirectory, string modSettings) {
      try {
        InitLogger(modDirectory);

        Logger.Log("Loading ExtendedConversations settings");
        settings = JsonConvert.DeserializeObject<Settings>(modSettings);
      } catch (Exception e) {
        Logger.LogError(e);
        Logger.Log("Error loading mod settings - using defaults.");
        settings = new Settings();
      }

      HarmonyInstance harmony = HarmonyInstance.Create("co.uk.cwolf.ExtendedConversations");
      harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
  }
}