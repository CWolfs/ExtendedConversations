using Harmony;

using BattleTech;

using isogame;

using System;

using ExtendedConversations.Utils;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "ResolveLink")]
  public class SimGameConversationManagerResolveLinkPatch {
    static void Prefix(SimGameConversationManager __instance, ref ConversationLink link) {
      try {
        // Do a scan of all link actions - if it contains a 'Sideload Conversation' on the link - then run the below code (otherwise ignore it)
        if (ConversationHelper.DoesLinkContainAction("Sideload Conversation", link)) {
          // Pre-run DoLinkActions(link) to get ahead of the link issue
          __instance.DoLinkActions(link);
          link = __instance.currentLink;
        }
      } catch (Exception e) {
        Main.Logger.LogError("[Extended Conversations] An error occured in SimGameConversationManagerResolveLinkPatch. Caught gracefully." + e.StackTrace.ToString());
      }
    }
  }
}