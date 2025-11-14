using System;

using Harmony;

using BattleTech;

using ExtendedConversations.State;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameState), "DeductQuarterlyFunds")]
  public class DeductQuarterlyFundsPatch {
    static bool Prefix(SimGameState __instance, int quarterPassed) {
      try {
        TimeSkipStateManager stateManager = TimeSkipStateManager.Instance;

        if (stateManager.IsTimeSkipActive && stateManager.DisableCost) {
          Main.Logger.Log($"[DeductQuarterlyFundsPatch] Skipping quarterly deduction during time skip (quarters: {quarterPassed})");

          // Still need to reset the quarter timer and trigger new quarter begin
          // Otherwise the game state gets out of sync
          __instance.OnNewQuarterBegin();
          __instance.RoomManager.RefreshDisplay();

          return false; // Skip the original method (prevents funds deduction)
        }

        return true; // Execute original method normally
      } catch (Exception e) {
        Main.Logger.LogError("[DeductQuarterlyFundsPatch] An error occurred in DeductQuarterlyFundsPatch. Caught gracefully. " + e.StackTrace.ToString());
        return true;
      }
    }
  }
}
