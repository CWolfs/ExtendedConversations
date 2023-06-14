using Harmony;

using BattleTech;

using ExtendedConversations.Animation;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameState), "SimGameUXCreatorLoaded")]
  public class SimGameStateSimGameUXCreatorLoadedPatch {
    public static void Postfix(SimGameState __instance) {
      AnimationManager.Instance.ReplaceAllVanillaCrewWithMimics();
    }
  }
}