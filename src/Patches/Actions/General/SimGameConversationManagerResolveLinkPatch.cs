using Harmony;

using BattleTech;

using isogame;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "ResolveLink")]
  public class SimGameConversationManagerResolveLinkPatch {
    static void Prefix(SimGameConversationManager __instance, ref ConversationLink link) {
      // Pre-run DoLinkActions(link) to get ahead of the link issue
      __instance.DoLinkActions(link);

      // If the link actions had a 'sideload conversation' action then replace the link going into ResolveLink
      if (Actions.ReplaceLinkOnResponseIfNeeded) {
        link = __instance.currentLink;
        Actions.ReplaceLinkOnResponseIfNeeded = false;
      }
    }

    static void Postfix(SimGameConversationManager __instance, ref ConversationLink link) {
      Actions.ReplaceLinkOnResponseIfNeeded = false;
    }
  }
}