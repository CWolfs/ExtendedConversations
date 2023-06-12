using Harmony;

using BattleTech;

using ExtendedConversations.Core;
using ExtendedConversations.State;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "SelectResponse")]
  public class SimGameConversationManagerSelectResponsePatch {
    static void Prefix(SimGameConversationManager __instance, int num) {
      if (Actions.SideLoadCaptureNextResponseIndex) {
        Actions.SideLoadCaptureNextResponseIndex = false;
        string currentConversationId = __instance.thisConvoDef.idRef.id;

        if (Actions.SideloadConversationMap.ContainsKey(currentConversationId)) {
          string previousConversationId = Actions.SideloadConversationMap[currentConversationId];
          SideloadConversationState cachedState = Actions.SideLoadCachedState[previousConversationId];
          cachedState.ResponseIndexClicked = num;
        }
      }
    }
  }
}