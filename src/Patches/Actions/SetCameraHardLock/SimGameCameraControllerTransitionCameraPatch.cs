

using Harmony;

using BattleTech;

using System;

using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameCameraController), "TransitionCamera")]
  [HarmonyPatch(new Type[] { typeof(SimGameState.SimGameCharacterType), typeof(bool) })]
  public class SimGameCameraControllerTransitionCameraPatch {
    static bool Prefix(SimGameCameraController __instance, SimGameState.SimGameCharacterType character) {
      if (Actions.HardLockTarget != null && Actions.HardLockTarget != character.ToString()) {
        return false;
      }

      return true;
    }
  }
}