using System;

using Harmony;

using BattleTech.UI;

using ExtendedConversations.State;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameInterruptManager), "DisplayIfAvailable")]
  public class SimGameInterruptManagerDisplayIfAvailablePatch {
    static bool Prefix() {
      try {
        TimeSkipStateManager stateManager = TimeSkipStateManager.Instance;

        if (stateManager.IsTimeSkipActive && stateManager.DisablePopups) {
          Main.Logger.Log($"[SimGameInterruptManagerDisplayIfAvailablePatch] Suppressing popup display during time skip");
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
