using System;
using System.Collections.Generic;

using Harmony;

using BattleTech.UI;

using ExtendedConversations.State;

namespace ExtendedConversations {
  /// <summary>
  /// Universal popup suppression patch for Time Skip operations.
  /// Patches the AddInterrupt bottleneck method that ALL Queue* methods call,
  /// catching popups from Timeline mod and other mods that trigger events during time advancement.
  /// Uses configurable whitelist from Settings to allow critical interrupts while blocking others.
  /// </summary>
  [HarmonyPatch(typeof(SimGameInterruptManager), "AddInterrupt")]
  public class SimGameInterruptManagerAddInterruptPatch {
    static bool Prefix(SimGameInterruptManager.Entry entry) {
      try {
        TimeSkipStateManager stateManager = TimeSkipStateManager.Instance;

        if (stateManager.IsTimeSkipActive && stateManager.DisablePopups) {
          // Get whitelist from settings with fallback to hardcoded defaults
          List<string> allowedTypes = Main.Settings.TimeSkipAllowedInterrupts;

          string interruptTypeName = entry.type.ToString();
          bool isAllowed = allowedTypes.Contains(interruptTypeName);

          if (isAllowed) {
            Main.Logger.Log($"[SimGameInterruptManagerAddInterruptPatch] Allowing whitelisted interrupt during time skip: {interruptTypeName}");
            return true; // Allow whitelisted interrupt to be queued
          }

          Main.Logger.Log($"[SimGameInterruptManagerAddInterruptPatch] Suppressing interrupt during time skip: {interruptTypeName}");
          return false; // Block interrupt from being queued
        }

        return true; // Not in time skip, allow all interrupts
      } catch (Exception e) {
        Main.Logger.LogError("[SimGameInterruptManagerAddInterruptPatch] An error occurred in SimGameInterruptManagerAddInterruptPatch. Caught gracefully. " + e.StackTrace.ToString());
        return true; // On error, allow interrupt to proceed normally
      }
    }
  }
}
