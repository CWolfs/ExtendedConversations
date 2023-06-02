using Harmony;

using BattleTech;

using isogame;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "DoLinkActions")]
  public class SimGameConversationManagerDoLinkActionsPatch {
    static void Prefix(SimGameConversationManager __instance) {
      Actions.IsLinkAction = true;
      Actions.IsNodeAction = false;
    }
  }
}