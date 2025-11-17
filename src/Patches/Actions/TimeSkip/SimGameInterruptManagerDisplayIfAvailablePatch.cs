using System;

using Harmony;

using BattleTech.UI;

using ExtendedConversations.State;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameInterruptManager), "DisplayIfAvailable")]
  public class SimGameInterruptManagerDisplayIfAvailablePatch {
    static bool Prefix(SimGameInterruptManager __instance) {
      try {
        TimeSkipStateManager stateManager = TimeSkipStateManager.Instance;

        if (stateManager.IsTimeSkipActive && stateManager.DisablePopups) {
          string popupType = "Unknown";
          if (__instance.popups.Count > 0) {
            popupType = __instance.popups[0].type.ToString();
          }
          Main.Logger.Log($"[SimGameInterruptManagerDisplayIfAvailablePatch] Suppressing popup during time skip: {popupType}");
          return false;
        }

        return true;
      } catch (Exception e) {
        Main.Logger.LogError("[SimGameInterruptManagerDisplayIfAvailablePatch] An error occurred in SimGameInterruptManagerDisplayIfAvailablePatch. Caught gracefully. " + e.StackTrace.ToString());
        return true;
      }
    }
  }
}
