using UnityEngine;
using System;
using Harmony;
using TScript;
using HBS.Logging;

using TestBattletechMod.Core;

namespace TestBattletechMod {
  [HarmonyPatch(typeof(TsEnvironment))]
  public class TsEnvironmentPatch {
    private static ILog Log = HBS.Logging.Logger.GetLogger(typeof(TsEnvironmentPatch).Name, LogLevel.Log);

    static void Postfix(TsEnvironment __instance) {
      Log.Log("Testing Postfix for TsEnvironment");
      ConversationUpgrades.Declare(__instance);
    }
  }
}