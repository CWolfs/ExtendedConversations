using System;

using Harmony;

using BattleTech;
using BattleTech.UI;
using isogame;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "StartConversation")]
  [HarmonyPatch(new Type[] { typeof(Conversation), typeof(string), typeof(string), typeof(CastDef), typeof(bool), typeof(DropshipMenuType), typeof(string) })]
  public class SimGameConversationManagerStartConversation2Patch {
    static void Postfix(SimGameConversationManager __instance, Conversation convoDef) {
      // Debug logging for conversation start (in Postfix so thisConvoDef is set)
      if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogConditions) {
        string conversationName = convoDef?.ui_name ?? __instance?.thisConvoDef?.ui_name ?? "unknown";
        Main.Logger.Log($"\n========================================");
        Main.Logger.Log($"[StartConversation] Starting: {conversationName}");
        Main.Logger.Log($"========================================\n");
      }
    }
  }
}
