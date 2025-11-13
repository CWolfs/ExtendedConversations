using Harmony;
using BattleTech;
using isogame;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "EvaluateLink")]
  public class SimGameConversationManagerEvaluateLinkPatch {
    static void Prefix(ConversationLink link) {
      if (Main.Settings == null || !Main.Settings.EnableDebugLogging || !Main.Settings.DebugLogConditions) return;

      string linkId = link.idRef?.id ?? "unknown";
      string responseText = string.IsNullOrEmpty(link.responseText) ? "[auto-follow]" : link.responseText;
      int conditionCount = link.conditions?.ops?.Count ?? 0;

      Main.Logger.Log($"\n[EvaluateLink] START - LinkId: {linkId}, Response: '{responseText}', OnlyOnce: {link.onlyOnce}, Conditions: {conditionCount}");
    }

    static void Postfix(ConversationLink link, bool __result) {
      if (Main.Settings == null || !Main.Settings.EnableDebugLogging || !Main.Settings.DebugLogConditions) return;

      string linkId = link.idRef?.id ?? "unknown";
      Main.Logger.Log($"[EvaluateLink] RESULT - LinkId: {linkId}, Passed: {__result}\n");
    }
  }
}
