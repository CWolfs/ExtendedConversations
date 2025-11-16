using Harmony;

using BattleTech;

using isogame;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "DoNodeActions")]
  public class SimGameConversationManagerDoNodeActionsPatch {
    static void Prefix(SimGameConversationManager __instance, ConversationNode node) {
      Actions.IsLinkAction = false;
      Actions.IsNodeAction = true;

      if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogActions) {
        int nodeIndex = node?.index ?? -1;
        int actionCount = node?.actions?.ops?.Count ?? 0;
        Main.Logger.Log($"\n[DoNodeActions] Executing {actionCount} action(s) for Node: {nodeIndex}");
      }
    }

    static void Postfix(ConversationNode node) {
      Actions.IsNodeAction = false;

      if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogActions) {
        int nodeIndex = node?.index ?? -1;
        Main.Logger.Log($"[DoNodeActions] Completed actions for Node: {nodeIndex}\n");
      }
    }
  }
}