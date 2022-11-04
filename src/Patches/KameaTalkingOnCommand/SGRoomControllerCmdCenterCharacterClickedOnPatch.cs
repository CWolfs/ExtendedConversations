using Harmony;

using BattleTech;
using BattleTech.UI;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SGRoomController_CmdCenter), "CharacterClickedOn")]
  public class SGRoomControllerCmdCenterCharacterClickedOnPatch {
    static void Postfix(SGRoomController_CmdCenter __instance, SimGameState.SimGameCharacterType characterClicked) {
      if (characterClicked == SimGameState.SimGameCharacterType.KAMEA) {
        UnityGameInstance.Instance.Game.Simulation.ConversationManager.BeginCharacterConversation(SimGameState.SimGameCharacterType.KAMEA);
      }
    }
  }
}