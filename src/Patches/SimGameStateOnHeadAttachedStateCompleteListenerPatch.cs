using Harmony;

using BattleTech;

using ExtendedConversations.Animation;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameState), "OnHeadAttachedStateCompleteListener")]
  public class SimGameStateOnHeadAttachedStateCompleteListenerPatch {
    public static void Postfix(SimGameState __instance) {
      AnimationManager.Instance.ReplaceAllVanillaCrewWithMimics();
    }
  }
}