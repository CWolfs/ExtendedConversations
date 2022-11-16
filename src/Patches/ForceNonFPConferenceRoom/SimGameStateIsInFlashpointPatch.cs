using Harmony;

using BattleTech;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameState), "IsInFlashpoint", MethodType.Getter)]
  public class SimGameStateIsInFlashpointPatch {
    static void Postfix(SimGameState __instance, ref bool __result) {
      if (Actions.ForceNextIsInFlashpointCheckFalse) {
        Main.Logger.Log("[SimGameStateIsInFlashpointPatch] Forcing to use non-FP conference room");
        __result = false;
      }
    }
  }
}