using Harmony;
using BattleTech;
using isogame;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "ShowNodeText")]
  public class SimGameConversationManagerShowNodeTextPatch {
    static void Prefix(ConversationNode node) {
      if (Main.Settings == null || !Main.Settings.EnableDebugLogging || !Main.Settings.DebugLogNodeText) return;

      int nodeIndex = node?.index ?? -1;
      string nodeText = node?.text ?? "";

      // Truncate if too long (keep first 150 chars)
      if (nodeText.Length > 150) {
        nodeText = nodeText.Substring(0, 150) + "...";
      }

      Main.Logger.Log($"\n[Node] Index: {nodeIndex}");
      Main.Logger.Log($"[Node] Text: \"{nodeText}\"");
    }
  }
}
