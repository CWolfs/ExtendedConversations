using System;

using Harmony;

using BattleTech.UI;

using ExtendedConversations.State;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameInterruptManager), "QueuePauseNotification")]
  public class SimGameInterruptManagerQueuePauseNotificationPatch {
    static bool Prefix() {
      try {
        TimeSkipStateManager stateManager = TimeSkipStateManager.Instance;

        if (stateManager.IsTimeSkipActive && stateManager.DisablePopups) {
          Main.Logger.Log($"[SimGameInterruptManagerQueuePauseNotificationPatch] Suppressing pause notification queue during time skip");
          return false;
        }

        return true;
      } catch (Exception e) {
        Main.Logger.LogError("[SimGameInterruptManagerQueuePauseNotificationPatch] An error occurred in SimGameInterruptManagerQueuePauseNotificationPatch. Caught gracefully. " + e.StackTrace.ToString());
        return true;
      }
    }
  }
}
