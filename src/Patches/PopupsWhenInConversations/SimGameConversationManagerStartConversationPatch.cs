using System;

using Harmony;

using BattleTech;
using BattleTech.UI;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "StartConversation")]
  [HarmonyPatch(new Type[] { typeof(SimGameInterruptManager.ConversationEntry) })]
  public class SimGameConversationManagerStartConversationPatch {
    static void Prefix(SimGameConversationManager __instance, SimGameInterruptManager.ConversationEntry entry) {
      SimGameInterruptManager.Entry currentPopup = UnityGameInstance.Instance.Game.Simulation.InterruptQueue.curPopup;

      // Only force close if it's a DIFFERENT popup than the one we're about to start
      // This prevents closing the same entry we're starting, which would corrupt state
      if (currentPopup != null && currentPopup != entry) {
        Main.Logger.Log("[SimGameConversationManagerStartConversationPatch] Force clearing previous interrupt (different from one being started).");
        currentPopup.Close();
      } else if (currentPopup != null && currentPopup == entry) {
        Main.Logger.LogWarning("[SimGameConversationManagerStartConversationPatch] Current popup is the same as entry being started - this suggests state corruption. Not force closing.");
      }
    }

    static void Postfix(SimGameConversationManager __instance) {
      if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogConditions) {
        string conversationName = __instance?.thisConvoDef?.ui_name ?? "unknown";
        Main.Logger.Log($"");
        Main.Logger.Log($"========================================");
        Main.Logger.Log($"[StartConversation] Starting: {conversationName}");
        Main.Logger.Log($"========================================\n");
      }
    }
  }
}