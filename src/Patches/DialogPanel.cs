using UnityEngine;
using System;
using Harmony;

using BattleTech.UI;

using ExtendedConversations.Core;

// this.Sim.DialogPanel.Show
namespace ExtendedConversations {
  [HarmonyPatch(typeof(SGDialogWidget), "Show")]
  public class DialogPanelPatch {
    static void Prefix(SGDialogWidget __instance, ref string text) {
      // Main.Logger.Log($"Prefix running for DialogPanel with the text '{text}'");
      text = ConversationInterpolator.GetInstance().Interpolate(text);
    }
  }
}