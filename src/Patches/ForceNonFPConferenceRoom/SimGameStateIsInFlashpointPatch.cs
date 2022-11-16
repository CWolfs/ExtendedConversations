using Harmony;

using BattleTech;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameState), "IsInFlashpoint", MethodType.Getter)]
  public class SimGameStateIsInFlashpointPatch {
    static void Postfix(SimGameState __instance, ref bool __result) {
      if (Actions.ForceNextIsInFlashpointCheckFalse) {
        __result = false;
      }
    }
  }
}