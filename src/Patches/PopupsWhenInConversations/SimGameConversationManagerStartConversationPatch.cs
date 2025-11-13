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

      if (currentPopup != null) {
        Main.Logger.Log("[SimGameConversationManagerStartConversationPatch] Force clearing any previous current interrupt.");
        UnityGameInstance.Instance.Game.Simulation.InterruptQueue.curPopup.Close();
      }
    }

    static void Postfix(SimGameConversationManager __instance) {
      // Debug logging for conversation start (in Postfix so thisConvoDef is set)
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