using Harmony;

using BattleTech;
using BattleTech.UI;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SGRoomController_CmdCenter), "IsDiegeticInThisRoom")]
  public class SGRoomControllerCmdCenterIsDiegeticInThisRoomPatch {
    static void Postfix(SGRoomController_CmdCenter __instance, SimGameState.SimGameCharacterType diegetic, ref bool __result) {
      if (diegetic == SimGameState.SimGameCharacterType.KAMEA) __result = true;
    }
  }
}