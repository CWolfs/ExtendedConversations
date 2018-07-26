using UnityEngine;
using System;
using Harmony;
using TScript;
using HBS.Logging;

using ExtendedConversations;
using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(TsEnvironment))]
  public class TsEnvironmentPatch {
    static void Postfix(TsEnvironment __instance) {
      ConversationUpgrades.Declare(__instance);
    }
  }
}