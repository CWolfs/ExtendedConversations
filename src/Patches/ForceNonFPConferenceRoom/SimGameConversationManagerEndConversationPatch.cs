using Harmony;

using BattleTech;
using isogame;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "EndConversation")]
  public class SimGameConversationManagerEndConversationPatch {
    static void Prefix(SimGameConversationManager __instance) {
      Conversation conversation = (Conversation)AccessTools.Field(typeof(SimGameConversationManager), "thisConvoDef").GetValue(__instance);

      if (Actions.ActiveConversation == conversation) {
        Actions.ForceNextIsInFlashpointCheckFalse = false;
        Actions.ActiveConversation = null;
      }
    }
  }
}