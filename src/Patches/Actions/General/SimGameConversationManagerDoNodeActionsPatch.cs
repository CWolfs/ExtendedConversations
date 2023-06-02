using Harmony;

using BattleTech;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "DoNodeActions")]
  public class SimGameConversationManagerDoNodeActionsPatch {
    static void Prefix(SimGameConversationManager __instance) {
      Actions.IsLinkAction = false;
      Actions.IsNodeAction = true;
    }
  }
}