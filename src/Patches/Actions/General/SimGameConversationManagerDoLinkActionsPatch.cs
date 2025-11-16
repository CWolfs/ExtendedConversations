using Harmony;

using BattleTech;

using isogame;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "DoLinkActions")]
  public class SimGameConversationManagerDoLinkActionsPatch {
    static void Prefix(SimGameConversationManager __instance, ConversationLink link) {
      Actions.IsLinkAction = true;
      Actions.IsNodeAction = false;

      if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogActions) {
        string linkId = link?.idRef?.id ?? "unknown";
        int actionCount = link?.actions?.ops?.Count ?? 0;
        Main.Logger.Log($"\n[DoLinkActions] Executing {actionCount} action(s) for LinkId: {linkId}");
      }
    }

    static void Postfix(ConversationLink link) {
      Actions.IsLinkAction = false;

      if (Main.Settings != null && Main.Settings.EnableDebugLogging && Main.Settings.DebugLogActions) {
        string linkId = link?.idRef?.id ?? "unknown";
        Main.Logger.Log($"[DoLinkActions] Completed actions for LinkId: {linkId}\n");
      }
    }
  }
}