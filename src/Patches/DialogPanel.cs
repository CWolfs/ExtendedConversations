using UnityEngine;
using System;
using Harmony;

using BattleTech.UI;

// this.Sim.DialogPanel.Show
namespace ExtendedConversations {
  [HarmonyPatch(typeof(DialogPanel))]
  [HarmonyPatch("Show")]
  public class DialogPanel {
    static void Prefix(DialogPanel __instance, string text) {
      Main.Logger.Log($"Prefix running for DialogPanel with the text {text}");
    }
  }
}