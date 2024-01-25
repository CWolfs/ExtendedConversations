using System;

using Harmony;

using BattleTech;
using BattleTech.UI;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameConversationManager), "StartConversation")]
  [HarmonyPatch(new Type[] { typeof(SimGameInterruptManager.ConversationEntry) })]
  public class SimGameConversationManagerStartConversationPatch {
    static void Prefix(SimGameConversationManager __instance) {
      SimGameInterruptManager.Entry currentPopup = UnityGameInstance.Instance.Game.Simulation.InterruptQueue.curPopup;

      if (currentPopup != null) {
        Main.Logger.Log("[SimGameConversationManagerStartConversationPatch] Force clearing any previous current interrupt.");
        UnityGameInstance.Instance.Game.Simulation.InterruptQueue.curPopup.Close();
      }
    }
  }
}